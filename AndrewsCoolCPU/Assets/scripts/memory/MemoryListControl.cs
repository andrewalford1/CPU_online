using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class to control all the memory locations.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 26/02/2019
 * @version 1.2 - 27/03/2019
 */
public class MemoryListControl : MonoBehaviour
{
    [SerializeField]
    GameObject inputFieldTemplate = null;

    //[NUM_MEMORY_LOCATIONS] Stores the total number of memory locations.
    public const int NUM_MEMORY_LOCATIONS = 256;

    //[memoryLocationsUI] The UI for all the memory locations.
    private List<GameObject> memoryLocationsUI = new List<GameObject>();

    //[slots] Keeps track of all the slots in memory.
    private List<MemorySlot> slots = new List<MemorySlot>();

    //[pointer] Points to the currently "focused on" memory slot
    //for accessing read/write functionality.
    private int pointer = 0;

    //[active] While 'true' memory can be manipulated by the user.
    private bool active = true;

    /**
     * @brief Initialisation code of the memory controls.
     */
    private void Start()
    {
        //Create memory locations
        for (int i = 0; i < NUM_MEMORY_LOCATIONS; i++)
        {
            Number ID = new Number((short)i);

            //Add a new memory location.
            memoryLocationsUI.Add(Instantiate(inputFieldTemplate) as GameObject);
            slots.Add((MemorySlot)ScriptableObject.CreateInstance("MemorySlot"));

            //Make the memory location active.
            memoryLocationsUI[i].SetActive(true);
            slots[i].Init();

            //Set the parent to the partent of the template we are spawning from.
            memoryLocationsUI[i].transform.SetParent(
                inputFieldTemplate.transform.parent,
                false
            );

            //Set the memory ID.
            memoryLocationsUI[i].GetComponentsInChildren<Text>()[1].text = ID.GetHex().Substring(2);

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
             memoryLocationsUI[slot.GetID()].GetComponentsInChildren<InputField>()[0]);
                }
            );
            //Retrieve the ID of memory slots when have had their values changed.
            memoryLocationsUI[locationIndex].GetComponentsInChildren<InputField>()[0].onEndEdit.AddListener(
                delegate { OnValueChanged(slot.GetID()); });
            locationIndex++;
        }
    }

    /**
     * @brief Callback function when a memory slot has been changed.
     *        Writes the new value to the memory slot.
     * @param memorySlotID - The ID of the memory slot that has been changed.
     */
    private void OnValueChanged(int memorySlotID)
    {
        Debug.Log("Memory Location " + memorySlotID + " has been changed.");
        slots[memorySlotID].Write(
            memoryLocationsUI[memorySlotID].GetComponentsInChildren<InputField>()[0].text
        );
        memoryLocationsUI[memorySlotID].GetComponentsInChildren<InputField>()[0].text = slots[memorySlotID].Read();
    }

    /**
     * @brief Clears the all memory slots to their starting values.
     */
    public void EmptyMemory()
    {
        //[loactionIndex] Used to point to a specific place in memory.
        int locationIndex = 0;
        //For every memory slot...
        foreach (MemorySlot slot in slots)
        {
            slot.Reset();
            //Reset the contents of the slot and UI.
            memoryLocationsUI[locationIndex].GetComponentsInChildren<InputField>()[0].text
                = slot.Read();

            locationIndex++;
        }
    }

    /**
     * @brief Generates an ID for a given memory location.
     * @parameter ID - The ID in integer form
     * @return A padded string of the ID. (E.g. 7 would return "007").
     */
    private string GetID(int id)
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
    public void SetPointer(int ID)
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
    public void WriteToMemorySlot(string value)
    {
        slots[pointer].Write(value);
        memoryLocationsUI[pointer].GetComponentsInChildren<InputField>()[0].text 
            = InputValidation.FillBlanks(value, 4);
    }

    /**
     * @brief Reads a hex string from the memory slot 
     *        currently being focused on.
     * @return Returns the content of the memory slot 
     *         currently in focus.
     */
    public string ReadFromMemorySlot()
    {
        return slots[pointer].Read();
    }

    /**
     * @brief Toggles interaction with the component's UI.
     * @param active - If 'true' then memory cannont be 
     *                 manually manipulated.
     */
    public void SetActive(bool active) {
        this.active = active;
        int locationIndex = 0;
        foreach(MemorySlot slot in slots) {
            memoryLocationsUI[locationIndex].
                GetComponentsInChildren<InputField>()[0].
                interactable = active;
            locationIndex++;
        }
    }

    /**
     * @returns 'True' if this component is active.
     */
    public bool IsActive() {
        return active;
    }
}
