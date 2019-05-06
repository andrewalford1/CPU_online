using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Abstract class representing a bus.
 * @extends SimulatorComponent
 * @author Andrew Alford
 * @date 14/02/2019
 * @version 1.1 - 15/02/2019
 */
public abstract class Bus : SimulatorComponent
{
    /**
     * @brief Initialises the bus.
     * @param inputRegisterUI - The input register for the bus.
     * @param outputRegisterUI - The output register for the bus.
     */
    public abstract void initialise(
        InputField inputRegisterUI, 
        InputField outputRegisterUI);

    /**
     * @brief Returns 'true' if this bus can transfer
     *                data in both directions. 
     *                (Otherwise 'false' is returned).
     */
    public abstract bool isBidirectional();

    /**
     * @brief Transfers data from one register to another.
     */
    public abstract void transferData();
}
