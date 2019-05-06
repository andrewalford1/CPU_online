using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
 * @brief Abstract class representing a register.
 * @extends SimulatorComponent
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 3.0 - 14/03/2019
 */
public abstract class Register : SimulatorComponent
{
    //[contents] Stores the contents of the register.
    protected Number contents = new Number();

    //[input] An input field for users to interact with this register.
    protected InputField input = null;

    /**
     * @brief Allocates an input field to this register.
     * @param inputField - The input field to be allocated.
     */
    public virtual void AllocateInputField(InputField inputField)
    {
        input = inputField;

        //Delagate input validation
        input.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input);
            Write(input.text);
        });
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
     * @brief Retrieves the content held in the register 
     *        in decimal integer format.
     * @return Returns the content held in the register
     *         in decimal integer format.
     */
    public int ReadInt()
    {
        Debug.Log(getID() + ": read - " + contents.ToString());
        return contents.GetSigned();
    }

    /**
     * @brief Retrieves the content held in the register.
     * @return Returns the content held in the register.
     */
    public string ReadString()
    {
        Debug.Log(getID() + ": read - " + contents.ToString());
        return contents.GetHex();
    }

    /**
     * @brief Sets the content of the register.
     * @param content - The new content for the register.
     */
    public void Write(string content)
    {
        contents.SetNumber(content);
        input.text = contents.GetHex();
        Debug.Log(getID() + ": write - " + contents.ToString());
        ConsoleControl.CONSOLE.logMessage(getID() + ": write - ");
        ConsoleControl.CONSOLE.logMessage("Decimal:\t" + contents.GetSigned() + "\nHex:\t" + contents.GetHex());
    }
}
