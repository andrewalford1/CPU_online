using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Class representing a slot in memory.
 * @extends ScriptableObject
 * @author Andrew Alford
 * @date 26/02/2019
 * @version 1.2 - 14/03/2019
 */
public class MemorySlot : ScriptableObject
{
    //[ID] Static variable to track the ID of memory slots.
    private static int ID = 0;

    //[myID] This ID of this specific memory slot.
    private int myID;

    //[contents] Holds the contents of the memory slot.
    private Number contents = new Number();

    /**
     * @brief Initialises the memory slot.
     */
    public void Init()
    {
        //Set the ID and increment it for the next slot.
        myID = ID++;
    }

    /**
     * @returns This memory slots ID.
     */
    public int GetID()
    {
        return myID;
    }

    /**
     * @brief Resets the memory slot.
     */
    public void Reset()
    {
        contents.Reset();
    }

    /**
     * @returns The content held in the memory slot.
     */
    public string Read()
    {
        Debug.Log("Memory Slot " + GetID() + ": read - " + contents.ToString());
        return contents.GetHex();
    }

    /**
     * @brief Sets the content of the memory slot.
     * @param content - The content to be written 
     *                  to the memory slot.
     */
    public void Write(string content)
    {
        contents.SetNumber(content);
        Debug.Log("Memory Slot " + GetID() + ": write - " + contents.ToString());
    }
}
