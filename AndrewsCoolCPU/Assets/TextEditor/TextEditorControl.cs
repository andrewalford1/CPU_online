using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class TextEditorControl : MonoBehaviour
{
    //[memory] A reference to the memory programs are loaded onto.
    [SerializeField] MemoryListControl memory = null;

    //[program] The program currently loaded into the text editor.
    private Program program = null;

    /**
     * @brief Initialises the Text Editor.
     */
    IEnumerator Start() {
        yield return LoadProgram("currentProgram");
        LoadIntoMemory();
    }

    private IEnumerator LoadProgram(string fileName) {
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
    }

    /**
     * @brief Loads the currently loaded program onto memory.
     */
    private void LoadIntoMemory() {
        if(program.Equals(null)) {
            ConsoleControl.CONSOLE.LogError("You must first load a program");
            return;
        }
        else if(!program.assembled) {
            ConsoleControl.CONSOLE.LogError("Cannot load a program which hasn't been assembled");
        }
        else if(program.errors.Length > 0)
        {
            ConsoleControl.CONSOLE.LogError("Program: " + program.name 
                + " has errors and cannot be loaded.");
            return;
        }
        else if(program.data.Length <= 0)
        {
            ConsoleControl.CONSOLE.LogError("Program: " + program.name +
                " does not contain any data.");
            return;
        }

        memory.LoadProgram(program);
    }
}
