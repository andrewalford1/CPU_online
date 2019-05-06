/**
 * @brief   Class representing general purpose register A.
 * @extends GeneralPurposeRegister
 * @author  Andrew Alford
 * @date    29/03/2019
 * @version 1.0 - 29/03/2019
 */
public class GeneralPurposeRegisterA : GeneralPurposeRegister {
    public override BusControl.BUS_ROUTE RouteToPC      => BusControl.BUS_ROUTE.PC_GPA;
    public override BusControl.BUS_ROUTE RouteToMAR     => BusControl.BUS_ROUTE.MAR_GPA;
    public override BusControl.BUS_ROUTE RouteToMDR     => BusControl.BUS_ROUTE.MDR_GPA;
    public override BusControl.BUS_ROUTE RouteToIR      => BusControl.BUS_ROUTE.IR_GPA;
    public override BusControl.BUS_ROUTE RouteToCU      => BusControl.BUS_ROUTE.CU_GPA;
    public override BusControl.BUS_ROUTE RouteToGPA     => BusControl.BUS_ROUTE.NONE;
    public override BusControl.BUS_ROUTE RouteToGPB     => BusControl.BUS_ROUTE.GPA_GPB;
    public override BusControl.BUS_ROUTE RouteToALUx    => BusControl.BUS_ROUTE.GPA_ALUX;
    public override BusControl.BUS_ROUTE RouteToALUy    => BusControl.BUS_ROUTE.GPA_ALUY;
    public override BusControl.BUS_ROUTE RouteToALUz    => BusControl.BUS_ROUTE.GPA_ALUZ;
}
