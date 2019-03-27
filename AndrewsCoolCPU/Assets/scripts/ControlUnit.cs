using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

/**
 * @brief Class representing the simulators control unit.
 * @extends MonoBehaviour
 * @author Andrew Alford
 * @date 20/03/2019
 * @version 1.1 - 21/03/2019
 */
public class ControlUnit : MonoBehaviour
{
    //CU
    [SerializeField] private InputField currentCommandDisplay = null;
    //REGISTERS
    [SerializeField] private ProgramCounter         PC      = null;
    [SerializeField] private MemoryAddressRegister  MAR     = null;
    [SerializeField] private MemoryDataRegister     MDR     = null;
    [SerializeField] private InstructionRegister    IR      = null;
    [SerializeField] private GeneralPurposeRegister GP_A    = null;
    [SerializeField] private GeneralPurposeRegister GP_B    = null;
    //MEMORY
    [SerializeField] private MemoryListControl      memory  = null;
    //ALU
    [SerializeField] private ArithmeticLogicUnit    ALU     = null;

    //[instructions] Holds all the instructions that the CU can perform.
    private List<Instruction> instructions = new List<Instruction>();
    private Instruction currentInstruction = null;

    /**
     * @brief Inialise the Control Unit.
     */
    IEnumerator Start()
    {
        string url = Application.streamingAssetsPath + "/instruction_set.json";
        string json;

        //Check if we should use UnityWebRequest or File.ReadAllBytes
        if (url.Contains("://") || url.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            json = www.downloadHandler.text;
        }
        else
        {
            json = File.ReadAllText(url);
        }

        foreach (Instruction instruction in JsonUtility
            .FromJson<InstructionSet>(json).instructions)
        {
            instructions.Add(instruction);
        }

        //Set up the first instruction in the CU.
        Decode();
    }

    private bool currentlyProcessing = false;

    IEnumerator CPUfetch() {
        currentlyProcessing = true;
        yield return new WaitForSeconds(1);
        Debug.Log("from ienumerator");
        yield return new WaitForSeconds(1);
        //Send the address stored in PC to the MAR for fetching.
        MAR.Write(PC.ReadString());
        yield return new WaitForSeconds(1);
        //Increment the PC.
        PC.Increment();
        yield return new WaitForSeconds(1);
        //Set the memory pointer to the value of MAR.
        memory.SetPointer(MAR.ReadUnsigned());
        yield return new WaitForSeconds(1);
        //Write the contents of the memory address being pointed to into MDR.
        MDR.Write(memory.ReadFromMemorySlot());
        currentlyProcessing = false;
    }

    /**
     * @brief Performs a CPU fetch.
     */
    public void Fetch()
    {
        ////Send the address stored in PC to the MAR for fetching.
        //MAR.Write(PC.ReadString());

        ////Increment the PC.
        //PC.Increment();

        ////Set the memory pointer to the value of MAR.
        //memory.SetPointer(MAR.ReadUnsigned());

        ////Write the contents of the memory address being pointed to into MDR.
        //MDR.Write(memory.ReadFromMemorySlot());

        if(!currentlyProcessing) {
            StartCoroutine(CPUfetch());
        } else {
            Debug.Log("Sorry John, I cannont do that");
        }
    }

    public void Decode()
    {
        //Copy the contents of the MDR into the IR for decoding.
        IR.Write(MDR.ReadString());

        SetCurrentInstruction(IR.Opcode());
    }

    /**
     * @brief Executes the current instruction on the CU.
     */
    public void Execute()
    {
        if(currentInstruction != null)
        {
            //Execute the current instruction.
            switch(currentInstruction.ID)
            {
                case (0): Add(GP_A); break;
                case (1): Add(GP_B); break;
                case (2): Add(GP_A, GP_B); break;
                case (3): Add(GP_B, GP_A); break;
            }
        }
    }

    /**
     * @brief Resets everything on the simulator.
     */
    public void Reset()
    {
        //Reset CU
        currentInstruction = null;
        currentCommandDisplay.text = "\0";
        //Empty memory
        memory.EmptyMemory();
        //Reset registers
        PC.Reset();
        MAR.Reset();
        MDR.Reset();
        IR.Reset();
        GP_A.Reset();
        GP_B.Reset();
        //Reset the ALU
        ALU.Reset();
        ConsoleControl.CONSOLE.LogMessage("CPU Reset");
        //Set up the first instruction in the CU.
        Decode();
    }

    //COMMANDS

    public Instruction GetInstruction(string command)
    {
        foreach(Instruction instruction in instructions)
        {
            if(instruction.command.Equals(command))
            {
                return instruction;
            }
        }
        return null;
    }

    public void SetCurrentInstruction(int id)
    {
        //Only change the current instruction if the ID is in bounds.
        if(id >= 0 && id < instructions.Count)
        {
            currentInstruction = instructions[id];
            currentCommandDisplay.text = currentInstruction.command.Substring(1);
        }
    }

    /**
     * @brief Adds the data held in IR to a given register.
     * @param x - The register to be added to.
     */
    private void Add(Register x)
    {
        //Copy the contents of register 'x' into ALUx.
        ALU.WriteX(x.ReadString());

        //Copy the Instruction Registers operand into ALUy.
        ALU.WriteY(IR.OperandString());

        //Set the ALU's circuity.
        ALU.SetAdditionCircuitry();

        //Compute ALUz.
        ALU.ComputeZ();

        //Store the result into register x.
        x.Write(ALU.ReadZ());
    }

    private void Add(Register x, Register y)
    {
        //Copy the contents of register 'x' into ALUx.
        ALU.WriteX(x.ReadString());

        //Copy the contents of register 'y' into ALUy.
        ALU.WriteY(y.ReadString());

        //Set the ALU's circuity.
        ALU.SetAdditionCircuitry();

        //Compute ALUz.
        ALU.ComputeZ();

        //Store the result into register x.
        x.Write(ALU.ReadZ());
    }
}
