using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

/**
 * @brief   Class representing the simulators control unit.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    20/03/2019
 * @version 1.4 - 29/03/2019
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
    //BUSES
    [SerializeField] private BusControl             buses                   = null;
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
        fetch_btn.interactable = !currentlyProcessing;
        decode_btn.interactable = !currentlyProcessing;
        execute_btn.interactable = !currentlyProcessing;
        reset_btn.interactable = !currentlyProcessing;
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

        //Send the address stored in PC to the MAR for fetching.
        buses.StartTransferringData(BusControl.BUS_ROUTE.PC_MAR);
        yield return new WaitForSeconds(clock.GetSpeed());
        MAR.Write(PC.ReadString());
        buses.StopTransferringData(BusControl.BUS_ROUTE.PC_MAR);

        //Increment the PC.
        buses.StartTransferringData(BusControl.BUS_ROUTE.PC_PC);
        yield return new WaitForSeconds(clock.GetSpeed());
        PC.Increment();
        buses.StopTransferringData(BusControl.BUS_ROUTE.PC_PC);
        
        //Set the memory pointer to the value of MAR.
        buses.StartTransferringData(BusControl.BUS_ROUTE.MAR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        memory.SetPointer(MAR.ReadUnsigned());
        buses.StopTransferringData(BusControl.BUS_ROUTE.MAR_MEMORY);

        //Write the contents of the memory address being pointed to into MDR.
        buses.StartTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        MDR.Write(memory.ReadFromMemorySlot());
        buses.StopTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);

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

        //Copy the contents of the MDR into the IR for decoding.
        buses.StartTransferringData(BusControl.BUS_ROUTE.MDR_IR);
        yield return new WaitForSeconds(clock.GetSpeed());
        IR.Write(MDR.ReadString());
        buses.StopTransferringData(BusControl.BUS_ROUTE.MDR_IR);

        //Decode IR's opcode.
        buses.StartTransferringData(BusControl.BUS_ROUTE.IR_CU);
        yield return new WaitForSeconds(clock.GetSpeed());
        SetCurrentInstructionFromCU(IR.Opcode());
        buses.StopTransferringData(BusControl.BUS_ROUTE.IR_CU);

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
                    case (0):
                        ALU.SetAdditionCircuitry();
                        StartCoroutine(Instruction_IMMEDIATE_COMPUTE(GP_A));
                        break;
                    case (1):
                        ALU.SetAdditionCircuitry();
                        StartCoroutine(Instruction_IMMEDIATE_COMPUTE(GP_B));
                        break;
                    case (2):
                        ALU.SetAdditionCircuitry();
                        StartCoroutine(Instruction_DIRECT_COMPUTE(GP_A));
                        break;
                    case (3):
                        ALU.SetAdditionCircuitry();
                        StartCoroutine(Instruction_DIRECT_COMPUTE(GP_B));
                        break;
                    case (4):
                        ALU.SetAdditionCircuitry();
                        StartCoroutine(InstructionCoroutine_COMPUTE(GP_A, GP_B));
                        break;
                    case (5):
                        ALU.SetAdditionCircuitry();
                        StartCoroutine(InstructionCoroutine_COMPUTE(GP_B, GP_A));
                        break;
                    case (6):
                        ALU.SetSubtractionCircuitry();
                        StartCoroutine(Instruction_IMMEDIATE_COMPUTE(GP_A));
                        break;
                    case (7):
                        ALU.SetSubtractionCircuitry();
                        StartCoroutine(Instruction_IMMEDIATE_COMPUTE(GP_B));
                        break;
                    case (8):
                        ALU.SetSubtractionCircuitry();
                        StartCoroutine(Instruction_DIRECT_COMPUTE(GP_A));
                        break;
                    case (9):
                        ALU.SetSubtractionCircuitry();
                        StartCoroutine(Instruction_DIRECT_COMPUTE(GP_B));
                        break;
                    case (10):
                        ALU.SetSubtractionCircuitry();
                        StartCoroutine(InstructionCoroutine_COMPUTE(GP_A, GP_B));
                        break;
                    case (11):
                        ALU.SetSubtractionCircuitry();
                        StartCoroutine(InstructionCoroutine_COMPUTE(GP_B, GP_A));
                        break;
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
     * @brief A coroutine to compute a register
     *        using immediate addressing.
     * @param x - The register to compute.
     */
    IEnumerator Instruction_IMMEDIATE_COMPUTE(Register x) {
        currentlyProcessing = true;

        //Copy the contents of register 'x' into ALUx.
        buses.StartTransferringData(x.RouteToALUx);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteX(x.ReadString());
        buses.StopTransferringData(x.RouteToALUx);

        //Copy the IRs operand into ALUy.
        buses.StartTransferringData(BusControl.BUS_ROUTE.IR_ALUY);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteY(IR.OperandString());
        buses.StopTransferringData(BusControl.BUS_ROUTE.IR_ALUY);

        //Compute ALUz.
        buses.StartTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.ComputeZ();
        buses.StopTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);

        //Store the result into register x.
        buses.StartTransferringData(x.RouteToALUz);
        yield return new WaitForSeconds(clock.GetSpeed());
        x.Write(ALU.ReadZ());
        buses.StopTransferringData(x.RouteToALUz);

        currentlyProcessing = false;
    }

    /**
     * @brief A coroutine to compute a register using
     *        direct addressing.
     * @param x - The register to compute.
     */
    IEnumerator Instruction_DIRECT_COMPUTE(Register x) {
        currentlyProcessing = true;

        //Copy the IRs operand into MAR.
        buses.StartTransferringData(IR.RouteToMAR);
        yield return new WaitForSeconds(clock.GetSpeed());
        MAR.Write(IR.OperandString());
        buses.StopTransferringData(IR.RouteToMAR);

        //Set the memory pointer to the value of MAR.
        buses.StartTransferringData(BusControl.BUS_ROUTE.MAR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        memory.SetPointer(MAR.ReadUnsigned());
        buses.StopTransferringData(BusControl.BUS_ROUTE.MAR_MEMORY);

        //Write the contents of the memory address being pointed to into MDR.
        buses.StartTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        MDR.Write(memory.ReadFromMemorySlot());
        buses.StopTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);

        //Send the contends of MDR to the ALU.
        buses.StartTransferringData(MDR.RouteToALUy);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteY(MDR.ReadString());
        buses.StopTransferringData(MDR.RouteToALUy);

        //Send the contents of register x to the ALU.
        buses.StartTransferringData(MDR.RouteToALUx);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteX(x.ReadString());
        buses.StopTransferringData(MDR.RouteToALUx);

        //Compute ALUz.
        buses.StartTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.ComputeZ();
        buses.StopTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);

        //Store the result into register x.
        buses.StartTransferringData(x.RouteToALUz);
        yield return new WaitForSeconds(clock.GetSpeed());
        x.Write(ALU.ReadZ());
        buses.StopTransferringData(x.RouteToALUz);

        currentlyProcessing = false;
    }

    /**
     * @brief A coroutine to perform a compute two
     *        registers to the first register
     * @param x - The register to be used.
     * @param y - The other register to be used.
     */
    IEnumerator InstructionCoroutine_COMPUTE(Register x, Register y) {
        currentlyProcessing = true;

        //Copy the contents of register 'x' into ALUx.
        buses.StartTransferringData(x.RouteToALUx);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteX(x.ReadString());
        buses.StopTransferringData(x.RouteToALUx);

        //Copy the contents of register 'y' into ALUy.
        buses.StartTransferringData(y.RouteToALUy);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteY(y.ReadString());
        buses.StopTransferringData(y.RouteToALUy);

        //Compute ALUz.
        buses.StartTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.ComputeZ();
        buses.StopTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);

        //Store the result into register x.
        buses.StartTransferringData(x.RouteToALUz);
        yield return new WaitForSeconds(clock.GetSpeed());
        x.Write(ALU.ReadZ());
        buses.StopTransferringData(x.RouteToALUz);

        currentlyProcessing = false;
    }
}
