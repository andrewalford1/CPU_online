/**
 * @brief   Class representing the MDR.
 * @extends Register
 * @author  Andrew Alford
 * @date    28/02/2019
 * @version 1.1 - 29/02/2019
 */
public class MemoryDataRegister : Register
{
    public override BusControl.BUS_ROUTE RouteToPC      => BusControl.BUS_ROUTE.PC_MDR;
    public override BusControl.BUS_ROUTE RouteToMAR     => BusControl.BUS_ROUTE.MAR_MDR;
    public override BusControl.BUS_ROUTE RouteToMDR     => BusControl.BUS_ROUTE.NONE;
    public override BusControl.BUS_ROUTE RouteToIR      => BusControl.BUS_ROUTE.MDR_IR;
    public override BusControl.BUS_ROUTE RouteToCU      => BusControl.BUS_ROUTE.MDR_CU;
    public override BusControl.BUS_ROUTE RouteToGPA     => BusControl.BUS_ROUTE.MDR_GPA;
    public override BusControl.BUS_ROUTE RouteToGPB     => BusControl.BUS_ROUTE.MDR_GPB;
    public override BusControl.BUS_ROUTE RouteToALUx    => BusControl.BUS_ROUTE.MDR_ALUX;
    public override BusControl.BUS_ROUTE RouteToALUy    => BusControl.BUS_ROUTE.MDR_ALUY;
    public override BusControl.BUS_ROUTE RouteToALUz    => BusControl.BUS_ROUTE.MDR_ALUZ;
}