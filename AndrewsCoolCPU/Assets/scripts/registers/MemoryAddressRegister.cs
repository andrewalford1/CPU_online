/**
 * @brief Class representing the MAR.
 * @extends Register
 * @author Andrew Alford
 * @date 28/02/2019
 * @version 1.1 - 29/03/2019
 */
public class MemoryAddressRegister : Register {
    public override BusControl.BUS_ROUTE RouteToPC      => BusControl.BUS_ROUTE.PC_MAR;
    public override BusControl.BUS_ROUTE RouteToMAR     => BusControl.BUS_ROUTE.NONE;
    public override BusControl.BUS_ROUTE RouteToMDR     => BusControl.BUS_ROUTE.MAR_MDR;
    public override BusControl.BUS_ROUTE RouteToIR      => BusControl.BUS_ROUTE.MAR_IR;
    public override BusControl.BUS_ROUTE RouteToCU      => BusControl.BUS_ROUTE.MAR_CU;
    public override BusControl.BUS_ROUTE RouteToGPA     => BusControl.BUS_ROUTE.MAR_GPA;
    public override BusControl.BUS_ROUTE RouteToGPB     => BusControl.BUS_ROUTE.MAR_GPB;
    public override BusControl.BUS_ROUTE RouteToALUx    => BusControl.BUS_ROUTE.MAR_ALUX;
    public override BusControl.BUS_ROUTE RouteToALUy    => BusControl.BUS_ROUTE.MAR_ALUY;
    public override BusControl.BUS_ROUTE RouteToALUz    => BusControl.BUS_ROUTE.MAR_ALUZ;
}