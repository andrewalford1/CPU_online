using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
 * @brief Abstract class representing a register.
 * @extends SimulatorComponent
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 2.0 - 19/02/2019
 */
public abstract class Register : SimulatorComponent
{
    //[decimalContent] Stores the content of the register in decimal format.
    private System.Int32 decimalContent = 0x0000;

    /**
     * @brief Retrieves the registers default value.
     * @return Returns the registers defualt value.
     */
    public string getStartingContent()
    {
        return "0000";
    }

    /**
     * @brief Retrieves the content held in the register.
     * @return Returns the content held in the register.
     */
    public string read()
    {
        Debug.Log(this.getID() + ": read - " + decimalContent);
 
        //Convert the registers content to a hexadecimal string.
        return fillBlanks(decimalContent.ToString("X"));
    }

    /**
     * @brief Sets the content of the register.
     * @param content - The new content for the register.
     */
    public void write(string content)
    {
        //Convert the input from a hex string to a decimal format.
        decimalContent = System.Int32.Parse(
            fillBlanks(content), 
            System.Globalization.NumberStyles.HexNumber
        );

        Debug.Log(this.getID() + ": read - " + decimalContent);
    }

    /**
     * @brief Fills in the blanks with 0's to represent hex values.
     * @param content - This is the content to be filled.
     * @return Returns the filled content. E.g., "A" would become "000A".
     */
    private string fillBlanks(string content)
    {
        //Fill in any blank spaces with 0's.
        if (content.Length < 4)
        {
            string blankSpaces = "";
            for (int i = 0; i < (4 - content.Length); i++)
            {
                blankSpaces += "0";
            }
            content = blankSpaces += content;
        }

        return content;
    }
}
