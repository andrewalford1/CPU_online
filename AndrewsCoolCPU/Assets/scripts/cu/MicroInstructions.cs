using System.Collections;
using UnityEngine;

/**
 * @brief   Class to handle all CPU microinstructions
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    29/03/2019
 * @version 1.1 - 30/03/2019
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
    private ProcessStatusRegister PSR = null;
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
        ProcessStatusRegister PSR,
        MemoryListControl memory, ArithmeticLogicUnit ALU,
        Clock clock, BusControl busSystem)
    {
        this.PC = PC;
        this.MAR = MAR;
        this.MDR = MDR;
        this.IR = IR;
        this.GPA = GPA;
        this.GPB = GPB;
        this.PSR = PSR;
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
        if (PC.Equals(register)) {
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
     * @brief Uses the data in IR to perform a memory fetch.
     */
    public IEnumerator MemoryDirectFetch() {
        yield return ReadIROperand(MAR);
        yield return MemoryRead();
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
        ALU.ComputeZ();

        busSystem.StartTransferringData(BusControl.BUS_ROUTE.ALUZ_PSR);
        yield return new WaitForSeconds(clock.GetSpeed());
        ALU.SetPSR();
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
            case (0x00):
                ALU.SetAdditionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPA);
                break;
            case (0x01):
                ALU.SetAdditionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPB);
                break;
            case (0x02):
                ALU.SetAdditionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPA);
                break;
            case (0x03):
                ALU.SetAdditionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPB);
                break;
            case (0x04):
                ALU.SetAdditionCircuitry();
                yield return Instruction_COMPUTE(GPA, GPB);
                break;
            case (0x05):
                ALU.SetAdditionCircuitry();
                yield return Instruction_COMPUTE(GPB, GPA);
                break;
            case (0x06):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPA);
                break;
            case (0x07):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_IMMEDIATE_COMPUTE(GPB);
                break;
            case (0x08):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPA);
                break;
            case (0x09):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_DIRECT_COMPUTE(GPB);
                break;
            case (0x0A):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_COMPUTE(GPA, GPB);
                break;
            case (0x0B):
                ALU.SetSubtractionCircuitry();
                yield return Instruction_COMPUTE(GPB, GPA);
                break;
            case (0x0C):
                yield return Instruction_IMMEDIATE_COMPARE(GPA);
                break;
            case (0x0D):
                yield return Instruction_IMMEDIATE_COMPARE(GPB);
                break;
            case (0x0E):
                yield return Instruction_DIRECT_COMPARE(GPA);
                break;
            case (0x0F):
                yield return Instruction_DIRECT_COMPARE(GPB);
                break;
            case (0x10):
                yield return Instruction_COMPARE(GPA, GPB);
                break;
            case (0x11):
                yield return Instruction_COMPARE(GPB, GPA);
                break;
            case (0x12):
                yield return ReadIROperand(GPA);
                break;
            case (0x13):
                yield return ReadIROperand(GPB);
                break;
            case (0x14):
                yield return MemoryDirectFetch();
                yield return WriteToGPA(MDR);
                break;
            case (0x15):
                yield return MemoryDirectFetch();
                yield return WriteToGPB(MDR);
                break;
            case (0x16):
                yield return WriteToGPA(GPB);
                break;
            case (0x17):
                yield return WriteToGPB(GPA);
                break;
            case (0x18):
                yield return Instruction_moveToMemory_DIRECT(GPA);
                break;
            case (0x19):
                yield return Instruction_moveToMemory_DIRECT(GPB);
                break;
            case (0x1A):
                yield return Instruction_moveToMemory_INDIRECT(GPA);
                break;
            case (0x1B):
                yield return Instruction_moveToMemory_INDIRECT(GPB);
                break;
            case (0x1C):
                yield return ReadIROperand(PC);
                break;
            case (0x1D):
                yield return Instruction_DIRECT_jump();
                break;
            case (0x1E):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.CARRY, true);
                break;
            case (0x1F):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.CARRY, false);
                break;
            case (0x20):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.CARRY, true);
                break;
            case (0x21):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.CARRY, false);
                break;
            case (0x22):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.NEGATIVE, true);
                break;
            case (0x23):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.NEGATIVE, false);
                break;
            case (0x24):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.NEGATIVE, true);
                break;
            case (0x25):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.NEGATIVE, false);
                break;
            case (0x26):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.ZERO, true);
                break;
            case (0x27):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.ZERO, false);
                break;
            case (0x28):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.ZERO, true);
                break;
            case (0x29):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.ZERO, false);
                break;
            case (0x2A):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.OVERFLOW, true);
                break;
            case (0x2B):
                yield return Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS.OVERFLOW, false);
                break;
            case (0x2C):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.OVERFLOW, true);
                break;
            case (0x2D):
                yield return Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS.OVERFLOW, false);
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
        yield return MemoryDirectFetch();
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
    public IEnumerator Instruction_COMPUTE(Register x, Register y) {
        yield return WriteToALUx(x);
        yield return WriteToALUy(y);
        yield return ComputeALUz();
        yield return ReadFromALUz(x);
    }

    /**
     * @brief A coroutine to compare a register 
     *        using immediate addressing.
     *        (Result is stored PSR).
     * @param x - The register to compare data with.
     */
    public IEnumerator Instruction_IMMEDIATE_COMPARE(Register x) {
        ALU.SetSubtractionCircuitry();
        yield return WriteToALUx(x);
        yield return ReadIROperandToALUy();
        yield return ComputeALUz();
    }

    /**
     * @brief A coroutine to compare a register 
     *        using direct addressing.
     *        (Result is stored PSR).
     * @param x - The register to compare data with.
     */
    public IEnumerator Instruction_DIRECT_COMPARE(Register x) {
        yield return MemoryDirectFetch();
        ALU.SetSubtractionCircuitry();
        yield return WriteToALUy(MDR);
        yield return WriteToALUx(x);
        yield return ComputeALUz();
    }

    /**
     * @brief Compares the contents of two registers.
     *        (Result is stored PSR).
     * @param x - The first register to be compared.
     * @param y - The second register to be compared.
     */
    public IEnumerator Instruction_COMPARE(Register x, Register y) {
        ALU.SetSubtractionCircuitry();
        yield return WriteToALUx(x);
        yield return WriteToALUy(y);
        yield return ComputeALUz();
    }

    /**
     * @brief Moves data directly into memory.
     *        (Address is calculated from the data in IR).
     * @param x - Holds the content to write to memory.
     */
    public IEnumerator Instruction_moveToMemory_DIRECT(Register x) {
        yield return ReadIROperand(MAR);
        yield return WriteToMDR(x);
        yield return MemoryWrite();
    }

    /**
     * @brief Moves data into memory using indirect addressing.
     *        (Address are calculated from the data in IR).
     * @param x - Holds the content to write to memory.
     */
    public IEnumerator Instruction_moveToMemory_INDIRECT(Register x) {
        yield return MemoryDirectFetch();       
        yield return WriteToIR(MDR);
        yield return WriteToMDR(x);
        yield return ReadIROperand(MAR);
        yield return MemoryWrite();
    }

    /**
     * @brief Jumps to an address using direct addressing.
     */
    public IEnumerator Instruction_DIRECT_jump() {
        yield return MemoryDirectFetch();
        yield return WriteToIR(MDR);
        yield return ReadIROperand(PC);
    }
    
    /**
     * @brief Checks the state of a flag and sets the PC accordingly.
     * @param flag      - The flag to be checked.
     * @param onTrue    - If 'true' then the branch only occurs if 
     *                    the flag is not set. If 'false' then the branch 
     *                    only occurs if the flag is set.
     */
    public IEnumerator Instruction_IMMEDIATE_branchOnFlag(ProcessStatusRegister.FLAGS flag, bool onClear) {
        if(!onClear) {
            if (PSR.GetState(ProcessStatusRegister.FLAGS.CARRY)) {
                yield return ReadIROperand(PC);
            }
        }
        else {
            if (!PSR.GetState(ProcessStatusRegister.FLAGS.CARRY))
            {
                yield return ReadIROperand(PC);
            }
        }
    }

    /**
     * @brief Checks the state of a flag and sets the PC accordingly.
     * @param flag      - The flag to be checked.
     * @param onTrue    - If 'true' then the branch only occurs if 
     *                    the flag is not set. If 'false' then the branch 
     *                    only occurs if the flag is set.
     */
    public IEnumerator Instruction_DIRECT_branchOnFlag(ProcessStatusRegister.FLAGS flag, bool onClear) {
        if (!onClear) {
            if (PSR.GetState(ProcessStatusRegister.FLAGS.CARRY)) {
                yield return Instruction_DIRECT_jump();
            }
        }
        else {
            if (!PSR.GetState(ProcessStatusRegister.FLAGS.CARRY)) {
                yield return Instruction_DIRECT_jump();
            }
        }
    }
}
