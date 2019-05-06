using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
 * @brief Abstract class representing a register.
 * @extends SimulatorComponent
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 2.4 - 05/03/2019
 */
public abstract class Register : SimulatorComponent
{
    //[MAX_VALUE] The maxium value that a register can hold.
    protected const ushort MAX_VALUE = 0xFFFF;

    //[STARTING_CONTENT] Holds the starting content for the register.
    protected const ushort STARTING_CONTENT = 0x0000;

    //[decimalContent] Stores the content of the register in decimal format.
    protected ushort decimalContent = STARTING_CONTENT;

    //[input] An input field for users to interact with this register.
    protected InputField input = null;

    /**
     * @brief Allocates an input field to this register.
     * @param inputField - The input field to be allocated.
     */
    public virtual void allocateInputField(InputField inputField)
    {
        input = inputField;

        //Delagate input validation
        input.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input);
            write(input.text);
        });
    }

    /**
     * @brief Resets the register.
     */
    public void reset()
    {
        decimalContent = STARTING_CONTENT;
        input.text = InputValidation.fillBlanks((STARTING_CONTENT).ToString("X"), 4);
    }

    /**
     * @brief Retrieves the content held in the register.
     * @return Returns the content held in the register.
     */
    public string read()
    {
        Debug.Log(this.getID() + ": read - " + decimalContent);
 
        //Convert the registers content to a hexadecimal string and return it.
        return InputValidation.fillBlanks(decimalContent.ToString("X"), 4);
    }

    /**
     * @brief Retrieves the content held in the register 
     *        in decimal integer format.
     * @return Returns the content held in the register
     *         in decimal integer format.
     */
    public int readAsDecimalInt()
    {
        Debug.Log(this.getID() + ": read - " + decimalContent);

        return decimalContent;
    }

    /**
     * @brief Sets the content of the register.
     * @param content - The new content for the register.
     */
    public void write(string content)
    {
        //Convert the input from a hex string to decimal format.
        decimalContent = ushort.Parse(
            InputValidation.fillBlanks(content, 4), 
            System.Globalization.NumberStyles.HexNumber
        );

        Debug.Log(this.getID() + ": write - " + decimalContent);
    }
}
