﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/**
 * @brief   Controls the text editor.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    01/04/2019
 * @version 1.0 - 01/04/2019
 */
public class TextEditorControl : MonoBehaviour {

    //[textEditor] The editor this script is controlling.
    [SerializeField] private TextEditor textEditor = null;

    //[memory] A reference to the memory programs are loaded onto.
    [SerializeField] MemoryListControl memory = null;

    //[program] The program currently loaded into the text editor.
    private Program program = null;

    private Assembler assembler = new Assembler();

    //[currentlyProecssing] 'True' whilst the text editor is in use. 
    //(Prevents multiple operations occurring at once).
    private bool currentlyProcessing = false;

    private IEnumerator Start() {
        yield return StartCoroutine(UploadProgram("currentProgram"));
        UpdateProgram();
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
        textEditor.LoadPorgram(program.name, program.code);
    }

    public void UpdateProgram() {
        program.code = textEditor.GetProgram();
    }

    /**
     * @brief Attempts to assemble the currently loaded program.
     */
    public void Assemble() {
        if(currentlyProcessing) {
            ConsoleControl.CONSOLE.LogError("Cannot assemble program as Text Editor is currently processing.");
            return;
        }

        currentlyProcessing = true;
        UpdateProgram();
        assembler.AssembleProgram(program);
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
}
