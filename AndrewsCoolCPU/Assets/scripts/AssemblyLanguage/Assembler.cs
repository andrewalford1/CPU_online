using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/**
* @brief   Helper class to assemble programs.
* @author  Andrew Alford
* @date    02/04/2019
* @version 1.1 - 06/04/2019
*/
public class Assembler
{
    //[program] Holds the program currently being assembled.
    private Program program = null;

    //[whiteSpaceRemover] Regex to find white space in a string.
    Regex whiteSpaceRemover = new Regex("[ ]{1,}");

    /**
     * @brief Helper method to wipe the program prior to its assembly.
     *        This is to prevent errors during the assembly process.
     */
    private void WipeProgram() {
        if (program.Equals(null)) {
            ConsoleControl.CONSOLE.LogError("Cannot wipe a program which doesn't exist.");
            return;
        }

        program.assembled = false;
        program.errors.Clear();
        program.data.Clear();
    }

    /**
     * @brief Removes all of the white space in the program code.
     */
    private void RemoveWhiteSpace() {
        for(int i = 0; i < program.code.Count; i++) {
            program.code[i] = whiteSpaceRemover.Replace(program.code[i], "@");
        }
    }

    public void AssembleProgram(Program programToAssemble) {
        program = programToAssemble;

        //Wipe the program clean prior to assembly.
        WipeProgram();

        RemoveWhiteSpace();

        Debug.Log("----start program code----");
        foreach(string code in program.code) {
            Debug.Log(code);
        }
        Debug.Log("----end program code----");

        //Break down insturctions into raw data.
        for (int i = 0; i < program.code.Count; i++) {
            string data = GetData(program.code[i], i + 1);
            if(!(data == null)) {
                program.data.Add(data);
            }
        }

        foreach(string data in program.data) {
            Debug.Log("Decimal: " + data);
        }

        //Log any errors if they occurred.
        if (program.errors.Count == 0) {
            program.assembled = true;
            ConsoleControl.CONSOLE.LogMessage("Program compiled with 0 error(s).");
        } else {
            ConsoleControl.CONSOLE.LogMessage("Program compiled with " + program.errors.Count + " error(s):");
            foreach (string error in program.errors) {
                ConsoleControl.CONSOLE.LogError(error);
            }
        }
    }

    //ERROR LOGGING...

    /**
     * @brief Logs an error message and adds it to the program file.
     * @param lineNumber - The line the error occurred on.
     * @param message - The error message to be logged.
     */
    private void NewError(int lineNumber, string message) {
        string error = "Line " + lineNumber + ": " + message;
        program.errors.Add(error);
    }

    //PARAMETER VALIDATION...

    /**
     * @brief Checks if too many parameters have been given.
     * @param parameters - A list of all parameters given.
     * @param max - The maximum number of parameters allowed.
     * @param lineNumber - The line being checked.
     * @returns 'true' if the error has occurred.
     */
    private bool TooManyParameters(List<string> parameters, int max, int lineNumber) {
        bool error = parameters.Count > max;
        if (error) { NewError(lineNumber, "Too many parameters given."); }
        return error;
    }

    /**
     * @brief Checks if a given parameter is a hexidecimal value.
     * @param parameter - The parameter being checked.
     * @param lineNumber - The line number of the parameter being checked.
     * @returns 'true' if the error has occurred.
     */
    private bool IsHex(string parameter, int lineNumber) {
        bool valid = InputValidation.IsHex(parameter);
        if (!valid) { NewError(lineNumber, "Parameter must be a hexidecimal value."); }
        return valid;
    }

    /**
     * @brief Checks if a given parameter is a decimal value.
     * @param parameter - The parameter being checked.
     * @param lineNumber - The line number of the parameter being checked.
     * @returns 'true' if the error has occurred.
     */
    private bool IsDecimal(string parameter, int lineNumber) {
        bool valid = InputValidation.IsDecimal(parameter);

        //Confirm it is a decimal number.
        if (!valid) { NewError(lineNumber, "Parameter must be a decimal value."); }
        
        //Check the numeber fits within a single byte.
        if(int.Parse(parameter.Substring(1)) > byte.MaxValue) {
            NewError(lineNumber, "Decimal values cannot exceed 255.");
            valid = false;
        }

        return valid;
    }

    /**
     * @brief Checks if a command has been called at the wrong time.
     * @param command - The command which has been called.
     * @param expectedLineNumuber - The line number it should be called at.
     * @param actualLineNumber - The line number it was actually called at.
     * @returns 'true' if the error has occurred.
     */
    private bool CalledAtWrongTime(string command, int expectedLineNumber, int actualLineNumber) {
        bool error = expectedLineNumber != actualLineNumber;
        if (error) {
            NewError(actualLineNumber, (command + " can only occur at line " + expectedLineNumber));
        }
        return error;
    }

    //DATA RETRIEVAL...

    /**
     * @brief Breaks down an instruction into it's key components.
     * @param label         - The section of code (optional)
     * @param command       - The command to be executed.
     * @param parameters    - The parameters to be used with the command.
     * @returns The key components of the instruction.
     */
    private List<string> BreakDownInstruction(
        in string instruction, out string label, 
        out string command, ref List<string> parameters) {
        
        //[components] Will holds all of the components 
        //which make up the instrucion.
        List<string> components = new List<string>();
        
        //Break down the instruction.
        string[] breakDown = instruction.Split('@');

        label = "\0";
        command = "\0";

        Debug.Log(breakDown.Length);

        if(breakDown.Length > 3) {
            //Unrecognised command.
            label = "\0";
            command = "\0";
        }
        else if(breakDown.Length == 3) {
            if(breakDown[0].Length > 0) {
                label = breakDown[0];
            }
            command = breakDown[1];
            parameters = BreakDownParameters(breakDown[2]);
        }
        else if(breakDown.Length == 2) {
            label = "\0";
            command = breakDown[0];
            parameters = BreakDownParameters(breakDown[1]);
        }
        else if(breakDown.Length == 1) {
            label = "\0";
            command = breakDown[0];
        }

        return components;
    }

    /**
     * @brief Breaks down an instruction into raw data. E.g., ADD $23,A becomes 0023.
     * @param instruction - The instruction to be broken down.
     * @param lineNumber - The line the instruction resides on.
     */
    private string GetData(string insturction, int lineNumber) {

        List<string> parameters = new List<string>();
        BreakDownInstruction(insturction, out string label, out string command, ref parameters);

        Debug.Log(label);
        Debug.Log(command);
        foreach(string param in parameters) {
            Debug.Log(param);
        }

        //Calculate the raw data based on the command.
        switch (command) {
            case ("ORG"):
                return CreateORGCommand(parameters, lineNumber);
            case ("ADD"):
                return CreateADDCommand(parameters, lineNumber);
            case ("SUB"):
                return CreateSUBCommand(parameters, lineNumber);
            case ("CMP"):
                return CreateCMPCommand(parameters, lineNumber);
            case ("MOVE"):
                return CreateMOVECommand(parameters, lineNumber);
            case ("HALT"):
                return CreateHALTCommand(parameters, lineNumber);
        }

        NewError(lineNumber, "Unrecognised command"); 
        return null;
    }

    /**
     * @brief Breaks down the parameters section of an 
     *        instruction into individual parameters.
     * @param parameters - The parameters section of an instruction.
     * @return Returns a list of individual parameters.
     */
    public List<string> BreakDownParameters(string parameters) {
        List<string> parameterList = new List<string>();

        string[] individualParameters = parameters.Split(',');

        if (individualParameters.Length == 0) {
            parameterList.Add(parameters);
        } else {
            foreach (string parameter in individualParameters) {
                parameterList.Add(parameter);
            }
        }
        return parameterList;
    }

    /**
     * @brief Extracts a piece of data as a quantifiable value.
     * @param data - The data to be extracted.
     * @param lineNumber - The line this data resides on.
     * @returns the extracted data.
     */
    private string ExtractData(string data, int lineNumber)
    {
        Debug.Log("Data: " + data);
        switch (data.ToCharArray()[0])
        {
            case ('^'): //It's a decimal value.
                if (IsDecimal(data, lineNumber))
                {
                    return InputValidation.DecimalToHex(data.Substring(1));
                }
                break;
            case ('$'): //It's a hex value.
                if (IsHex(data, lineNumber))
                {
                    return data.Substring(1);
                }
                break;
        }

        NewError(lineNumber, "Unrecognised Number format");
        return null;
    }

    //CASE SPECIFIC...

    /**
     * @brief Makes up an ORG command (where to load the program).
     * @returns the ORG command (or \0 if any errors occurred).
     */
    private string CreateORGCommand(List<string> parameters, int lineNumber) {
        //Error check parameters.
        if (CalledAtWrongTime("ORG", 1, lineNumber)) { return null; }
        else if (TooManyParameters(parameters, 1, lineNumber)) { return null; }
        else if (!IsHex(parameters[0], lineNumber)) { return null; }

        Number command = new Number();
        command.SetNumber("00" + parameters[0].Substring(1));
        return command.GetHex();
    }

    /**
     * @brief Makes up an ADD command.
     * @returns the ADD command (or \0 if any errors occurred).
     */
    private string CreateADDCommand(List<string> parameters, int lineNumber) {
        //Error check parameters.
        if(TooManyParameters(parameters, 2, lineNumber)) { return null; }
        else if(parameters[0].Equals(parameters[1])) {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        string operand = "00";
        string opcode = "00";

        Debug.Log("parameter 0: " + parameters[0] + "\tparameter 1: " + parameters[1]);

        //Immediate add.
        if(parameters[0].ToCharArray()[0].Equals('#')) {
            if(parameters[1].Equals("A")) {
                //Immediate add to GPA.
                operand = "00";
            }
            else if(parameters[1].Equals("B")) {
                //Immediate add to GPB.
                operand = "01";
            }
            opcode = ExtractData(parameters[0].Substring(1), lineNumber).Substring(2);
            //Number is invalid.
            if (opcode.Equals("\0")) { return opcode; }
        }
        //Direct ADD.
        else if(parameters[0].StartsWith("$") || parameters[0].StartsWith("^")) {
            if(parameters[1].Equals("A")) {
                //Direct add to GPA.
                operand = "02";
            }
            else if(parameters[1].Equals("B")) {
                //Direct add to GPB.
                operand = "03";
            }
            Debug.Log("direct add");
            //Extract the data.
            opcode = ExtractData(parameters[0], lineNumber);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //ADD GPB to GPA
        else if(parameters[0].Equals("A") && parameters[1].Equals("B")) {
            operand = "04";
        }
        //ADD GPA to GPB
        else if (parameters[0].Equals("B") && parameters[1].Equals("A")) {
            operand = "05";
        }
        else {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        Number command = new Number();
        command.SetNumber(operand + opcode);
        return command.GetHex();
    }

    /**
      * @brief Makes up an SUB command.
      * @returns the SUB command (or \0 if any errors occurred).
      */
    private string CreateSUBCommand(List<string> parameters, int lineNumber)
    {
        //Error check parameters.
        if (TooManyParameters(parameters, 2, lineNumber)) { return null; }
        else if (parameters[0].Equals(parameters[1]))
        {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        string operand = "00";
        string opcode = "00";

        //Immediate sub.
        if (parameters[0].ToCharArray()[0].Equals('#'))
        {
            if (parameters[1].Equals("A"))
            {
                //Immediate sub to GPA.
                operand = "06";
            }
            else if (parameters[1].Equals("B"))
            {
                //Immediate sub to GPB.
                operand = "07";
            }
            opcode = ExtractData(parameters[0].Substring(1), lineNumber).Substring(2);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //Direct SUB.
        else if (parameters[0].StartsWith("$") || parameters[0].StartsWith("^"))
        {
            if (parameters[1].Equals("A"))
            {
                //Direct sub to GPA.
                operand = "08";
            }
            else if (parameters[1].Equals("B"))
            {
                //Direct sub to GPB.
                operand = "09";
            }
            //Extract the data.
            opcode = ExtractData(parameters[0], lineNumber);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //SUB GPB to GPA
        else if (parameters[0].Equals("A") && parameters[1].Equals("B"))
        {
            operand = "0A";
        }
        //SUB GPA to GPB
        else if (parameters[0].Equals("B") && parameters[1].Equals("A"))
        {
            operand = "0B";
        }
        else
        {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        Number command = new Number();
        command.SetNumber(operand + opcode);
        return command.GetHex();
    }

    /**
      * @brief Makes up an CMP command.
      * @returns the CMP command (or \0 if any errors occurred).
      */
    private string CreateCMPCommand(List<string> parameters, int lineNumber)
    {
        //Error check parameters.
        if (TooManyParameters(parameters, 2, lineNumber)) { return null; }
        else if (parameters[0].Equals(parameters[1])) {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        string operand = "00";
        string opcode = "00";

        //Immediate CMP.
        if (parameters[0].ToCharArray()[0].Equals('#')) {
            if (parameters[1].Equals("A")) {
                //Immediate cmp to GPA.
                operand = "0C";
            }
            else if (parameters[1].Equals("B")) {
                //Immediate cmp to GPB.
                operand = "0D";
            }
            opcode = ExtractData(parameters[0].Substring(1), lineNumber).Substring(2);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //Direct CMP.
        else if (parameters[0].StartsWith("$") || parameters[0].StartsWith("^")) {
            if (parameters[1].Equals("A")) {
                //Direct cmp to GPA.
                operand = "0E";
            }
            else if (parameters[1].Equals("B")) {
                //Direct cmp to GPB.
                operand = "0F";
            }
            //Extract the data.
            opcode = ExtractData(parameters[0], lineNumber);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //CMP GPB to GPA
        else if (parameters[0].Equals("A") && parameters[1].Equals("B")) {
            operand = "10";
        }
        //CMP GPA to GPB
        else if (parameters[0].Equals("B") && parameters[1].Equals("A")) {
            operand = "11";
        }
        else {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        Number command = new Number();
        command.SetNumber(operand + opcode);
        return command.GetHex();
    }

    /**
      * @brief Makes up an CMP command.
      * @returns the CMP command (or \0 if any errors occurred).
      */
    private string CreateMOVECommand(List<string> parameters, int lineNumber)
    {
        //Error check parameters.
        if (TooManyParameters(parameters, 2, lineNumber)) { return null; }
        else if (parameters[0].Equals(parameters[1]))
        {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        string operand = "00";
        string opcode = "00";

        //Immediate MOVE.
        if (parameters[0].ToCharArray()[0].Equals('#'))
        {
            if (parameters[1].Equals("A"))
            {
                //Immediate move to GPA.
                operand = "12";
            }
            else if (parameters[1].Equals("B"))
            {
                //Immediate move to GPB.
                operand = "13";
            }
            opcode = ExtractData(parameters[0].Substring(1), lineNumber).Substring(2);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //Direct MOVE.
        else if (parameters[0].StartsWith("$") || parameters[0].StartsWith("^"))
        {
            if (parameters[1].Equals("A")) {
                //Direct move to GPA.
                operand = "14";
            }
            else if (parameters[1].Equals("B")) {
                //Direct move to GPB.
                operand = "15";
            }
            Debug.Log("direct move");
            //Extract the data (minus the brackets).
            opcode = ExtractData(parameters[0], lineNumber);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //MOVE GPB to GPA
        else if (parameters[0].Equals("A") && parameters[1].Equals("B"))
        {
            operand = "16";
        }
        //MOVE GPA to GPB
        else if (parameters[0].Equals("B") && parameters[1].Equals("A"))
        {
            operand = "17";
        }
        //MOVE GPA or GPB to direct address
        else if (parameters[1].StartsWith("$") || parameters[1].StartsWith("^"))
        {
            if (parameters[0].Equals("A")) {
                //direct store from GPA.
                operand = "18";
            }
            else if (parameters[0].Equals("B")) {
                //direct store from GPB.
                operand = "19";
            }
            Debug.Log("indirect move");
            //Extract the data (minus the brackets).
            opcode = ExtractData(parameters[1], lineNumber);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        //MOVE GPA or GPB to indirect address
        else if (parameters[1].StartsWith("(") && parameters[1].EndsWith(")"))
        {
            if (parameters[0].Equals("A"))
            {
                //Indirect store from GPA.
                operand = "1A";
            }
            else if (parameters[0].Equals("B"))
            {
                //Indirect store from GPB.
                operand = "1B";
            }
            //Extract the data (minus the brackets).
            opcode = ExtractData(parameters[1].Substring(1, parameters[1].Length - 2), lineNumber).Substring(2);
            //Number is invalid.
            if (opcode.Equals("\0")) { return null; }
        }
        else
        {
            NewError(lineNumber, "Unrecognised command");
            return null;
        }

        Number command = new Number();
        command.SetNumber(operand + opcode);
        return command.GetHex();
    }

    /**
     * @brief Makes up a HALT command (where to end the program).
     * @returns the HALT command (or \0 if any errors occurred).
     */
    private string CreateHALTCommand(List<string> parameters, int lineNumber) {
        //Error check parameters.
        if(CalledAtWrongTime("HALT", program.code.Count, lineNumber)) { return null; }
        else if(TooManyParameters(parameters, 0, lineNumber)) { return null; }

        Number command = new Number();
        command.SetNumber("FF00");
        return command.GetHex();
    }
}

