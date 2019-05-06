using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Abstract class representing the Control Bus.
 * @extends Bus
 * @author Andrew Alford
 * @date 19/02/2019
 * @version 1.0 - 19/02/2019
 */
[CreateAssetMenu(menuName = "Control_Bus")]
public class ControlBus : Bus
{
    //[input] The register the bus recieves data from.
    [SerializeField] private Register input = null;
    //[output] Thre register the bus sends data to.
    [SerializeField] private Register output = null;

    //[inputUI] An input field to show the operations of this bus.
    private InputField inputUI = null;

    //[outputUI] An output field to show the operations of this bus.
    private InputField outputUI = null;

    /**
     * @brief Initialises the bus.
     * @param inputRegisterUI - The UI element bus input comes from.
     * @param outputRegisterUI - The UI element bus output comes from.
     */
    public override void initialise(
    InputField inputRegisterUI,
    InputField outputRegisterUI)
    {
        inputUI = inputRegisterUI;
        outputUI = outputRegisterUI;
    }

    /**
     * @brief Checks if the bus is bidirectional.
     * @return Returns 'false' as the bus is not bidirectional.
     */
    public override bool isBidirectional()
    {
        return false;
    }

    /**
     * @brief Transfers data from one register to another.

     */
    public override void transferData()
    {
        output.write(input.read());
        outputUI.text = input.read();
    }

    /**
     * @brief Retrieves the input register connected to this bus.
     * @return Returns the input register.
     */
    public Register getInputRegister()
    {
        return this.input;
    }

    /**
     * @brief Retrieves the output register connected to this bus.
     * @return Returns the output register.
     */
    public Register getOutputRegister()
    {
        return this.output;
    }
}
