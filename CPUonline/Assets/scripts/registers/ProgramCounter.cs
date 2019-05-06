using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class representing the program counter.
 * @extends Register
 * @author Andrew Alford
 * @date 28/02/2019
 * @version 1.0 - 28/02/2019
 */
[CreateAssetMenu(menuName = "Program Counter")]
public class ProgramCounter : Register
{
    /**
     * @brief Allocates an input field to the PC. 
     *        (Overriden from the superclass).
     * @param inputField - The input field to be allocated.
     */
    public override void allocateInputField(InputField inputField)
    {
        input = inputField;

        //Delagate input validation
        input.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input);
            validateUserInput(input);
            write(input.text);
        });
    }

    /**
     * @brief Increments the PC by 1.
     */
    public void increment()
    {
        //[newValue] Stores the new value of the PC.
        int newValue = this.decimalContent + 1;

        //Reset to 0 if the new value is too big.
        if(newValue >= MemoryListControl.NUM_MEMORY_LOCATIONS)
        {
            newValue = 0;
        }

        //Write the new value to the PC.
        this.write(InputValidation.fillBlanks(newValue.ToString("X"), 4));
    }

    /**
     * @brief Checks if the user input is within range of the max memory address.
     * @param pcInput - The input field being validated.
     */
    public void validateUserInput(InputField pcInput)
    {
        //[currentValue] Convert the input from a hex string to decimal format.
        int currentValue = ushort.Parse(
            InputValidation.fillBlanks(pcInput.text, 4),
            System.Globalization.NumberStyles.HexNumber
        );

        //Cap the PC's value to the maximum address index.
        if (currentValue >= MemoryListControl.NUM_MEMORY_LOCATIONS)
        {
            pcInput.text = InputValidation.fillBlanks((MemoryListControl.NUM_MEMORY_LOCATIONS - 1).ToString("X"), 4);
        }
    }
}
