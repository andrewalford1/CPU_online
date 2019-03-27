using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


/**
 * @brief Abstract class representing a register.
 * @extends SimulatorComponent
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 4.1 - 21/03/2019
 */
public abstract class Register : SimulatorComponent
{
    //[input] An input field for users to interact with this register.
    [SerializeField]  protected InputField input = null;

    [SerializeField] protected Text registerName = null;

    //[contents] Stores the contents of the register.
    protected Number contents = new Number();

    /**
     * @brief Initialises the Register.
     */
    public void Start()
    {
        //Assign a name to the register.
        if(registerName)
        {
            id = registerName.text;
        }
        else
        {
            id = "reg";

        }

        //Delagate input validation
        input.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input.onEndEdit.AddListener(delegate
        {
            InputValidation.fillBlanks_Register(input);
            Write(input.text);
        });
        Write(input.text);
    }

    /**
     * @brief Resets the register.
     */
    public void Reset()
    {
        contents.Reset();
        input.text = contents.GetHex();
    }

    /**
     * @returns the content as a signed number.
     */
    public short ReadSigned()
    {
        Debug.Log(GetID() + ": read - " + contents.ToString());
        return contents.GetSigned();
    }

    /**
     * @returns the content as an unsigned number.
     */
    public ushort ReadUnsigned()
    {
        Debug.Log(GetID() + ": read - " + contents.ToString());
        return contents.GetUnsigned();
    }

    /**
     * @returns the conotent as a hexadecimal string.
     */
    public string ReadString()
    {
        Debug.Log(GetID() + ": read - " + contents.ToString());
        return contents.GetHex();
    }

    /**
     * @returns the opcode currently being stored.
     */
    public ushort Opcode()
    {
        return (ushort)Convert.ToInt16(contents.GetUnsignedString().Substring(0, 2), 16);
    }

    /**
     * @returns the operand currently being stored.
     */
    public ushort Operand()
    {
        return (ushort)Convert.ToInt16(contents.GetUnsignedString().Substring(2, 2), 16);
    }

    /**
     * @returns the opcode currently being stored (in string format).
     */
    public string OpcodeString()
    {
        return InputValidation.FillBlanks(Opcode().ToString("X"), 4);
    }

    /**
     * @returns the operand currently being stored (in string format).
     */
    public string OperandString()
    {
        return InputValidation.FillBlanks(Operand().ToString("X"), 4);
    }

    /**
     * @brief Sets the content of the register.
     * @param content - The new content for the register.
     */
    public void Write(string content)
    {
        contents.SetNumber(content);
        input.text = contents.GetHex();
        Debug.Log(GetID() + ": write - " + contents.ToString());
    }
}
