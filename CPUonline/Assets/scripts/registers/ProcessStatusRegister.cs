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
        CARRY = 0b00000001,     //C
        ZERO = 0b00000010,     //Z 
        INTERRUPT_DISABLE = 0b00000100,     //I
        DECIMAL_MODE = 0b00001000,     //D
        BREAK_COMMAND = 0b00010000,     //B
        UNUSED = 0b00100000,     //-
        OVERFLOW = 0b01000000,     //V
        NEGATIVE = 0b10000000      //N
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
    public void allocateInputField(InputField inputField)
    {
        input = inputField;
        input.text = byteToString(PSRcontent);
        //Reset the PSR as values have been changed.
        reset();
    }

    /**
     * @brief Resets the contents of the PSR.
     */
    public void reset()
    {
        PSRcontent = STARTING_CONTENT;
        input.text = byteToString(PSRcontent);
    }

    /**
     * @brief Reads the value of the PSR.
     * @return Returns the PSRs content as a string.
     */
    public string read()
    {
        return byteToString(PSRcontent);
    }

    /**
     * @brief Converts a given byte into string format.
     * @param data - This is the byte to be converted.
     * @return Returns the converted string.
     */
    private string byteToString(byte data)
    {
        return System.Convert.ToString(data, 2).PadLeft(8, '0');
    }

    /**
     * @brief Allows a flag to set on the PSR.
     * @param flag  -   The flag to be set.
     * @param state -   The sate to set the flag (1/0).
     */
    public void setFlag(FLAGS flag, bool state)
    {
        //[flagSet] If 'true' then this flag is currently set on the PSR.
        bool flagSet = System.Convert.ToBoolean((PSRcontent & (byte)flag));

        //If the flag needs to be set then set it.
        if ((!flagSet && state == System.Convert.ToBoolean(1)) ||
            (flagSet && state == System.Convert.ToBoolean(0)))
        {
            //Toggle the flag.
            PSRcontent ^= (byte)flag;
            input.text = byteToString(PSRcontent);
            Debug.Log("Setting flag: " + flag +
                "\nNew PSR state: " + byteToString(PSRcontent));
        }

    }
}
