using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief   Manages the simulator's interacive text editor.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    03/04/2019
 * @version 1.0 - 03/04/2019
 */
public class TextEditor : MonoBehaviour
{
    //[programName] Displays the name of the currently loaded program.
    [SerializeField] private Text programName = null;

    //[editor] The area where user's write their code.
    [SerializeField] private InputField editor = null;

    public void LoadPorgram(string programName, List<string> content) {
        this.programName.text = programName;
        editor.text = "\0";

        for(int i = 0; i < content.Count; i++) {
            if(i == content.Count - 1) {
                editor.text += (content[i]);
            } else {
                editor.text += (content[i] + "\n");
            }
        }
    }

    public InputField GetEditor() => editor;

    public List<string> GetProgram() {
        Debug.Log(editor.text);

        List<string> code = new List<string>();
        foreach(string lineOfCode in editor.text.Split('\n')) {
            if(!lineOfCode.Equals("")) {
                code.Add(lineOfCode);
            }
        }

        return code;
    }
}
