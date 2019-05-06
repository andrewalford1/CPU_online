using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class representing the Process Status Register.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 05/03/2019
 * @version 1.1 - 05/03/2019
 */
[CreateAssetMenu(menuName = "Process Status Register")]
public class ProcessStatusRegister : ScriptableObject
{

    //[FLAGS] All of the different flags held in the PSR.
    public enum FLAGS : byte
    {
        CARRY       =   0b00000001,     //C
        ZERO        =   0b00000010,     //Z 
        NEGATIVE    =   0b00000100,     //N
        OVERFLOW    =   0b00001000,     //V
        UNUSED1     =   0b00010000,     //-
        UNUSED2     =   0b00100000,     //-
        UNUSED3     =   0b01000000,     //-
        UNUSED4     =   0b10000000      //-
    }

    //[STARTING_CONTENT] Holds the starting content of the register.
    private const byte STARTING_CONTENT = 0b00000000;

    //[PSRcontent] Stores the state of each PSR flag.
    private byte PSRcontent = STARTING_CONTENT;

    //[input] An input field for users to interact with this register.
    private InputField input = null;

    /**
     * @brief Allocates an input field to this register.
     * @param inputField - The input field to be allocated.
     */
    public void AllocateInputField(InputField inputField)
    {
        input = inputField;
        input.text = ByteToString(PSRcontent);
        //Reset the PSR as values have been changed.
        Reset();
    }

    /**
     * @brief Resets the contents of the PSR.
     */
    public void Reset()
    {
        PSRcontent = STARTING_CONTENT;
        input.text = ByteToString(PSRcontent);
    }

    /**
     * @brief Reads the value of the PSR.
     * @return Returns the PSRs content as a string.
     */
    public string Read()
    {
        return ByteToString(PSRcontent);
    }

    /**
     * @brief Converts a given byte into string format.
     * @param data - This is the byte to be converted.
     * @return Returns the converted string.
     */
    private string ByteToString(byte data)
    {
        return Convert.ToString(data, 2).PadLeft(8, '0');
    }

    /**
     * @brief Allows a flag to set on the PSR.
     * @param flag  -   The flag to be set.
     * @param state -   The sate to set the flag (1/0).
     */
    public void SetFlag(FLAGS flag, bool state)
    {
        //[flagSet] If 'true' then this flag is currently set on the PSR.
        bool flagSet = Convert.ToBoolean((PSRcontent & (byte)flag));

        //If the flag needs to be set then set it.
        if ((!flagSet && state == Convert.ToBoolean(1)) ||
            (flagSet && state == Convert.ToBoolean(0)))
        {
            //Toggle the flag.
            PSRcontent ^= (byte)flag;
            input.text = ByteToString(PSRcontent);
            Debug.Log("Setting flag: " + flag +
                "\nNew PSR state: " + ByteToString(PSRcontent));
        }

    }
}
