﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public class LineOfCode {
    public int lineNumber;
    public string label = null;
    public string command = null;
    public List<string> parameters = null;
    public string data = null;
}

/**
 * @brief   Takes programs and assembles them.
 * @author  Andrew Alford
 * @date    02/04/2019
 * @version 2.0 - 07/04/2019
 */
public class Assembler_2
{
    //[SEPERATOR] Used to visualise where differnet
    //components of instructions begin/end.
    private const char SEPERATOR = '@';
    //[DECIMAL_SIGNITURE] data that starts with 
    //this value is decimal.
    private const char DECIMAL_SIGNITURE = '^';
    //[HEX_SIGNITURE] data that starts with 
    //this value is hexidecimal.
    private const char HEX_SIGNITURE = '$';
    //[IMMEDIATE_ADDRESSING_SIGNATURE] data that starts with 
    //this value uses immediate addressing.
    private const char IMMEDIATE_ADDRESSING_SIGNATURE = '#';
    //[LINE_NUMBER_OFFSET] An offset to represent the true line number.
    //(Because lists start from 0).
    private const int LINE_NUMBER_OFFSET = 1;

    /**
     * @brief Assembles the program.
     * @param program - A reference to the program 
     *                  being assembled.
     */
    public void AssembleProgram(ref Program program) {

        //Check if the program actually exists.
        if (program.Equals(null)) {
            ConsoleControl.CONSOLE.LogError("Cannot assemble a program that doesn't exist");
            return;
        }

        //Remove all previous assemble data (if any).
        WipeProgram(ref program);

        //Remove all the whitespace from the program.
        RemoveWhiteSpace(ref program);

        //Break down the code.
        ExtractMainBody(ref program, out List<LineOfCode> mainBody);
        ExtractSubroutines(ref program, out List<List<LineOfCode>> subroutines);

        //Assemble the code into raw hexidecimal data.
        GenerateData(ref program, ref mainBody, ref subroutines);

        Debug.Log("----PROGRAM DATA----");
        foreach(string data in program.data) {
            Debug.Log(data);
        }

        //TEMP for debugging.
        //Debug.Log("----MAIN BODY----");
        //foreach (LineOfCode line in mainBody)
        //{
        //    Debug.Log("Command:\t " + line.command + "\n" +
        //        "Num Parameters:\t" + line.parameters.Count
        //    );
        //}

        //Debug.Log("----Subroutines----");
        //for (int i = 0; i < subroutines.Count; i++)
        //{
        //    Debug.Log("Subroutine: " + i);
        //    foreach (LineOfCode line in subroutines[i])
        //    {
        //        Debug.Log("Command:\t " + line.command + "\n" +
        //            "Num Parameters:\t" + line.parameters.Count
        //        );
        //    }
        //} 
    }

    /**
     * @brief Removes all previously assembled data 
     *        and error so the program can be 
     *        reassembled.
     * @param program - A reference to the program 
     *                  being wiped.
     */
    private void WipeProgram(ref Program program) {
        program.assembled = false;
        program.errors.Clear();
        program.data.Clear();
    }

    /**
     * @brief Helper function to be used as a predicate for when a 
     *        line is nothing but whitespace.
     * @param line - The line being checked.
     */
    private static bool IsEmptyLine(string line) => line == SEPERATOR.ToString();

    /**
     * @brief Removes all of the whitespace from a program.
     * @param program - The program having it's 
     *                  whitespace removed.
     */
    private void RemoveWhiteSpace(ref Program program) {
        //[whiteSpaceRemover] A regular expression to 
        //detect whitespace in a string.
        Regex whiteSpaceRemover = new Regex("[ ]{1,}");

        //Remove the whitespace for each line of code in the program.
        for (int i = 0; i < program.code.Count; i++) {
            program.code[i] = whiteSpaceRemover.Replace(program.code[i], SEPERATOR.ToString());
        }

        //Remove all empty lines from the code.
        Predicate<string> emptyLine = IsEmptyLine;
        program.code.RemoveAll(emptyLine);
    }

    /**
     * @brief Logs an error message and adds it to the program file.
     * @param lineNumber - The line the error occurred on.
     * @param message - The error message to be logged.
     */
    private void NewError(ref Program program, in int LINE_NUMBER, string message)
    {
        string error = "Line " + LINE_NUMBER + ": " + message;
        program.errors.Add(error);
        ConsoleControl.CONSOLE.LogError(error);
    }

    /**
     * @brief Retrieves the main body of code
     * @param program - A refence to the program having it's main body extracted.
     * @param mainBody - A reference to where the main body will be extracted to.
     */
    private void ExtractMainBody(ref Program program, out List<LineOfCode> mainBody) {
        mainBody = new List<LineOfCode>();

        bool inMainBody = true;

        //Loop through all code in the program.
        for (int i = 0; i < program.code.Count; i++)
        {
            if(inMainBody) {
                //[line] Stores the broken down line of code
                LineOfCode line = ParseString(ref program, program.code[i], in i);

                //Only add to the main body if it is not a subroutine.
                if (string.IsNullOrEmpty(line.label)) {
                    mainBody.Add(line);
                }
                else {
                    //Throw an error if a subroutine occurred 
                    //before the main body of code.
                    if(i == 0) {
                        NewError(
                            ref program, 
                            in line.lineNumber, 
                            "Main code must occur before subroutines"
                        );
                    }
                    inMainBody = false;
                }

            }
        }
    }

    /**
     * @brief Reads through the code and extracts all the subroutines.
     * @param program - A reference to the program being read.
     * @param subroutines - Will store the subroutines extacted.
     */
    private void ExtractSubroutines(ref Program program, out List<List<LineOfCode>> subroutines) {
        subroutines = new List<List<LineOfCode>>();

        bool inSubroutine = false;

        //Loop through all code in the program.
        for(int i = 0; i < program.code.Count; i++) {
            //[line] Stores the broken down line of code.
            LineOfCode line = ParseString(ref program, program.code[i], in i);

            //If the code does have a label (indicating there is a subroutine).
            if(!string.IsNullOrEmpty(line.label) && !inSubroutine) {

                List<LineOfCode> subroutine = new List<LineOfCode>();

                inSubroutine = true;
                int startOfSubroutine = i;

                //While the subroutine is being read...
                while(inSubroutine) {

                    //Loop through each line in the subroutine.
                    for(int j = startOfSubroutine; j < program.code.Count; j++) {
                        if(inSubroutine) {
                            //The first line will always have a label.
                            if (j == startOfSubroutine)
                            {
                                //Check if the subroutine has not already been declared.
                                foreach (List<LineOfCode> previousSubroutine in subroutines) {
                                    if(previousSubroutine.Count > 0) {
                                        if(previousSubroutine[0].label.Equals(line.label)) {
                                            NewError(
                                                ref program, 
                                                in line.lineNumber, 
                                                "Subroutine already declared"
                                            );
                                        }
                                    }
                                }
                                subroutine.Add(line);
                            }
                            else {
                                LineOfCode subCode = ParseString(ref program, program.code[j], in j);

                                //If no new label has occurred then this
                                //line of code is part of the subroutine.
                                if(string.IsNullOrEmpty(subCode.label)) {
                                    subroutine.Add(subCode);
                                }
                                else {
                                    //Move to the next subroutine.
                                    inSubroutine = false;
                                }
                            }
                        }
                    }
                    inSubroutine = false;
                }

                //Add the subroutine to the list of subroutines.
                subroutines.Add(subroutine);
            }
        }
    }

    /**
     * @brief Breaks down a line of code in string format.
     * @param lineOfCode_str    - The line of code in string format.
     * @param LINE_NUMBER       - Where this line of code occurs.
     */
    private LineOfCode ParseString(ref Program program, string lineOfCode_str, in int LINE_NUMBER) {
        //[line] Will hold the line of code.
        LineOfCode line = new LineOfCode
        {
            lineNumber = LINE_NUMBER + LINE_NUMBER_OFFSET,
            label = null,
            command = null,
            parameters = new List<string>(),
            data = null
        };

        //Split up the string.
        string[] breakDown = lineOfCode_str.Split(SEPERATOR);

        if (breakDown.Length > 3 || breakDown.Length <= 0)
        {
            //Too much / too little information.
            NewError(ref program, in LINE_NUMBER, "Unrecognised command");
        } 
        else {
            switch(breakDown.Length) {
                case (3): //label, command, and parameters given.
                    line.label = breakDown[0];
                    line.command = breakDown[1];
                    line.parameters = ParseParameters(breakDown[2]);
                    break;
                case (2):
                    //label && command given
                    if (breakDown[0].StartsWith("_")) {
                        line.label = breakDown[0];
                        line.command = breakDown[1];
                    }
                    //Command && parameters given.
                    else if (breakDown[0].Length > 0) {
                        line.command = breakDown[0];
                        line.parameters = ParseParameters(breakDown[1]);
                    } 
                    else {
                        //Just command given (e.g., HALT).
                        line.command = breakDown[1];
                    }
                    break;
                case (1): //Just command given (e.g., HALT).
                    line.command = breakDown[0];
                    break;
            }
        }

        return line;
    }

    /**
     * @brief Breaks down parameters.
     * @param parameters_str - The parameters to be parsed.
     */
    private List<string> ParseParameters(string parameters_srt) {

        //[parameters] Will hold all the parameters once they are parsed.
        List<string> parameters = new List<string>();

        //[individualParameters] Stores each parameter seperatly.
        string[] individualParameters = parameters_srt.Split(',');

        //If only one parameter is given then add the entire list.
        if (individualParameters.Length == 0) {
            parameters.Add(parameters_srt);
        }
        else {
            //Add all the parameters to the list.
            foreach (string parameter in individualParameters) {
                parameters.Add(parameter);
            }
        }

        return parameters;
    }

    /**
     * @brief Works through the code and translates it to 
     *        executable hexidecimal instructions.
     * @param programData - Where the data will be stored.
     * @param mainBody - The main body of code to be translated.
     * @param subroutines - All the subroutines to be translated.
     */
    private void GenerateData(
        ref Program program,
        ref List<LineOfCode> mainBody,
        ref List<List<LineOfCode>> subroutines)
    {
        foreach(LineOfCode line in mainBody) {

            //Execute the given command.
            switch (line.command) {
                case ("ORG"):
                    line.data = CreateORGCommand(ref program, line);
                    break;
                case ("ADD"):
                    line.data = CreateAddCommand(ref program, line);
                    break;
                case ("HALT"):
                    line.data = CreateHALTCommand(ref program, line);
                    break;
            }


            //If data was generate then add it.
            if (!string.IsNullOrEmpty(line.data)) {
                program.data.Add(line.data);
            }
            else {
                //If the command is unknown 
                //then throw an error.
                NewError(
                    ref program,
                    in line.lineNumber,
                    "Unrecognised command"
                );
            }
        }
    }

    /**
     * @brief Checks if a given command was called at the wrong time.
     * @param program               - The program being assembled.
     * @param command               - The command being checked.
     * @param expectedLineNumber    - The line this command should occur on.
     * @param actualLineNumber      - The actual line this command occurred on.
     */
    private bool CalledAtWrongTime(
        ref Program program, 
        in string command, 
        in int expectedLineNumber, 
        in int actualLineNumber)
    {
        //[error] If 'true' then the error has occurred.
        bool error = expectedLineNumber != actualLineNumber;

        //Report the error.
        if(error) {
            NewError(
                ref program, 
                actualLineNumber, 
                (command + " can only occur on line " + expectedLineNumber)
            );
        }

        return error;
    }

    /**
     * @brief Checks if a given command has been allocated too many parameters.
     * @param program       - The program being assembled.
     * @param maxParameters - The maximum amount of parameters allowed.
     * @param lineOfCode    - The line of code being checked.
     */
    private bool TooManyParameters(
        ref Program program,
        in int maxPameters,
        in LineOfCode lineOfCode)
    {

        //[error] If 'true' then the error has occurred.
        bool error = lineOfCode.parameters.Count > maxPameters;

        //Report the error.
        if(error) {
            NewError(
                ref program, 
                lineOfCode.lineNumber, 
                ("Can only have up to " + maxPameters + " parameters")
            );
        }

        return error;
    }

    /**
     * @brief Takes a value and parses it as hex.
     * @param program       - The program being assembled.
     * @param data          - The data being parsed.
     * @param lineNumber    - The line number this data comes from.
     * @returns the value in hex format.
     */
    private string ParseHex(ref Program program, string data, in int lineNumber) {

        switch(data.ToCharArray()[0]) {
            case (DECIMAL_SIGNITURE):
                if(InputValidation.IsDecimal(data)) {
                    return InputValidation.DecimalToHex(data.Substring(1)).Substring(2);
                }
                break;
            case (HEX_SIGNITURE):
                if(InputValidation.IsHex(data)) {
                    return data.Substring(1);
                }
                break;
        }

        //If the value is unrecognised report it as an error.
        NewError(ref program, lineNumber, "Unrecognised number format.");

        return "";
    }

    /**
     * @brief Checks if a given piece of data can be used as a valid address.
     * @param hexData - The data being checked.
     * @returns 'true' if the data can be used as an address.
     */
    private bool IsValidAddress(in string hexData) {
        Number dataAsNumber = new Number();
        dataAsNumber.SetNumber(hexData);
        return dataAsNumber.GetUnsigned() <= MemoryListControl.NUM_MEMORY_LOCATIONS;
    }

    /**
     * @brief Translates a given line of code into an 'ORG' command.
     * @param program       - A reference to the program being assembled.
     * @param lineOfCode    - The line of code being translated.
     */
    private string CreateORGCommand(ref Program program, LineOfCode lineOfCode) {
        //ORG must be the first line of code.
        if (CalledAtWrongTime(ref program, "ORG", 1, lineOfCode.lineNumber)) { return ""; }
        //ORG can only occur once.
        foreach(string data in program.data) {
            if (data.Substring(0, 2).Equals("00")) {
                NewError(
                    ref program, 
                    lineOfCode.lineNumber, 
                    "ORG has already been declared"
                );
                return "";
            }
        }
        //ORG can only have one parameter.
        if(TooManyParameters(ref program, 1, lineOfCode)) { return ""; }

        string opcode = "00";

        //Convert the parameter to hexidecimal 
        //to retrieve the address.
        string operand = ParseHex(
            ref program, 
            lineOfCode.parameters[0], 
            lineOfCode.lineNumber
        );

        //The operand must be a valid address.
        if(!IsValidAddress(operand)) { return ""; }

        ///If either the opcode or operand hasn't been calculated then return nothing.
        if(string.IsNullOrEmpty(opcode) || string.IsNullOrEmpty(operand)) { return ""; }

        //Retrun the data.
        return opcode + operand;
    }

    /**
     * @brief Translates a given line of code into an 'ADD' command.
     * @param program       - A reference to the program being assembled.
     * @param lineOfCode    - The line of code being translated.
     */
    private string CreateAddCommand(ref Program program, LineOfCode lineOfCode) {
        //ADDs can only have two parameters.
        if (TooManyParameters(ref program, 2, lineOfCode)) { return ""; }
        //ADDs cannot have identical parameters.
        if (lineOfCode.parameters[0].Equals(lineOfCode.parameters[1])) { return ""; }

        string opcode = "";
        string operand = "";

        //IMMEDIATE ADD
        if (lineOfCode.parameters[0].StartsWith(IMMEDIATE_ADDRESSING_SIGNATURE.ToString())) {
            switch (lineOfCode.parameters[1]) {
                case ("A"): //IMMEDIATE ADD to GPA.
                    opcode = "00";
                    break;
                case ("B"): //IMMEDIATE ADD to GPB.
                    opcode = "01";
                    break;
            }
            //Convert the parameter to hexidecimal to retrieve the address.
            operand = ParseHex(ref program, lineOfCode.parameters[0].Substring(1), lineOfCode.lineNumber);
        }
        //DIRECT ADD
        else if (lineOfCode.parameters[0].StartsWith(DECIMAL_SIGNITURE.ToString()) ||
                lineOfCode.parameters[0].StartsWith(HEX_SIGNITURE.ToString()))
        {
            switch (lineOfCode.parameters[1]) {
                case ("A"): //DIRECT ADD to GPA.
                    opcode = "02";
                    break;
                case ("B"): //DIRECT ADD to GPB.
                    opcode = "03";
                    break;
            }
            //Convert the parameter to hexidecimal to retrieve the address.
            operand = ParseHex(ref program, lineOfCode.parameters[0], lineOfCode.lineNumber);
        }
        //ADD GPB to GPA
        else if (lineOfCode.parameters[0].Equals("A") && lineOfCode.parameters[1].Equals("B")) {
            opcode = "04";
            operand = "00";
        }
        //ADD GPA to GPB
        else if (lineOfCode.parameters[0].Equals("B") && lineOfCode.parameters[1].Equals("A")) {
            opcode = "05";
            operand = "00";
        }

        //The operand must be a valid address.
        if (!IsValidAddress(operand)) { return ""; }

        ///If either the opcode or operand hasn't been calculated then return nothing.
        if (string.IsNullOrEmpty(opcode) || string.IsNullOrEmpty(operand)) { return ""; }

        //Retrun the data.
        return opcode + operand;
    }

    /**
     * @brief Translates a given line of code into a 'HALT' command.
     * @param program       - A reference to the program being assembled.
     * @param lineOfCode    - The line of code being translated.
     */
    private string CreateHALTCommand(ref Program program, LineOfCode lineOfCode) {
        //HALT must be the last line called.
        if (CalledAtWrongTime(ref program, "ORG", program.code.Count, lineOfCode.lineNumber)) {
            return "";
        }
        //HALT must not have any parameters.
        if (TooManyParameters(ref program, 0, lineOfCode)) { return ""; }

        string opcode = "FF";
        string operand = "00";

        return opcode + operand;
    }
}
