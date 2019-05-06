using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class representing the Arithmetic Logic Unit.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 03/03/2019
 * @version 1.2 - 06/03/2019
 */
public class ArithmeticLogicUnit : MonoBehaviour
{
    //[MAX_VALUE] The maximum value that the ALU can hold.
    private const ushort MAX_VALUE = 0xFFFF;

    //[STARTING_CONTENT] Holds the starting content for the ALU fields.
    protected const ushort STARTING_CONTENT = 0x0000;

    //[x] Stores the value of ALUx (parameter A).
    private ushort x = STARTING_CONTENT;
    //[y] Stores the value of ALUy (Parameter B).
    private ushort y = STARTING_CONTENT;
    //[z] Stores the value of ALUz (Calculation using A and B).
    private ushort z = STARTING_CONTENT;

    //[CIRCUITRY] Defines the different 
    //types of circuitry for the ALU.
    public enum CIRCUITRY
    {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION
    }

    //[defualtCircuitry] The defualt circuitry used by the ALU.
    private CIRCUITRY defualtCircuitry = CIRCUITRY.ADDITION;
    //[currentCircuitry] The circuitry currently 
    //being used by the ALU.
    private CIRCUITRY currentCircuitry = CIRCUITRY.ADDITION;
    //[circuitryGraphic] A graphical representation of the ALU's circuitry.
    private Text circuitryGraphic = null;

    //[input_x] An input field for users to interact with ALU x.
    private InputField input_x = null;
    //[input_y] An input field for users to interact with ALU y.
    private InputField input_y = null;
    //[input_z] An input field for users to interact with ALU z.
    private InputField input_z = null;

    private ProcessStatusRegister PSR;

    /**
     * @brief Links the PSR to the ALU to show the state of operations.
     * @param psr - The PSR to be linked.
     */
    public void linkPSR(ProcessStatusRegister psr)
    {
        PSR = psr;

        //Turn on the decimal flag since this ALU works in base 10.
        PSR.setFlag(
            ProcessStatusRegister.FLAGS.DECIMAL_MODE,
            System.Convert.ToBoolean(1)
        );
    }

    /**
     * @brief Allocates a set of input fields to the ALU.
     * @param input_x - The input field to be binded to ALU x.
     * @param input_y - The input field to be binded to ALU y.
     * @param input_z - The input field to be binded to ALU z.
     * @param circuitry - A graphical representation of the ALU's circuitry.
     */
    public void allocatedInputFields(
        InputField xInput,
        InputField yInput,
        InputField zInput,
        Text circuitry)
    {
        input_x = xInput;
        input_y = yInput;
        input_z = zInput;
        circuitryGraphic = circuitry;

        //Delagate input validation on ALU input fields.
        input_x.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input_y.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input_z.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.validateAsHex(addedChar); };
        input_x.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input_x);
            writeX(input_x.text);
        });
        input_y.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input_y);
            writeY(input_y.text);
        });
        input_z.onEndEdit.AddListener(delegate {
            InputValidation.fillBlanks_Register(input_z);
        });

        //Wipe the slate clean after fields have been altered.
        this.reset();
    }

    /**
     * @brief Resets the ALU back to it's starting state.
     */
    public void reset()
    {
        this.setCircuitry(defualtCircuitry);
        x = STARTING_CONTENT;
        y = STARTING_CONTENT;
        z = STARTING_CONTENT;
        input_x.text = InputValidation.fillBlanks((STARTING_CONTENT).ToString("X"), 4);
        input_y.text = InputValidation.fillBlanks((STARTING_CONTENT).ToString("X"), 4);
        input_z.text = InputValidation.fillBlanks((STARTING_CONTENT).ToString("X"), 4);
    }

    /**
     * @brief Sets the content of ALU x.
     * @param content - New content for ALU x.
     */
    public void writeX(string content)
    {
        //Convert the input from a hex string to decimal format.
        x = ushort.Parse(
            InputValidation.fillBlanks(content, 4),
            System.Globalization.NumberStyles.HexNumber
        );

        Debug.Log("ALUx: write - " + x);
    }

    /**
     * @brief Retrieves the content held in ALU x.
     * @return Returns the contnent held in ALU x.
     */
    public string readX()
    {
        Debug.Log("ALUx: read - " + x);

        //Convert the decimal value into a hex string and return it.
        return InputValidation.fillBlanks(x.ToString("X"), 4);
    }

    /**
     * @brief Sets the content of ALU y.
     * @param content - New content for ALU y.
     */
    public void writeY(string content)
    {
        //Convert the input from a hex string to decimal format.
        y = ushort.Parse(
            InputValidation.fillBlanks(content, 4),
            System.Globalization.NumberStyles.HexNumber
        );

        Debug.Log("ALUy: write - " + y);
    }

    /**
     * @brief Retrives the content held in ALU y.
     * @return Returns the content held in ALU y.
     */
    public string readY()
    {
        Debug.Log("ALUy: read - " + y);

        //Convert the decimal value into a hex string and return it.
        return InputValidation.fillBlanks(y.ToString("X"), 4);
    }

    /**
     * @brief Retrieves the content held in ALU z.
     * @return Returns the content held in ALU z.
     */
    public string readZ()
    {
        Debug.Log("ALUz: read - " + z);

        //Convert the decimal value into a hex string and return it.
        return InputValidation.fillBlanks(z.ToString("X"), 4);
    }

    /**
     * @brief Used X and Y to compute the value of Z.
     */
    public void computeZ()
    {
        System.Int32 result = 0x0000;

        //Perform a different operation depending on the ALU's circuitry.
        switch (currentCircuitry)
        {
            case CIRCUITRY.ADDITION:
                {
                    result = x + y;
                    break;
                }
            case CIRCUITRY.DIVISION:
                {
                    result = x / y;
                    break;
                }
            case CIRCUITRY.MULTIPLICATION:
                {
                    result = x * y;
                    break;
                }
            case CIRCUITRY.SUBTRACTION:
                {
                    result = x - y;
                    break;
                }
        }

        Debug.Log("COMPUTING result: " + result.ToString("X"));

        //Update the PSR.
        setPSR(result);
        z = (ushort)result;
    }

    /**
     * @brief Checks the state of 'Z' and sets the ALU accordingly.
     */
    private void setPSR(System.Int32 value)
    {
        //If 'z' is equal to zero (Zero flag).
        if (value == 0)
        {
            PSR.setFlag(ProcessStatusRegister.FLAGS.ZERO, System.Convert.ToBoolean(1));
        }
        else
        {
            PSR.setFlag(ProcessStatusRegister.FLAGS.ZERO, System.Convert.ToBoolean(0));
        }


        //If 'z' exceeds the signed value range (Overflow flag).
        if (value > MAX_VALUE || value < (MAX_VALUE * -1))
        {
            PSR.setFlag(ProcessStatusRegister.FLAGS.OVERFLOW, System.Convert.ToBoolean(1));
        }
        else
        {
            PSR.setFlag(ProcessStatusRegister.FLAGS.OVERFLOW, System.Convert.ToBoolean(0));
        }

        //If 'z' is negative (Negative flag).
        if (value < 0)
        {
            PSR.setFlag(ProcessStatusRegister.FLAGS.NEGATIVE, System.Convert.ToBoolean(1));
        }
        else
        {
            PSR.setFlag(ProcessStatusRegister.FLAGS.NEGATIVE, System.Convert.ToBoolean(0));
        }
    }

    /**
     * @brief Allows the ALU's circuitry to be set.
     * @param newCircuitry - This is the new circuitry for the ALU.
     */
    public void setCircuitry(CIRCUITRY newCircuitry)
    {
        //Update the circuitry.
        switch (newCircuitry)
        {
            case CIRCUITRY.ADDITION:
                {
                    setAdditionCircuitry();
                    break;
                }
            case CIRCUITRY.DIVISION:
                {
                    setDivisionCircuitry();
                    break;
                }
            case CIRCUITRY.MULTIPLICATION:
                {
                    setMultiplicationCircuitry();
                    break;
                }
            case CIRCUITRY.SUBTRACTION:
                {
                    setSubtractionCircuitry();
                    break;
                }
        }
    }

    /**
     * @Sets the ALU's circuitry to addition.
     */
    public void setAdditionCircuitry()
    {
        Debug.Log("Setting ALU to Addition mode.");
        currentCircuitry = CIRCUITRY.ADDITION;
        circuitryGraphic.text = "+";
    }

    /**
     * @Sets the ALU's circuitry to division.
     */
    public void setDivisionCircuitry()
    {
        Debug.Log("Setting ALU to Division mode.");
        currentCircuitry = CIRCUITRY.DIVISION;
        circuitryGraphic.text = "÷";
    }

    /**
     * @Sets the ALU's circuitry to multiplication.
     */
    public void setMultiplicationCircuitry()
    {
        Debug.Log("Setting ALU to Multiplication mode.");
        currentCircuitry = CIRCUITRY.MULTIPLICATION;
        circuitryGraphic.text = "×";
    }

    /**
     * @Sets the ALU's circuitry to subtraction.
     */
    public void setSubtractionCircuitry()
    {
        Debug.Log("Setting ALU to Subtraction mode.");
        currentCircuitry = CIRCUITRY.SUBTRACTION;
        circuitryGraphic.text = "-";
    }
}
