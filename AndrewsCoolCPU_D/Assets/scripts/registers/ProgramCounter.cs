using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class representing the program counter.
 * @extends Register
 * @author Andrew Alford
 * @date 28/02/2019
 * @version 1.2 - 14/03/2019
 */
public class ProgramCounter : Register
{
    public new void Start()
    {
        //Assign a name to the register.
        if (registerName)
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
        input.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input);
            ValidateUserInput(input);
            Write(input.text);
        });

        Write(input.text);
    }

    /**
     * @brief Increments the PC by 1.
     */
    public void Increment()
    {
        //[newValue] Stores the new value of the PC.
        int newValue = contents.GetSigned() + 1;

        //Reset to 0 if the new value is too big.
        if(newValue >= MemoryListControl.NUM_MEMORY_LOCATIONS)
        {
            newValue = 0;
        }

        //Write the new value to the PC.
        Write(InputValidation.FillBlanks(newValue.ToString("X"), 4));
    }

    /**
     * @brief Checks if the user input is within range of the max memory address.
     * @param pcInput - The input field being validated.
     */
    public void ValidateUserInput(InputField pcInput)
    {
        //[currentValue] Convert the input from a hex string to decimal format.
        int currentValue = ushort.Parse(
            InputValidation.FillBlanks(pcInput.text, 4),
            System.Globalization.NumberStyles.HexNumber
        );

        //Cap the PC's value to the maximum address index.
        if (currentValue >= MemoryListControl.NUM_MEMORY_LOCATIONS)
        {
            pcInput.text = InputValidation.FillBlanks(
                (MemoryListControl.NUM_MEMORY_LOCATIONS - 1).ToString("X"), 
                4
            );
        }
    }
}
