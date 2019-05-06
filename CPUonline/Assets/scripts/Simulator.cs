using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief Class to manage the behaviour of the simulator.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 12/02/2019
 * @version 2.0 - 03/03/2019
 */
public class Simulator : MonoBehaviour
{
    //REGISTERS
    [SerializeField] private ProgramCounter PC = null;
    [SerializeField] private MemoryAddressRegister MAR = null;
    [SerializeField] private MemoryDataRegister MDR = null;
    [SerializeField] private InstructionRegister IR = null;

    //REGISTER INPUTS
    [SerializeField] private InputField PC_input = null;
    [SerializeField] private InputField MAR_input = null;
    [SerializeField] private InputField MDR_input = null;
    [SerializeField] private InputField IR_input = null;

    //MEMORY
    [SerializeField] private GameObject memoryList = null;
    private MemoryListControl memory = null;

    //BUTTONS
    [SerializeField] private Button reset = null;

    /**
     * @brief Initialise the simulator.
     */
    void Start()
    {
        //Link memory to the simulator.
        memory = memoryList.GetComponentsInChildren<MemoryListControl>()[0];

        //Assign input fields to registers.
        PC.allocateInputField(PC_input);
        MAR.allocateInputField(MAR_input);
        MDR.allocateInputField(MDR_input);
        IR.allocateInputField(IR_input);

        //Add listener to the 'reset' button.
        reset.onClick.AddListener(delegate
        {
            //Empty memory.
            memory.emptyMemory();
            //Reset registers.
            PC.reset();
            MAR.reset();
            MDR.reset();
            IR.reset();

            Debug.Log("Simulator Reset");
        });
    }

    /**
     * @brief Perform a fetch on the CPU.
     */
    public void fetch()
    {
        Debug.Log("Performing CPU Fetch");

        //Check the content of the PC.
        PC.write(PC_input.text);

        //Write the content of PC to MAR.
        MAR.write(PC.read());
        MAR_input.text = MAR.read();

        //Increment the PC.
        PC.increment();
        PC_input.text = PC.read();

        //Set the memory pointer to the value of the MAR.
        memory.setPointer(MAR.readAsDecimalInt());

        //Write the contents of the memory address being pointed to into MDR.
        MDR.write(memory.readFromMemorySlot());
        MDR_input.text = MDR.read();

        //Copy the contents of the MDR to the instruction register.
        IR.write(MDR.read());
        IR_input.text = IR.read();
    }
}