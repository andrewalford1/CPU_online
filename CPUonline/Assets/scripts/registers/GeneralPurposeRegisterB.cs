/**
 * @brief   Class representing general purpose register B.
 * @extends GeneralPurposeRegister
 * @author  Andrew Alford
 * @date    29/03/2019
 * @version 1.0 - 29/03/2019
 */
public class GeneralPurposeRegisterB : GeneralPurposeRegister
{
    public override BusControl.BUS_ROUTE RouteToPC      => BusControl.BUS_ROUTE.PC_GPB;
    public override BusControl.BUS_ROUTE RouteToMAR     => BusControl.BUS_ROUTE.MAR_GPB;
    public override BusControl.BUS_ROUTE RouteToMDR     => BusControl.BUS_ROUTE.MDR_GPB;
    public override BusControl.BUS_ROUTE RouteToIR      => BusControl.BUS_ROUTE.IR_GPB;
    public override BusControl.BUS_ROUTE RouteToCU      => BusControl.BUS_ROUTE.CU_GPB;
    public override BusControl.BUS_ROUTE RouteToGPA     => BusControl.BUS_ROUTE.GPA_GPB;
    public override BusControl.BUS_ROUTE RouteToGPB     => BusControl.BUS_ROUTE.NONE;
    public override BusControl.BUS_ROUTE RouteToALUx    => BusControl.BUS_ROUTE.GPB_ALUX;
    public override BusControl.BUS_ROUTE RouteToALUy    => BusControl.BUS_ROUTE.GPB_ALUY;
    public override BusControl.BUS_ROUTE RouteToALUz    => BusControl.BUS_ROUTE.GPB_ALUZ;
}
