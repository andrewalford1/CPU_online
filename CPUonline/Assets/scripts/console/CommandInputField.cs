using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * @brief   Class to control the console's input field.
 * @extends InputField
 * @author  Andrew Alford
 * @date    23/03/2019
 * @version 1.1 - 29/03/2019
 */
public class CommandInputField : InputField
{
    /**
     * @brief Initialises the Input Field.
     */
    protected override void Start() {
        base.Start();

        //Force input to be all-caps.
        onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return addedChar.ToString().ToUpper().ToCharArray()[0]; };
    }

    /**
     * @brief Actions when the Input Field is selected.
     * @param eventData - Information about the selection.
     */
    public override void OnSelect(BaseEventData eventData) {
        //Delagate validation events to the Input Field.
        onEndEdit.AddListener(delegate {
            if (text.Length > 0) {
                if (text.Substring(0, 1).Equals("\\")) {
                    if (ConsoleControl.CONSOLE.ValidateCommand(text)) {
                        text = "\0";
                    }
                }
                else {
                    ConsoleControl.CONSOLE.LogMessage(text);
                    text = "";
                }
            }
            ActivateInputField();
        });
        base.OnSelect(eventData);
    }

    /**
     * @brief Actions when the Input Field is deselected
     * @param eventData - Information about the deselection
     */
    public override void OnDeselect(BaseEventData eventData) {
        //Remove all listeners until the Input Field 
        //is clicked on again.
        onEndEdit.RemoveAllListeners();
        base.OnDeselect(eventData);
    }
}
