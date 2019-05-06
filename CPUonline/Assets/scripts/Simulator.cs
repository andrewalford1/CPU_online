using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class to manage the behaviour of the simulator.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 1.2 - 14/02/2019
 */
public class Simulator : MonoBehaviour
{
    //[registers] Holds all of the general purpose registers on the CPU.
    [SerializeField] private GeneralPurposeRegister[] registers = { };
    [SerializeField] private InputField[] registerInputs = { };
    [SerializeField] private AddressBus addressBus = null;
    [SerializeField] private DataBus dataBus = null;
    [SerializeField] private ControlBus controlBus = null;

    /**
     * @brief Empties the contents of  all the registers on the CPU.
     */
    public void emptyRegisters()
    {
        //Loop through all registers.
        for(int i = 0; i < registers.Length; i++)
        {
            //Set each register to it's defualt value.
            registers[i].write(registers[i].getStartingContent());
        }

        Debug.Log("Reset");
    }

    // Start is called before the first frame update
    void Start()
    {      
        addressBus.initialise(registerInputs[0], registerInputs[1]);
        dataBus.initialise(registerInputs[2], registerInputs[3]);
        controlBus.initialise(registerInputs[4], registerInputs[5]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
