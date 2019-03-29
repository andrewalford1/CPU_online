using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class containing the controls for the CPUs console.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 06/03/2019
 * @version 1.4 - 25/03/2019
 */
public class ConsoleControl : MonoBehaviour {
    //[CONSOLE] The single instance of this object.
    public static ConsoleControl CONSOLE;

    //[textTemplate] Template text which all console items will follow.
    [SerializeField] private GameObject textTemplate = null;

    //[maxItems] The total number of items that the console can contain.
    [SerializeField] private uint maxItems = 100;

    //[commandInput] The console's input field.
    [SerializeField] private CommandInputField commandInput = null;

    //[CU] A link to the control unit for executing commands.
    [SerializeField] private ControlUnit CU = null;

    //[textItems] A list of all the consoles content.
    private List<GameObject> textItems;

    //[previousItemIndex] Stores the index of previously logged
    //items. (For scrolling through previous logs).
    private int previousItemIndex = 0;

    /**
     * @brief Initialise the console.
     */
    private void Awake() {
        //DECLARE AS SINGLETON...

        //Code to make the Game Manager a singleton.
        if (!CONSOLE) {
            //If the game manager does not yet exist,
            //assign it to this instance.
            DontDestroyOnLoad(gameObject);
            CONSOLE = this;
        } else if (CONSOLE != this) {
            Destroy(gameObject);
        }

        //Create the list of text items.
        textItems = new List<GameObject>();
    }

    /**
     * @brief Updates the console once every frame.
     */
    private void Update() {
        if (commandInput.isFocused) {
            CheckToGetPreviousInput();
        }   
    }

    /**
     * @brief Sets the text to one of the previous inputs,
     *        using the arrow keys to select.
     */
    private void CheckToGetPreviousInput() {
        if (textItems.Count > 0) {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                previousItemIndex--;
                if (previousItemIndex < 0) {
                    previousItemIndex = 0;
                }
                commandInput.text = textItems[previousItemIndex].GetComponent<ConsoleItem>().GetText();
                commandInput.caretPosition = commandInput.text.Length;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                previousItemIndex++;
                if (previousItemIndex >= textItems.Count) {
                    previousItemIndex = textItems.Count - 1;
                }
                commandInput.text = textItems[previousItemIndex].GetComponent<ConsoleItem>().GetText();
                commandInput.caretPosition = commandInput.text.Length;
            }
        }
    }

    /**
     * @brief Validates text as a command.
     * @param text - The text to be validated.
     * @returns 'true' if the text is recognised
     *          as a valid command.
     */
    public bool ValidateCommand(string text) {
        //Do not perform any commands if the CPU is currently processing.
        if(CU.IsCurrentlyProcessing()) {
            LogError("Console cannot perform commands while the CU is processing");
            return false;
        }

        //If it is not a global command then
        //it may be a micro instuction.
        if(!CheckForGlobalCommand(text)) {
            Instruction instruction = CU.GetInstruction(text);
            if (instruction != null) {
                LogCommand(text);
                CU.SetCurrentInstructionFromConsole(instruction.ID);
                CU.Execute();
                return true;
            }
        } else {
            return true;
        }
        LogError("unrecognised Command : " + text);
        return false;
    }

    /**
     * @brief Checks if text is a global command that 
     *        would affect the entire simulator.
     * @param text - The text to be checked.
     * @returns 'true' if the text is recognised as a 
     *          global command.
     */
    public bool CheckForGlobalCommand(string text) {
        switch (text) {
            case ("\\FETCH"):
                LogCommand(text);
                CU.Fetch();
                return true;
            case ("\\DECODE"):
                LogCommand(text);
                CU.Decode();
                return true;
            case ("\\EXECUTE"):
                LogCommand(text);
                CU.Execute();
                return true;
            case ("\\RESET"):
                LogCommand(text);
                CU.Reset();
                return true;
        }
        return false;
    }

    /**
     * @brief Logs a command.
     * @param command - The command to be logged.
     */
    public void LogCommand(string command) {
        Write(command, Color.blue);
    }

    /**
     * @brief Logs an error message.
     * @param error - The error to be logged.
     */
    public void LogError(string error) {
        Write(error, Color.red);
    }

    /**
     * @brief Logs a regular message.
     * @param message - The message to be logged.
     */
    public void LogMessage(string message) {
        Write(message, Color.gray);
    }

    /**
     * @brief Writes a message to the console.
     * @param message - The message to be written.
     * @param colour - The colour of the messages text.
     */
    private void Write(string message, Color colour) {
        CheckToRemoveOldestItem();
        AddText(message, colour);
    }

    /**
     * @brief Checks to see if the maximum number
     *        of items has been reached.
     * @returns 'true' if this is the case.
     */
    private bool CheckToRemoveOldestItem() {
        //If the maximum number of items has been reached...
        if (textItems.Count == maxItems) {
            //Remove the oldest item from the list.
            GameObject tempItem = textItems[0];
            Destroy(tempItem.gameObject);
            textItems.Remove(tempItem);
            return true;
        }
        return false;
    }

    /**
     * @brief  Helper method to write given text to the console.
     * @param message - The message to be written.
     * @param colour - The colour of the messages text.
     */
    private void AddText(string message, Color colour) {
        //Do not log the message if nothing was written.
        if (message.Length == 0) { return; }
        //Add the new message.
        GameObject newText = Instantiate(textTemplate) as GameObject;
        newText.SetActive(true);
        newText.GetComponent<ConsoleItem>().SetText(message, colour);
        newText.transform.SetParent(textTemplate.transform.parent, false);
        textItems.Add(newText.gameObject);
        previousItemIndex = textItems.Count;
    }
}
