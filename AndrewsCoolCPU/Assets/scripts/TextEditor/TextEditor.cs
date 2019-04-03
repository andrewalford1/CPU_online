using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEditor : MonoBehaviour
{
    [SerializeField] private Text programName = null;

    public void LoadPorgram(string programName, List<string> content) {
        this.programName.text = programName;
    }
}
