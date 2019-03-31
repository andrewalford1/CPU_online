using UnityEngine;
using UnityEngine.UI;

/**
 * @brief   Class representing the Arithmetic Logic Unit.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    03/03/2019
 * @version 1.4 - 27/03/2019
 */
public class ArithmeticLogicUnit : MonoBehaviour {
    //[x] Stores the value of ALUx (parameter A).
    private Number x = new Number();
    //[y] Stores the value of ALUy (Parameter B).
    private Number y = new Number();
    //[z] Stores the value of ALUz (Calculation using A and B).
    private Number z = new Number();

    //[active] While 'true' the ALU can be interacted with.
    protected bool active = true;

    //[CIRCUITRY] Defines the different 
    //types of circuitry for the ALU.
    public enum CIRCUITRY {
        ADDITION,
        SUBTRACTION,
        MULTIPLICATION,
        DIVISION
    }

    //[defualtCircuitry] The defualt circuitry used by the ALU.
    private readonly CIRCUITRY defualtCircuitry = CIRCUITRY.ADDITION;
    //[currentCircuitry] The circuitry currently 
    //being used by the ALU.
    private CIRCUITRY currentCircuitry = CIRCUITRY.ADDITION;
    //[circuitryGraphic] A graphical representation of the ALU's circuitry.
    [SerializeField] private Text circuitryGraphic = null;

    //[input_x] An input field for users to interact with ALU x.
    [SerializeField] private InputField input_x = null;
    //[input_y] An input field for users to interact with ALU y.
    [SerializeField] private InputField input_y = null;
    //[input_z] An input field for users to interact with ALU z.
    [SerializeField] private InputField input_z = null;

    //[PSR] The Process Status Register, used to show the states of ALU calculations.
    [SerializeField] private ProcessStatusRegister PSR = null;

    /**
     * @brief Initialilse the ALU.
     */
    public void Start() {
        //Delagate input validation on ALU input fields.
        input_x.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.ValidateAsHex(addedChar); };
        input_y.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.ValidateAsHex(addedChar); };
        input_z.onValidateInput += delegate (string input, int charIndex, char addedChar)
        { return InputValidation.ValidateAsHex(addedChar); };
        input_x.onEndEdit.AddListener(delegate {
            InputValidation.FillBlanks_Register(input_x);
            WriteX(input_x.text);
        });
        input_y.onEndEdit.AddListener(delegate {
            InputValidation.FillBlanks_Register(input_y);
            WriteY(input_y.text);
        });
        input_z.onEndEdit.AddListener(delegate {
            InputValidation.FillBlanks_Register(input_z);
        });

        //Wipe the slate clean after fields have been altered.
        Reset();
    }

    /**
     * @brief Resets the ALU back to it's starting state.
     */
    public void Reset() {
        SetCircuitry(defualtCircuitry);
        x.Reset();
        y.Reset();
        z.Reset();
        input_x.text = x.GetHex();
        input_y.text = y.GetHex();
        input_z.text = z.GetHex();
        PSR.Reset();
    }

    /**
     * @brief Sets the content of ALU x.
     * @param content - New content for ALU x.
     */
    public void WriteX(string content) {
        x.SetNumber(content);
        input_x.text = x.GetHex();
        Debug.Log("ALUx: write - " + x.ToString());
    }

    /**
     * @returns The contnent held in ALU x.
     */
    public string ReadX() {
        Debug.Log("ALUx: read - " + x.ToString());
        return x.GetHex();
    }

    /**
     * @brief Sets the content of ALU y.
     * @param content - New content for ALU y.
     */
    public void WriteY(string content) {
        y.SetNumber(content);
        input_y.text = y.GetHex();
        Debug.Log("ALUy: write - " + y.ToString());
    }

    /**
     * @returns The content held in ALU y.
     */
    public string ReadY() {
        Debug.Log("ALUy: read - " + y.ToString());
        return y.GetHex();
    }

    /**
     * @returns The content held in ALU z.
     */
    public string ReadZ() {
        Debug.Log("ALUz: read - \n" +
            "Decimal:\t" + z.GetSigned() + "\n" +
            "Hex:\t" + z.ToString()
        );
        Debug.Log("ALUz: read - " + z.ToString());
        return z.GetHex();
    }

    /**
     * @brief Used X and Y to compute the value of Z.
     */
    public void ComputeZ() {
        //Perform a different operation depending on the ALU's circuitry.
        switch (currentCircuitry) {
            case CIRCUITRY.ADDITION:
                z = Number.Add(x, y);
                break;
            case CIRCUITRY.SUBTRACTION:
                z = Number.Subtract(x, y);
                break;
            case CIRCUITRY.MULTIPLICATION:
                z = Number.Multiply(x, y);
                break;
            case CIRCUITRY.DIVISION:
                z = Number.Divide(x, y);
                break;
        }

        Debug.Log("COMPUTING Z: - \n" + z.ToString());

        input_z.text = z.GetHex();
    }

    /**
     * @brief Upates the PSR with the value of ALU z.
     */
    public void SetPSR() {
        z.SetPSR(PSR);
    }

    /**
     * @brief Allows the ALU's circuitry to be set.
     * @param newCircuitry - This is the new circuitry for the ALU.
     */
    public void SetCircuitry(CIRCUITRY newCircuitry) {
        //Update the circuitry.
        switch (newCircuitry) {
            case CIRCUITRY.ADDITION:
                SetAdditionCircuitry();
                break;
            case CIRCUITRY.DIVISION:
                SetDivisionCircuitry();
                break;
            case CIRCUITRY.MULTIPLICATION:
                SetMultiplicationCircuitry();
                break;
            case CIRCUITRY.SUBTRACTION:
                SetSubtractionCircuitry();
                break;
        }
    }

    /**
     * @Sets the ALU's circuitry to addition.
     */
    public void SetAdditionCircuitry() {
        Debug.Log("Setting ALU to Addition mode.");
        currentCircuitry = CIRCUITRY.ADDITION;
        circuitryGraphic.text = "+";
    }

    /**
     * @Sets the ALU's circuitry to division.
     */
    public void SetDivisionCircuitry() {
        Debug.Log("Setting ALU to Division mode.");
        currentCircuitry = CIRCUITRY.DIVISION;
        circuitryGraphic.text = "÷";
    }

    /**
     * @Sets the ALU's circuitry to multiplication.
     */
    public void SetMultiplicationCircuitry() {
        Debug.Log("Setting ALU to Multiplication mode.");
        currentCircuitry = CIRCUITRY.MULTIPLICATION;
        circuitryGraphic.text = "×";
    }

    /**
     * @Sets the ALU's circuitry to subtraction.
     */
    public void SetSubtractionCircuitry() {
        Debug.Log("Setting ALU to Subtraction mode.");
        currentCircuitry = CIRCUITRY.SUBTRACTION;
        circuitryGraphic.text = "-";
    }

    /**
     * @brief Toggles interaction with the component's UI.
     * @param active - If 'true' then the ALU cannnot 
     *                 be manually manipulated.
     */
    public void SetActive(bool active)
    {
        this.active = active;
        input_x.interactable = active;
        input_y.interactable = active;
        input_z.interactable = active;
    }

    /**
     * @returns 'true' if the component is currently active.
     */
    public bool IsActive()
    {
        return active;
    }
}
