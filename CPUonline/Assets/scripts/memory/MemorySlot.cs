using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Class representing a slot in memory.
 * @extends ScriptableObject
 * @author Andrew Alford
 * @date 26/02/2019
 * @version 1.1 - 27/02/2019
 */
public class MemorySlot : ScriptableObject
{
    //[ID] Static variable to track the ID of memory slots.
    private static int ID = 0;

    //[myID] This ID of this specific memory slot.
    private int myID;

    //[decimalContent] Stores the content of the 
    //memory slot in deciaml format.
    private ushort decimalContent = 0x0000;

    /**
     * @brief Initialises the memory slot.
     */
    public void init()
    {
        //Set the ID and increment it for the next slot.
        myID = ID++;
    }

    /**
     * @brief Retrieves the ID of this memory slot.
     * @return Returns this memory slots ID.
     */
    public int getID()
    {
        return myID;
    }

    /**
     * @brief Retrieves the slots default value.
     * @return Returns the slots defualt value.
     */
    public string getStartingContent()
    {
        return "0000";
    }

    /**
     * @brief Retrieves the content held in the memory slot.
     * @return Returns the content held in the memory slot.
     */
    public string read()
    {
        Debug.Log("Reading " + decimalContent + 
            " from memory slot: " + this.getID());

        //Convert the content to a hexadecimal string.
        return InputValidation.fillBlanks(decimalContent.ToString("X"), 4);
    }

    /**
     * @brief Sets the content of the memory slot.
     * @param content - The content to be written 
     *                  to the memory slot.
     */
    public void write(string content)
    {
        //Convert the input from a hex string to decimal format.
        decimalContent = ushort.Parse(
            InputValidation.fillBlanks(content, 4),
            System.Globalization.NumberStyles.HexNumber
        );

        Debug.Log("Writing " + decimalContent + 
            " to memory slot: " + this.getID());
    }
}
