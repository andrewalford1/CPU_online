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
 * @version 1.2 - 27/03/2019
 */
public class ControlUnit : MonoBehaviour
{
    //CU
    [SerializeField] private InputField             currentCommandDisplay   = null;
    //REGISTERS
    [SerializeField] private ProgramCounter         PC                      = null;
    [SerializeField] private MemoryAddressRegister  MAR                     = null;
    [SerializeField] private MemoryDataRegister     MDR                     = null;
    [SerializeField] private InstructionRegister    IR                      = null;
    [SerializeField] private GeneralPurposeRegister GP_A                    = null;
    [SerializeField] private GeneralPurposeRegister GP_B                    = null;
    //MEMORY
    [SerializeField] private MemoryListControl      memory                  = null;
    //ALU
    [SerializeField] private ArithmeticLogicUnit    ALU                     = null;
    //CLOCK
    [SerializeField] private Clock                  clock                   = null;
    //BUTTONS
    [SerializeField] private Button                 fetch_btn               = null;
    [SerializeField] private Button                 decode_btn              = null;
    [SerializeField] private Button                 execute_btn             = null;
    [SerializeField] private Button                 reset_btn               = null;

    //[instructions] Holds all the instructions that the CU can perform.
    private List<Instruction> instructions = new List<Instruction>();
    private Instruction currentInstruction = null;

    //[currentlyProecssing] 'True' whilst the CU is in use. 
    //(Prevents multiple operations occurring at once.
    private bool currentlyProcessing = false;

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

        //Delagate listeners to buttons.
        fetch_btn.onClick.AddListener(  delegate {  Fetch();     });
        decode_btn.onClick.AddListener( delegate {  Decode();    });
        execute_btn.onClick.AddListener(delegate {  Execute();   });
        reset_btn.onClick.AddListener(  delegate {  Reset();     });

        //Set up the first instruction in the CU.
        SetCurrentInstructionFromCU(0);
    }

    /**
     * @brief Updates the CU once every frame.
     */
    private void Update() {
        fetch_btn.interactable      = !currentlyProcessing;
        decode_btn.interactable     = !currentlyProcessing;
        execute_btn.interactable    = !currentlyProcessing;
        reset_btn.interactable      = !currentlyProcessing;
        PC.SetActive(!currentlyProcessing);
        MAR.SetActive(!currentlyProcessing);
        MDR.SetActive(!currentlyProcessing);
        IR.SetActive(!currentlyProcessing);
        GP_A.SetActive(!currentlyProcessing);
        GP_B.SetActive(!currentlyProcessing);
        ALU.SetActive(!currentlyProcessing);
        if (memory.IsActive() && currentlyProcessing) {
            memory.SetActive(false);
        } else if(!memory.IsActive() && !currentlyProcessing) {
            memory.SetActive(true);
        }
    }

    public bool IsCurrentlyProcessing() {
        return currentlyProcessing;
    }

    /**
     * @brief A coroutine to perform a CPU fetch 
     *        at the desired clockspeed.
     */
    IEnumerator FetchCoroutine() {
        currentlyProcessing = true;
        yield return new WaitForSeconds(clock.GetSpeed());
        //Send the address stored in PC to the MAR for fetching.
        MAR.Write(PC.ReadString());
        yield return new WaitForSeconds(clock.GetSpeed());
        //Increment the PC.
        PC.Increment();
        yield return new WaitForSeconds(clock.GetSpeed());
        //Set the memory pointer to the value of MAR.
        memory.SetPointer(MAR.ReadUnsigned());
        yield return new WaitForSeconds(clock.GetSpeed());
        //Write the contents of the memory address being pointed to into MDR.
        MDR.Write(memory.ReadFromMemorySlot());
        currentlyProcessing = false;
    }

    /**
     * @brief Performs a CPU fetch.
     */
    public void Fetch()
    {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform fetch as CPU is currently processing.");
        } else {
            StartCoroutine(FetchCoroutine());
        }
    }

    /**
     * @brief A coroutine to perform a CPU decode 
     *        at the desired clockspeed.
     */
    IEnumerator DecodeCoroutine()
    {
        currentlyProcessing = true;
        yield return new WaitForSeconds(clock.GetSpeed());
        //Copy the contents of the MDR into the IR for decoding.
        IR.Write(MDR.ReadString());
        yield return new WaitForSeconds(clock.GetSpeed());
        SetCurrentInstructionFromCU(IR.Opcode());
        currentlyProcessing = false;
    }

    public void Decode()
    {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform decode as CPU is currently processing");
        } else {
            StartCoroutine(DecodeCoroutine());
        }
    }

    /**
     * @brief Executes the current instruction on the CU.
     */
    public void Execute()
    {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform execute as CPU is currently processing");
        } else {
            if (currentInstruction != null) {
                //Execute the current instruction.
                switch(currentInstruction.ID) {
                    case (0): Add(GP_A); break;
                    case (1): Add(GP_B); break;
                    case (2): Add(GP_A, GP_B); break;
                    case (3): Add(GP_B, GP_A); break;
                }
            }
        }
    }

    /**
     * @brief Resets everything on the simulator.
     */
    public void Reset()
    {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform execute as CPU is currently processing");
        } else {
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
            //Reset the clock
            clock.Reset();
            //Set up the first instruction in the CU.
            SetCurrentInstructionFromCU(0);
            ConsoleControl.CONSOLE.LogMessage("CPU Reset");
        }
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

    private void SetCurrentInstructionFromCU(int id) {
        //Only change the current instruction if the ID is in bounds.
        if (id >= 0 && id < instructions.Count) {
            currentInstruction = instructions[id];
            currentCommandDisplay.text = currentInstruction.command.Substring(1);
        }
    }

    public void SetCurrentInstructionFromConsole(int id)
    {
        if (currentlyProcessing) {
            Debug.Log("Cannot set instruction as CPU is currently processing");
        } else {
            //Only change the current instruction if the ID is in bounds.
            if (id >= 0 && id < instructions.Count) {
                currentInstruction = instructions[id];
                currentCommandDisplay.text = currentInstruction.command.Substring(1);
            }
        }
    }

    /**
     * @brief A coroutine to perform a ADD instruction 
     *        at the desired clockspeed.
     * @param x - The register to be added to.
     */
    IEnumerator InstructionCoroutine_ADD(Register x) {
        currentlyProcessing = true;
        yield return new WaitForSeconds(clock.GetSpeed());
        //Copy the contents of register 'x' into ALUx.
        ALU.WriteX(x.ReadString());
        yield return new WaitForSeconds(clock.GetSpeed());
        //Copy the Instruction Registers operand into ALUy.
        ALU.WriteY(IR.OperandString());
        yield return new WaitForSeconds(clock.GetSpeed());
        //Set the ALU's circuity.
        ALU.SetAdditionCircuitry();
        yield return new WaitForSeconds(clock.GetSpeed());
        //Compute ALUz.
        ALU.ComputeZ();
        yield return new WaitForSeconds(clock.GetSpeed());
        //Store the result into register x.
        x.Write(ALU.ReadZ());
        currentlyProcessing = false;
    }

    /**
     * @brief Adds the data held in IR to a given register.
     * @param x - The register to be added to.
     */
    private void Add(Register x) {
        StartCoroutine(InstructionCoroutine_ADD(x));
    }

    /**
     * @brief A coroutine to perform a ADD instruction 
     *        at the desired clockspeed.
     * @param x - The register to be added to.
     * @param y - The other register to be used.
     */
    IEnumerator InstructionCoroutine_ADD(Register x, Register y) {
        currentlyProcessing = true;
        yield return new WaitForSeconds(clock.GetSpeed());
        //Copy the contents of register 'x' into ALUx.
        ALU.WriteX(x.ReadString());
        yield return new WaitForSeconds(clock.GetSpeed());
        //Copy the contents of register 'y' into ALUy.
        ALU.WriteY(y.ReadString());
        yield return new WaitForSeconds(clock.GetSpeed());
        //Set the ALU's circuity.
        ALU.SetAdditionCircuitry();
        yield return new WaitForSeconds(clock.GetSpeed());
        //Compute ALUz.
        ALU.ComputeZ();
        yield return new WaitForSeconds(clock.GetSpeed());
        //Store the result into register x.
        x.Write(ALU.ReadZ());
        yield return new WaitForSeconds(clock.GetSpeed());
        currentlyProcessing = false;
    }

    private void Add(Register x, Register y) {
        StartCoroutine(InstructionCoroutine_ADD(x, y));
    }
}
