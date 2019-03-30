using UnityEngine.UI;

/**
 * @brief   Class representing the program counter.
 * @extends Register
 * @author  Andrew Alford
 * @date    28/02/2019
 * @version 1.3 - 29/03/2019
 */
public class ProgramCounter : Register
{
    public override BusControl.BUS_ROUTE RouteToPC      => BusControl.BUS_ROUTE.NONE;
    public override BusControl.BUS_ROUTE RouteToMAR     => BusControl.BUS_ROUTE.PC_MAR;
    public override BusControl.BUS_ROUTE RouteToMDR     => BusControl.BUS_ROUTE.PC_MDR;
    public override BusControl.BUS_ROUTE RouteToCU      => BusControl.BUS_ROUTE.PC_CU;
    public override BusControl.BUS_ROUTE RouteToIR      => BusControl.BUS_ROUTE.MDR_IR;
    public override BusControl.BUS_ROUTE RouteToGPA     => BusControl.BUS_ROUTE.PC_GPA;
    public override BusControl.BUS_ROUTE RouteToGPB     => BusControl.BUS_ROUTE.PC_GPB;
    public override BusControl.BUS_ROUTE RouteToALUx    => BusControl.BUS_ROUTE.PC_ALUX;
    public override BusControl.BUS_ROUTE RouteToALUy    => BusControl.BUS_ROUTE.PC_ALUY;
    public override BusControl.BUS_ROUTE RouteToALUz    => BusControl.BUS_ROUTE.PC_ALUZ;

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
        { return InputValidation.ValidateAsHex(addedChar); };
        input.onEndEdit.AddListener(delegate {
            InputValidation.FillBlanks_Register(input);
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
