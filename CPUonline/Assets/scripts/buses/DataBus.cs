using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Abstract class representing a Data Bus.
 * @extends Bus
 * @author Andrew Alford
 * @date 15/02/2019
 * @version 1.0 - 15/02/2019
 */
[CreateAssetMenu(menuName ="Data_Bus")]
public class DataBus : Bus
{
    //[registerA] The first register this bus is connected to.
    [SerializeField] private Register registerA = null;

    //[registerB] The second register this bus is connected to.
    [SerializeField] private Register registerB = null;

    //[registerA_UI] The input field for register A.
    private InputField registerA_UI = null;

    //[registerB_UI] The input field for register B.
    private InputField registerB_UI = null;

    //[direction] Defines the way in which data is transferred.
    //If 'true', data flows from register A to register B.
    //If 'false', data flows from register B to register A.
    [SerializeField] private bool direction = true;

    /**
     * @brief Initialises the databus.
     *        Note: As this bus is bidirectional it I/O doesn't matter.
     * @param inputRegisterUI - A UI element to display register content.
     * @param outputRegisterUI - A UI element to display register content.
     */
    public override void initialise(
        InputField inputRegisterUI, 
        InputField outputRegisterUI)
    {
        registerA_UI = inputRegisterUI;
        registerB_UI = outputRegisterUI;
    }

    /**
     * @brief Checks if the bus is bidirectional.
     * @return Returns 'true' as the data bus is bidirectional.
     */
    public override bool isBidirectional()
    {
        return true;
    }

    /**
     * @brief Transfers data from one register to another.
     */
    public override void transferData()
    {
        if (direction)
        {
            Debug.Log("A to B");
            //Transfer data from A to B.
            registerB.write(registerA.read());
            registerB_UI.text = registerA.read();
        }
        else
        {
            Debug.Log("B to A");
            //Transfer data from B to A.
            registerA.write(registerB.read());
            registerA_UI.text = registerB.read();
        }

    }

    /**
     * @brief Allows the direction of data flow to be set.
     * @param direction - The direction in which data flows.
     *                    If 'true', data flows from 
     *                    register A to register B.
     *                    If 'false', data flows from 
     *                    register B to register A.
     */
    public void setDirection(bool direction)
    {
        this.direction = direction;
    }

    /**
     * @brief Toogle the direction of data flow.
     */
    public void toggleDirection()
    {
        this.direction = !this.direction;
    }

    /**
     * @brief Retrieves one of the two registers connected to this bus.
     * @return Returns one of the two registers connected to this bus.
     */
    public Register getRegisterA()
    {
        return this.registerA;
    }

    /**
     * @brief Retrieves one of the two registers connected to this bus.
     * @return Returns one of the two registers connected to this bus.
     */
    public Register getRegisterB()
    {
        return this.registerB;
    }
}
