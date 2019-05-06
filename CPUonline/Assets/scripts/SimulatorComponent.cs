using UnityEngine;

/**
 * @brief   Abstract class representing a simulator component.
 * @extends ScriptableObject
 * @author  Andrew Alford
 * @date    12/02/2019
 * @version 1.0 - 12/02/2019
 */
public class SimulatorComponent : MonoBehaviour
{
    //[id] The ID of the simulator component.
    protected string id = "Override Me";

    /**
     * @brief Retrieves the ID of the register.
     * @return Returns the registers ID.
     */
    public string GetID() {
        return id;
    }
}
