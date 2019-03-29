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
 * @version 1.5 - 29/03/2019
 */
public class ControlUnit : MonoBehaviour
{
    //CU
    [SerializeField] private InputField                 currentCommandDisplay   = null;
    //REGISTERS
    [SerializeField] private ProgramCounter             PC                      = null;
    [SerializeField] private MemoryAddressRegister      MAR                     = null;
    [SerializeField] private MemoryDataRegister         MDR                     = null;
    [SerializeField] private InstructionRegister        IR                      = null;
    [SerializeField] private GeneralPurposeRegisterA    GPA                     = null;
    [SerializeField] private GeneralPurposeRegisterB    GPB                     = null;
    //MEMORY
    [SerializeField] private MemoryListControl          memory                  = null;
    //ALU
    [SerializeField] private ArithmeticLogicUnit        ALU                     = null;
    //CLOCK
    [SerializeField] private Clock                      clock                   = null;
    //BUSES
    [SerializeField] private BusControl                 busSystem               = null;
    //BUTTONS
    [SerializeField] private Button                     fetch_btn               = null;
    [SerializeField] private Button                     decode_btn              = null;
    [SerializeField] private Button                     execute_btn             = null;
    [SerializeField] private Button                     reset_btn               = null;    

    //[instructions] Holds all the instructions that the CU can perform.
    private List<Instruction> instructions = new List<Instruction>();
    private Instruction currentInstruction = null;
    private MicroInstructions microInstructions = null;

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

        //Link in the micro instructions.
        microInstructions = gameObject.GetComponent<MicroInstructions>();
        microInstructions.LinkCPUcomponents(
            PC, MAR, MDR, IR, GPA, GPB,
            memory, ALU, clock, busSystem
        );

        //Delagate listeners to buttons.
        fetch_btn.onClick.AddListener(  delegate {  StartCoroutine(FetchCycle());     });
        decode_btn.onClick.AddListener( delegate {  StartCoroutine(DecodeCycle());    });
        execute_btn.onClick.AddListener(delegate {  StartCoroutine(ExecuteCycle());   });
        reset_btn.onClick.AddListener(  delegate {  Reset();                          });

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
        GPA.SetActive(!currentlyProcessing);
        GPB.SetActive(!currentlyProcessing);
        ALU.SetActive(!currentlyProcessing);
        if (memory.IsActive() && currentlyProcessing) {
            memory.SetActive(false);
        } else if(!memory.IsActive() && !currentlyProcessing) {
            memory.SetActive(true);
        }
    }

    /**
     * @returns 'true' if the CU is currently processing.
     */
    public bool IsCurrentlyProcessing() {
        return currentlyProcessing;
    }

    /**
     * @brief A coroutine to perform the 'fetch' cycle.
     */
    public IEnumerator FetchCycle() {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform fetch as CPU is currently processing.");
        } else {
            currentlyProcessing = true;
            yield return microInstructions.WriteToMar(PC);
            yield return microInstructions.WriteToPC(PC);
           // yield return microInstructions.PCIncrement();
            yield return microInstructions.MemoryRead();
            currentlyProcessing = false;
        }
    }

    /**
     * @brief A coroutine to perform the 'decode' cycle.
     */
    public IEnumerator DecodeCycle()
    {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform decode as CPU is currently processing");
        } else {
            currentlyProcessing = true;

            yield return microInstructions.WriteToIR(MDR);

            //Decode IR's opcode.
            busSystem.StartTransferringData(BusControl.BUS_ROUTE.IR_CU);
            yield return new WaitForSeconds(clock.GetSpeed());
            SetCurrentInstructionFromCU(IR.Opcode());
            busSystem.StopTransferringData(BusControl.BUS_ROUTE.IR_CU);

            currentlyProcessing = false;
        }
    }

    /**
     * @brief Executes the current instruction on the CU.
     */
    public IEnumerator ExecuteCycle()
    {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform execute as CPU is currently processing");
        } else {
            currentlyProcessing = true;
            if (currentInstruction != null) {
                //Execute the current instruction.
                yield return microInstructions.ExecuteInstrucion(currentInstruction.ID);
            }
            currentlyProcessing = false;
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
            GPA.Reset();
            GPB.Reset();
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
}
