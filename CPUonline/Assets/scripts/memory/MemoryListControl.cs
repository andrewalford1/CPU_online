using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/**
 * @brief Class to control all the memory locations.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 26/02/2019
 * @version 1.0 - 26/02/2019
 */
public class MemoryListControl : MonoBehaviour
{
    [SerializeField]
    GameObject inputFieldTemplate = null;

    public const int NUM_MEMORY_LOCATIONS = 256;

    //[memoryLocationsUI] The UI for all the memory locations.
    private List<GameObject> memoryLocationsUI = new List<GameObject>();

    //[slots] Keeps track of all the slots in memory.
    private List<MemorySlot> slots = new List<MemorySlot>();

    //[pointer] Points to the currently "focused on" memory slot
    //for accessing read/write functionality.
    private int pointer = 0;

    /**
     * @brief Initialisation code of the memory controls.
     */
    private void Start()
    {
        //Create memory locations
        for (int i = 0; i < NUM_MEMORY_LOCATIONS; i++)
        {
            //Add a new memory location.
            memoryLocationsUI.Add(Instantiate(inputFieldTemplate) as GameObject);
            slots.Add((MemorySlot)ScriptableObject.CreateInstance("MemorySlot"));

            //Make the memory location active.
            memoryLocationsUI[i].SetActive(true);
            slots[i].init();

            //Set the parent to the partent of the template we are spawning from.
            memoryLocationsUI[i].transform.SetParent(
                inputFieldTemplate.transform.parent,
                false
            );

            //Set the memory ID.
            memoryLocationsUI[i].GetComponentsInChildren<Text>()[1].text = getID(i);

            //Added input validation.
            memoryLocationsUI[i].GetComponentsInChildren<InputField>()[0].onValidateInput += 
                delegate (string input, int charIndex, char addedChar) 
                { return InputValidation.validateAsHex(addedChar); };

        }

        //[loactionIndex] Used to point to a specific place in memory.
        int locationIndex = 0;
        //For every memory slot...
        foreach (MemorySlot slot in slots)
        {
            //Callback functions for input UI.
            //Fill in the blanks.
            memoryLocationsUI[locationIndex].GetComponentsInChildren<InputField>()[0].onEndEdit.AddListener(
                delegate
                {
                    InputValidation.fillBlanks_Register(
             memoryLocationsUI[slot.getID()].GetComponentsInChildren<InputField>()[0]);
                }
            );
            //Retrieve the ID of memory slots when have had their values changed.
            memoryLocationsUI[locationIndex].GetComponentsInChildren<InputField>()[0].onEndEdit.AddListener(
                delegate { onValueChanged(slot.getID()); });
            locationIndex++;
        }
    }

    /**
     * @brief Callback function when a memory slot has been changed.
     *        Writes the new value to the memory slot.
     * @param memorySlotID - The ID of the memory slot that has been changed.
     */
    private void onValueChanged(int memorySlotID)
    {
        Debug.Log("Memory Location " + memorySlotID + " has been changed.");
        slots[memorySlotID].write(
            memoryLocationsUI[memorySlotID].GetComponentsInChildren<InputField>()[0].text
        );
    }

    /**
     * @brief Clears the all memory slots to their starting values.
     */
    public void emptyMemory()
    {
        //[loactionIndex] Used to point to a specific place in memory.
        int locationIndex = 0;
        //For every memory slot...
        foreach (MemorySlot slot in slots)
        {
            //Reset the contents of the slot and UI.
            memoryLocationsUI[locationIndex].GetComponentsInChildren<InputField>()[0].text
                = slot.getStartingContent();
            slot.write(slot.getStartingContent());
            locationIndex++;
        }
    }

    /**
     * @brief Generates an ID for a given memory location.
     * @parameter ID - The ID in integer form
     * @return A padded string of the ID. (E.g. 7 would return "007").
     */
    private string getID(int id)
    {
        //[stringID] contains the ID in string format.
        string stringID = id.ToString();

        //Padd out the ID.
        if (id < 100)
        {
            stringID = "0" + stringID;

            if (id < 10)
            {
                stringID = "0" + stringID;
            }
        }

        //Return the ID as a string.
        return stringID;
    }

    /**
     * @brief Allows you to set the memory location being focused on.
     * @param ID - The ID of the memory location to be focused on.
     */
    public void setPointer(int ID)
    {
        //Check the given ID is in range.
        if (ID > (slots.Count - 1) || ID < 0)
        {
            throw new System.ArgumentOutOfRangeException("The given ID is out of range");
        }
        else
        {
            //Allocate the new pointer ID.
            pointer = ID;
            Debug.Log("slot pointer set to: " + pointer);
        }
    }

    /**
     * @brief Writes a hex string to the memory slot being pointed at.
     * @param value - The value to be written to the memory slot.
     */
    public void writeToMemorySlot(string value)
    {
        slots[pointer].write(value);
        memoryLocationsUI[pointer].GetComponentsInChildren<InputField>()[0].text 
            = InputValidation.fillBlanks(value, 4);
    }

    /**
     * @brief Reads a hex string from the memory slot 
     *        currently being focused on.
     * @return Returns the content of the memory slot 
     *         currently in focus.
     */
    public string readFromMemorySlot()
    {
        return slots[pointer].read();
    }
}
