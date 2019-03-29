using System.Collections;
using UnityEngine;

/**
 * @brief   Class to handle all CPU microinstructions
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    29/03/2019
 * @version 1.0 - 29/03/2019
 */
public class MicroInstructions : MonoBehaviour
{
    //REGISTERS
    private ProgramCounter PC = null;
    private MemoryAddressRegister MAR = null;
    private MemoryDataRegister MDR = null;
    private InstructionRegister IR = null;
    private GeneralPurposeRegisterA GPA = null;
    private GeneralPurposeRegisterB GPB = null;
    //MEMORY
    private MemoryListControl memory = null;
    //ALU
    private ArithmeticLogicUnit ALU = null;
    //CLOCK
    private Clock clock = null;
    //BUSES
    private BusControl busSystem = null;

    /**
     * @brief Links all the component so they 
     *        can be used in micro instructions.
     */
    public void LinkCPUcomponents(
        ProgramCounter PC, MemoryAddressRegister MAR,
        MemoryDataRegister MDR, InstructionRegister IR,
        GeneralPurposeRegisterA GPA, GeneralPurposeRegisterB GPB,
        MemoryListControl memory, ArithmeticLogicUnit ALU,
        Clock clock, BusControl busSystem)
    {
        this.PC = PC;
        this.MAR = MAR;
        this.MDR = MDR;
        this.IR = IR;
        this.GPA = GPA;
        this.GPB = GPB;
        this.memory = memory;
        this.ALU = ALU;
        this.clock = clock;
        this.busSystem = busSystem;
    }

    //DATA_MOVEMENT**********************************************\

    /**
     * @brief Reads the content of the PC into a given register.
     * @param register - The register to be written to.
     */
    public IEnumerator ReadFromPC(Register register) {
        busSystem.StartTransferringData(register.RouteToPC);
        yield return new WaitForSeconds(clock.GetSpeed());
        register.Write(PC.ReadString());
        busSystem.StopTransferringData(register.RouteToPC);
    }

    /**
     * @brief Writes the content of a given register to the PC.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToPC(Register register) {
        if(PC.Equals(register)) {
            busSystem.StartTransferringData(BusControl.BUS_ROUTE.PC_PC);
            yield return new WaitForSeconds(clock.GetSpeed());
            PC.Increment();
            busSystem.StopTransferringData(BusControl.BUS_ROUTE.PC_PC);
        } else {
            busSystem.StartTransferringData(register.RouteToPC);
            yield return new WaitForSeconds(clock.GetSpeed());
            PC.Write(register.ReadString());
            busSystem.StopTransferringData(register.RouteToPC);
        }
    }

    /**
     * @brief Sets the memory pointer to the value of MAR.
     */
    private IEnumerator SetMemoryPointer() {
        busSystem.StartTransferringData(BusControl.BUS_ROUTE.MAR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        memory.SetPointer(MAR.ReadUnsigned());
        busSystem.StopTransferringData(BusControl.BUS_ROUTE.MAR_MEMORY);
    }

    /**
     * @brief Uses the MDR and the MAR to read a value from memory.
     */
    public IEnumerator MemoryRead() {

        yield return SetMemoryPointer();

        //Read the contents of the memory address being pointed to into MAR.
        busSystem.StartTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        MDR.Write(memory.ReadFromMemorySlot());
        busSystem.StopTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);
    }

    /**
     * @brief Writes the content of a given register to the MAR.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToMar(Register register) {
        busSystem.StartTransferringData(register.RouteToMAR);
        yield return new WaitForSeconds(clock.GetSpeed());
        MAR.Write(register.ReadString());
        busSystem.StopTransferringData(register.RouteToMAR);
    }

    /**
     * @brief Uses the MDR and MAR to write a value to memory.
     */
    public IEnumerator MemoryWrite() {

        yield return SetMemoryPointer();

        //Write to the contents of the memory address being pointed to into MAR.
        busSystem.StartTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);
        yield return new WaitForSeconds(clock.GetSpeed());
        memory.WriteToMemorySlot(MDR.ReadString());
        busSystem.StopTransferringData(BusControl.BUS_ROUTE.MDR_MEMORY);
    }

    /**
     * @brief Writes the content of a given register to the MDR.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToMDR(Register register) {
        busSystem.StartTransferringData(register.RouteToMDR);
        yield return new WaitForSeconds(clock.GetSpeed());
        MDR.Write(register.ReadString());
        busSystem.StopTransferringData(register.RouteToMDR);
    }

    /**
     * @brief Reads the content of the IR into a given register.
     * @param register - The register to be written to.
     */
    public IEnumerator ReadFromIR(Register register) {
        busSystem.StartTransferringData(register.RouteToIR);
        yield return new WaitForSeconds(clock.GetSpeed());
        register.Write(IR.ReadString());
        busSystem.StopTransferringData(register.RouteToIR);
    }

    /**
     * @brief Reads the operand of the current instruction
     *        into a given register.
     * @param register - The register to be written to.
     */
    public IEnumerator ReadIROperand(Register register) {
        busSystem.StartTransferringData(register.RouteToIR);
        yield return new WaitForSeconds(clock.GetSpeed());
        register.Write(IR.OperandString());
        busSystem.StopTransferringData(register.RouteToIR);
    }

    /**
     * @brief Reads the operand of the current instruction
     *        into ALU y.
     */
    public IEnumerator ReadIROperandToALUy() {
        busSystem.StartTransferringData(BusControl.BUS_ROUTE.IR_ALUY);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteY(IR.OperandString());
        busSystem.StopTransferringData(BusControl.BUS_ROUTE.IR_ALUY);
    }

    /**
     * @brief Writes the content of a given register to the IR.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToIR(Register register) {
        busSystem.StartTransferringData(register.RouteToIR);
        yield return new WaitForSeconds(clock.GetSpeed());
        IR.Write(register.ReadString());
        busSystem.StopTransferringData(register.RouteToIR);
    }

    /**
     * @brief Reads the content of GPA into a given register.
     * @param register - The register to be written to.
     */
    public IEnumerator ReadFromGPA(Register register) {
        busSystem.StartTransferringData(register.RouteToGPA);
        yield return new WaitForSeconds(clock.GetSpeed());
        register.Write(GPA.ReadString());
        busSystem.StopTransferringData(register.RouteToGPA);
    }

    /**
     * @brief Writes the content of a given register to GPA.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToGPA(Register register) {
        busSystem.StartTransferringData(register.RouteToGPA);
        yield return new WaitForSeconds(clock.GetSpeed());
        GPA.Write(register.ReadString());
        busSystem.StopTransferringData(register.RouteToGPA);
    }

    /**
     * @brief Reads the content of the GPB into a given register.
     * @param register - The register to be written to.
     */
    public IEnumerator ReadFromGPB(Register register) {
        busSystem.StartTransferringData(register.RouteToGPB);
        yield return new WaitForSeconds(clock.GetSpeed());
        register.Write(GPB.ReadString());
        busSystem.StopTransferringData(register.RouteToGPB);
    }

    /**
     * @brief Writes the content of a given register to GPB.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToGPB(Register register) {
        busSystem.StartTransferringData(register.RouteToGPB);
        yield return new WaitForSeconds(clock.GetSpeed());
        GPB.Write(register.ReadString());
        busSystem.StopTransferringData(register.RouteToGPB);
    }

    /**
     * @brief Writes the content of a given register to ALUx.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToALUx(Register register) {
        busSystem.StartTransferringData(register.RouteToALUx);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteX(register.ReadString());
        busSystem.StopTransferringData(register.RouteToALUx);
    }

    /**
     * @brief Writes the content of a given register to ALUy.
     * @param register - The register to be read from.
     */
    public IEnumerator WriteToALUy(Register register) {
        busSystem.StartTransferringData(register.RouteToALUy);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.WriteY(register.ReadString());
        busSystem.StopTransferringData(register.RouteToALUy);
    }

    /**
     * @brief Calculates the value of ALUz.
     */
    public IEnumerator ComputeALUz()
    {
        busSystem.StartTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.ComputeZ();
        busSystem.StopTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);
    }

    /**
     * @brief Stores the value of ALUz into a given register.
     * @param register - The register to store ALUz's content into.
     */
    public IEnumerator ReadFromALUz(Register register) {
        busSystem.StartTransferringData(register.RouteToALUz);
        yield return new WaitForSeconds(clock.GetSpeed());
        register.Write(ALU.ReadZ());
        busSystem.StopTransferringData(register.RouteToALUz);
    }


    //MICRO_INSTRUCTIONS*****************************************/

    /**
     * @brief Executes an instruction.
     * @param id - the ID of the instruction to be executed.
     */
    public IEnumerator ExecuteInstrucion(int id) {
        switch (id) {
            case (0):
                ALU.SetAdditionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPA);
                break;
            case (1):
                ALU.SetAdditionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPB);
                break;
            case (2):
                ALU.SetAdditionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPA);
                break;
            case (3):
                ALU.SetAdditionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPB);
                break;
            case (4):
                ALU.SetAdditionCircuitry();
                yield return InstructionCoroutine_COMPUTE(GPA, GPB);
                break;
            case (5):
                ALU.SetAdditionCircuitry();
                yield return InstructionCoroutine_COMPUTE(GPB, GPA);
                break;
            case (6):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPA);
                break;
            case (7):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPB);
                break;
            case (8):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPA);
                break;
            case (9):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPB);
                break;
            case (10):
                ALU.SetSubtractionCircuitry();
                yield return InstructionCoroutine_COMPUTE(GPA, GPB);
                break;
            case (11):
                ALU.SetSubtractionCircuitry();
                yield return InstructionCoroutine_COMPUTE(GPB, GPA);
                break;
        }
    }

    /**
     * @brief A coroutine to compute a register
     *        using immediate addressing.
     * @param x - The register to compute.
     */
    public IEnumerator Instruction_IMMEDIATE_COMPUTE(Register x) { 
        yield return WriteToALUx(x);
        yield return ReadIROperandToALUy();
        yield return ComputeALUz();
        yield return ReadFromALUz(x);
    }

    /**
     * @brief A coroutine to compute a register using
     *        direct addressing.
     * @param x - The register to compute.
     */
    public IEnumerator Instruction_DIRECT_COMPUTE(Register x) {
        yield return ReadIROperand(MAR);
        yield return MemoryRead();
        yield return WriteToALUy(MDR);
        yield return WriteToALUx(x);
        yield return ComputeALUz();
        yield return ReadFromALUz(x);
    }

    /**
     * @brief A coroutine to perform a compute two
     *        registers to the first register
     * @param x - The register to be used.
     * @param y - The other register to be used.
     */
    public IEnumerator InstructionCoroutine_COMPUTE(Register x, Register y) {
        yield return WriteToALUx(x);
        yield return WriteToALUy(y);
        yield return ComputeALUz();
        yield return ReadFromALUz(x);
    }

}
