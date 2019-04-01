using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TextEditorControl : MonoBehaviour
{
    //[memory] A reference to the memory programs are loaded onto.
    [SerializeField] MemoryListControl memory = null;

    //[program] The program currently loaded into the text editor.
    private Program program = null;

    //[currentlyProecssing] 'True' whilst the text editor is in use. 
    //(Prevents multiple operations occurring at once).
    private bool currentlyProcessing = false;

    private IEnumerator Start() {
        yield return StartCoroutine(UploadProgram("currentProgram"));
    }

    private IEnumerator UploadProgram(string fileName) {
        if (currentlyProcessing) {
            ConsoleControl.CONSOLE.LogError("Cannot load program as Text Editor is currently processing.");
        }  else {
            currentlyProcessing = true;
            string url = Application.streamingAssetsPath + "/json/" + fileName + ".json";
            string json;

            //Check if we should use UnityWebRequest or File.ReadAllBytes
            if (url.Contains("://") || url.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();
                json = www.downloadHandler.text;
            }
            else {
                json = File.ReadAllText(url);
            }

            program = JsonUtility.FromJson<Program>(json);
            currentlyProcessing = false;
        }
    }

    /**
     * @brief Helper method to wipe the program prior to its assembly.
     *        This is to prevent errors during the assembly process.
     */
    private void WipeProgram() {
        if(program.Equals(null)) {
            ConsoleControl.CONSOLE.LogError("Cannot wipe a program which doesn't exist.");
            return;
        }

        program.assembled = false;
        program.errors.Clear();
        program.data.Clear();
    }

    public void Assemble() {
        if(currentlyProcessing) {
            ConsoleControl.CONSOLE.LogError("Cannot assemble program as Text Editor is currently processing.");
            return;
        }

        currentlyProcessing = true;

        //Wipe the program clean prior to assembly.
        WipeProgram();

        //Break down insturctions into raw data.
        for(int i = 0; i < program.code.Count; i++) {
            program.data.Add(GetData(program.code[i], i));
        }

        //Log any errors if they occurred.
        if(program.errors.Count == 0) {
            program.assembled = true;
            ConsoleControl.CONSOLE.LogMessage("Program compiled with 0 errors.");
        } else {
            ConsoleControl.CONSOLE.LogMessage("Program compiled with " + program.errors.Count + " errors:");
            foreach(string error in program.errors) {
                ConsoleControl.CONSOLE.LogError(error);
            }
        }
        currentlyProcessing = false;
    }

    /**
     * @brief Loads the currently loaded program onto memory.
     */
    public void LoadIntoMemory() {
        if(currentlyProcessing) {
            ConsoleControl.CONSOLE.LogError("Cannot load program as Text Editor is currently processing.");
            return;
        }

        currentlyProcessing = true;
        if(program.Equals(null)) {
            ConsoleControl.CONSOLE.LogError("You must first load a program");
            return;
        }
        else if(!program.assembled) {
            ConsoleControl.CONSOLE.LogError("Cannot load a program which hasn't been assembled");
        }
        else if(program.errors.Count > 0)
        {
            ConsoleControl.CONSOLE.LogError("Program: " + program.name 
                + " has errors and cannot be loaded.");
            return;
        }
        else if(program.data.Count <= 0)
        {
            ConsoleControl.CONSOLE.LogError("Program: " + program.name +
                " does not contain any data.");
            return;
        }

        memory.LoadProgram(program);
        currentlyProcessing = false;
    }

    private string GetData(string insturction, int lineNumber) {
        string[] instrutionBreakDown = insturction.Split(' ');

        string command = instrutionBreakDown[0];
        List<string> parameters = new List<string>();

        if (instrutionBreakDown.Length > 1)
        {
            for(int i = 1; i < instrutionBreakDown.Length; i++) {
                parameters.Add(instrutionBreakDown[i]);
                Debug.Log(instrutionBreakDown[i]);
            }
        }


        if(parameters.Count > 0) {
            Debug.Log("Command: " + command + " Parameters: " + parameters.Count);
        } 
        else
        {
            Debug.Log("Command: " + command + " Parameters: No Parameters");
        }

        switch(command)
        {
            case ("ORG"):
                return CreateORGCommand(parameters, lineNumber);
            case ("MOVE"):
                break;
            case ("HALT"):
                return new Number(0xFFFF).GetHex();
        }


        return new Number(0x0001).GetHex();
    }

    private string CreateORGCommand(List<string> parameters, int lineNumber ) {
        if(parameters.Count > 1) {
            ConsoleControl.CONSOLE.LogError("Error: Parameter must be a hex value. (Line: " + lineNumber + ")");
            program.errors.Add("Error: Too many parameters given. (Line: " + lineNumber + ")");
        }

        if(parameters[0].Length > 2) {
            ConsoleControl.CONSOLE.LogError("Error: Parameter must be a hex value. (Line: " + lineNumber + ")");
            program.errors.Add("Error: Parameter is too big. (Line: " + lineNumber + ")");
        }

        foreach(char c in parameters[0]) {
            if(InputValidation.ValidateAsHex(c).Equals('\0')) {
                ConsoleControl.CONSOLE.LogError("Error: Parameter must be a hex value. (Line: " + lineNumber + ")");
                program.errors.Add("Error: Parameter must be a hex value. (Line: " + lineNumber + ")");
            }
        }

        Number value = new Number();
        value.SetNumber(parameters[0]);
        return value.GetHex();
    }
}
