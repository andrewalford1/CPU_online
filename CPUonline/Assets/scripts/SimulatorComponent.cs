using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief Abstract class representing a simulator component.
 * @extends ScriptableObject
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 1.0 - 12/02/2019
 */
public abstract class SimulatorComponent : ScriptableObject
{
    //[id] The ID of the simulator component.
    [SerializeField] private string id = "Override Me";

    /**
     * @brief Retrieves the ID of the register.
     * @return Returns the registers ID.
     */
    public string getID()
    {
        return id;
    }
}
