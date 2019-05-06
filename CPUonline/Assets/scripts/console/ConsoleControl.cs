using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/**
 * @brief Class containing the controls for the CPUs console.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 06/03/2019
 * @version 1.0 - 06/03/2019
 */
public class ConsoleControl : MonoBehaviour
{
    public static ConsoleControl CONSOLE;

    //[textTemplate] Template text which all console items will follow.
    [SerializeField] private GameObject textTemplate = null;

    //[maxItems] The total number of items that the console can contain.
    [SerializeField] private uint maxItems = 100;

    [SerializeField] private InputField commandInput = null;

    //[textItems] A list of all the consoles content.
    private List<GameObject> textItems;

    //[commandList] A list of all available commands.
    private List<string> commandList;



    private void Awake()
    {
        //DECLARE AS SINGLETON...

        //Code to make the Game Manager a singleton (like me).
        if (!CONSOLE)
        {
            //If the game manager does not yet exist,
            //assign it to this instance.
            DontDestroyOnLoad(gameObject);
            CONSOLE = this;
        }
        else if (CONSOLE != this)
        {
            Destroy(gameObject);
        }

        textItems = new List<GameObject>();
        commandList = new List<string>();
        ReadCommandFile();

        commandInput.onEndEdit.AddListener(delegate {
            validateCommand(commandInput.text);
            commandInput.text = "";
        });
    }

    private void ReadCommandFile()
    {
        commandList.Add("\\fetch");
        commandList.Add("\\decode");
        commandList.Add("\\executet");
    }

    private void validateCommand(string text)
    {
        bool validCommand = false;

        foreach(string command in commandList)
        {
            if(text.Equals(command))
            {
                logCommand(command);
                validCommand = true;
            }
        }

        if(!validCommand)
        {
            logError("unrecognised Command : " + text);
        }
    }

    /**
     * @brief Logs a command.
     * @param command - The command to be logged.
     */
    public void logCommand(string command)
    {
        write(command, Color.blue);
    }

    /**
     * @brief Logs an error message.
     * @param error - The error to be logged.
     */
    public void logError(string error)
    {
        write(error, Color.red);
    }

    /**
     * @brief Logs a regular message.
     * @param message - The message to be logged.
     */
    public void logMessage(string message)
    {
        write(message, Color.gray);
    }

    /**
     * @brief Writes a message to the console.
     * @param message - The message to be written.
     * @param colour - The colour of the messages text.
     */
    private void write(string message, Color colour)
    {
        //Do not log the message if nothing was written.
        if(message.Length == 0)
        {
            return;
        }

        //If the maximum number of items has been reached...
        if(textItems.Count == maxItems)
        {
            //Remove the oldest item from the list.
            GameObject tempItem = textItems[0];
            Destroy(tempItem.gameObject);
            textItems.Remove(tempItem);
        }
        
        //Add the new message.
        GameObject newText = Instantiate(textTemplate) as GameObject;
        newText.SetActive(true);

        newText.GetComponent<ConsoleItem>().setText(message, colour);
        newText.transform.SetParent(textTemplate.transform.parent, false);

        textItems.Add(newText.gameObject);
    }
}
