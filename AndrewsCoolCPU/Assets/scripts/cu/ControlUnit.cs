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
    [SerializeField] private ProcessStatusRegister      PSR                     = null;
    //MEMORY
    [SerializeField] private MemoryListControl          memory                  = null;
    //ALU
    [SerializeField] private ArithmeticLogicUnit        ALU                     = null;
    //CLOCK
    [SerializeField] private Clock                      clock                   = null;
    //BUSES
    [SerializeField] private BusControl                 busSystem               = null;
    //BUTTONS
    [SerializeField] private Button assemble = null;
    [SerializeField] private Button loadToMemory = null;
    [SerializeField] private Button play_pause = null;
    [SerializeField] private Button fetch = null;
    [SerializeField] private Button decode = null;
    [SerializeField] private Button execute = null;
    [SerializeField] private Button reset = null;

    //[instructionSet] Holds all the instructions that the CU can perform.
    private List<Instruction> instructionSet = new List<Instruction>();
    private Instruction currentInstruction = null;
    private MicroInstructions microInstructions = null;

    //[currentlyProecssing] 'True' whilst the CU is in use. 
    //(Prevents multiple operations occurring at once).
    private bool currentlyProcessing = false;
    //[loading] 'True' whilst the CU is initialising.
    private bool loading = true;
    //[running] 'True' whilst the CU is running the F/D/E cycle continously.
    private bool running = false;

    /**
     * @brief Inialise the Control Unit.
     */
    IEnumerator Start()
    {
        //Wait until the insturction set it loaded.
        yield return LoadInstructionSet();

        //Link in the micro instructions.
        microInstructions = gameObject.GetComponent<MicroInstructions>();
        microInstructions.LinkCPUcomponents(
            PC, MAR, MDR, IR, GPA, GPB, PSR,
            memory, ALU, clock, busSystem
        );

        //Set up the first instruction in the CU.
        SetCurrentInstructionFromCU(0);

        //Set up the buttons.
        InitialiseButtons();

        loading = false;
    }

    private void InitialiseButtons() {
        fetch.onClick.AddListener(delegate { StartCoroutine(FetchCycle()); });
        decode.onClick.AddListener(delegate { StartCoroutine(DecodeCycle()); });
        execute.onClick.AddListener(delegate { StartCoroutine(ExecuteCycle()); });
        reset.onClick.AddListener(delegate { Reset(); });
        play_pause.onClick.AddListener(delegate
        {
            running = !running;
            play_pause.GetComponent<ImageControlBtn>().SetImage(!running);
            if (running) {
                StartCoroutine(RunCycle());
            }
        });
    }

    /**
     * @brief Loads the CU's instruction set.
     */
    private IEnumerator LoadInstructionSet() {
        string url = Application.streamingAssetsPath + "/json/instruction_set.json";
        string json;

        //Check if we should use UnityWebRequest or File.ReadAllBytes
        if (url.Contains("://") || url.Contains(":///")) {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            json = www.downloadHandler.text;
        } else {
            json = File.ReadAllText(url);
        }

        foreach (Instruction instruction in JsonUtility
            .FromJson<InstructionSet>(json).instructions) {
            instructionSet.Add(instruction);
        }
    }

    /**
     * @brief Updates the CU once every frame.
     */
    private void Update() {
        PC.SetActive(!currentlyProcessing);
        MAR.SetActive(!currentlyProcessing);
        MDR.SetActive(!currentlyProcessing);
        IR.SetActive(!currentlyProcessing);
        GPA.SetActive(!currentlyProcessing);
        GPB.SetActive(!currentlyProcessing);
        ALU.SetActive(!currentlyProcessing);
        assemble.interactable       = !currentlyProcessing;
        loadToMemory.interactable   = !currentlyProcessing;
        fetch.interactable          = !currentlyProcessing;
        decode.interactable         = !currentlyProcessing;
        execute.interactable        = !currentlyProcessing;
        reset.interactable          = !currentlyProcessing;
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
     * @returns 'ture' if the CU is currently loading.
     */
    public bool IsLoading() {
        return loading;
    }

    public bool IsRunning() {
        return running;
    }

    public IEnumerator RunCycle() {
        if(currentlyProcessing) {
            Debug.Log("Cannot run program as CPU is currently processing.");
        } else {
            running = true;
            play_pause.GetComponent<ImageControlBtn>().SetImage(!running);
            while (currentInstruction.ID != 0xFF && running) {
                yield return FetchCycle();
                yield return DecodeCycle();
                yield return ExecuteCycle();                
            }
            running = false;
            play_pause.GetComponent<ImageControlBtn>().SetImage(!running);
        }
    }

    /**
     * @brief A coroutine to perform the 'fetch' cycle.
     */
    public IEnumerator FetchCycle() {
        if(currentlyProcessing) {
            Debug.Log("Cannot perform fetch as CPU is currently processing.");
        } else {
            currentlyProcessing = true;
            ConsoleControl.CONSOLE.LogMessage("Performing Fetch Cycle");
            yield return microInstructions.WriteToMar(PC);
            yield return microInstructions.WriteToPC(PC);
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
            ConsoleControl.CONSOLE.LogMessage("Performing Decode Cycle");

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
                ConsoleControl.CONSOLE.LogMessage("Performing Execute Cycle");
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
        foreach(Instruction instruction in instructionSet)
        {
            if(instruction.command.Equals(command))
            {
                return instruction;
            }
        }
        return null;
    }

    /**
     * @brief Assigns an instruction to be executed to the CU.
     * @param id - The id of the instruction being set.
     */
    private void SetCurrentInstructionFromCU(int id) {
        bool validID = false;

        //Check if the id is valid.
        for(int i = 0; i < instructionSet.Count; i++) {
            if (id == instructionSet[i].ID) {
                currentInstruction = instructionSet[i];
                currentCommandDisplay.text = currentInstruction.command.Substring(1);
                validID = true;
            }

        }
        if (!validID) {
            ConsoleControl.CONSOLE.LogError("Invalid instruction given");
        }
    }

    /**
     * @brief Assigns an instruction to be executed to the CU.
     * @param id - The id of the instruction being set.
     */
    public void SetCurrentInstructionFromConsole(int id)
    {
        if (currentlyProcessing) {
            Debug.Log("Cannot set instruction as CPU is currently processing");
        } else {
            SetCurrentInstructionFromCU(id);
        }
    }
}
