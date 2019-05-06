using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

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
    [SerializeField] private GeneralPurposeRegister GP_A = null;
    [SerializeField] private ProcessStatusRegister PSR = null;

    //REGISTER INPUTS
    [SerializeField] private InputField PC_input = null;
    [SerializeField] private InputField MAR_input = null;
    [SerializeField] private InputField MDR_input = null;
    [SerializeField] private InputField IR_input = null;
    [SerializeField] private InputField GP_A_input = null;
    [SerializeField] private InputField PSR_input = null;

    //MEMORY
    [SerializeField] private MemoryListControl memory = null;

    //ALU
    [SerializeField] private ArithmeticLogicUnit ALU = null;

    //ALU INPUTS
    [SerializeField] private InputField ALUx_input = null;
    [SerializeField] private InputField ALUy_input = null;
    [SerializeField] private InputField ALUz_input = null;
    [SerializeField] private Text ALU_circuitry = null;

    //BUTTONS
    [SerializeField] private Button reset = null;

    /**
     * @brief Initialise the simulator.
     */
    void Start()
    {
        //Assign input fields to registers.
        PC.AllocateInputField(PC_input);
        MAR.AllocateInputField(MAR_input);
        MDR.AllocateInputField(MDR_input);
        IR.AllocateInputField(IR_input);
        GP_A.AllocateInputField(GP_A_input);
        PSR.AllocateInputField(PSR_input);

        //Assign input fields to the ALU.
        ALU.AllocatedInputFields(ALUx_input, ALUy_input, ALUz_input, ALU_circuitry);
        ALU.LinkPSR(PSR);

        //Add listener to the 'reset' button.
        reset.onClick.AddListener(delegate
        {
            //Empty memory.
            memory.EmptyMemory();
            //Reset registers.
            PC.Reset();
            MAR.Reset();
            MDR.Reset();
            IR.Reset();
            GP_A.Reset();
            PSR.Reset();
            //Reset the ALU.
            ALU.Reset();

            ConsoleControl.CONSOLE.logMessage("Reset");
        });
    }

    /**
     * @brief Perform a fetch on the CPU.
     */
    public void Fetch()
    {
        ConsoleControl.CONSOLE.logMessage("Performing CPU Fetch");

        //Check the content of the PC.
        PC.Write(PC_input.text);

        //Write the content of PC to MAR.
        MAR.Write(PC.ReadString());
        MAR_input.text = MAR.ReadString();

        //Increment the PC.
        PC.Increment();
        PC_input.text = PC.ReadString();

        //Set the memory pointer to the value of the MAR.
        memory.SetPointer(MAR.ReadInt());

        //Write the contents of the memory address being pointed to into MDR.
        MDR.Write(memory.ReadFromMemorySlot());
        MDR_input.text = MDR.ReadString();

        //Copy the contents of the MDR to the instruction register.
        IR.Write(MDR.ReadString());
        IR_input.text = IR.ReadString();
    }

    public void Execute()
    {
        ConsoleControl.CONSOLE.logMessage("Performing CPU Execute");

        //Copy the contents of IR into ALUx.
        ALU.WriteX(IR.ReadString());
        ALUx_input.text = IR.ReadString();

        //Compute ALUz.
        ALU.ComputeZ();
        ALUz_input.text = ALU.ReadZ();

        //Copy the contents of ALUz into GP A.
        GP_A.Write(ALU.ReadZ());
        GP_A_input.text = GP_A.ReadString();
    }
}