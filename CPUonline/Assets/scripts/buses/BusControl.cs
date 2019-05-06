using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief   Class to define and control all bus animations.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    28/03/2019
 * @version 1.0 - 28/03/2019
 */
public class BusControl : MonoBehaviour {
    //BUS_PATHS {
    //    MAIN_A,                 //0
    //    MAIN_B,                 //1
    //    MAIN_C,                 //2
    //    MAIN_D,                 //3
    //    MAIN_E,                 //4
    //    MAIN_F,                 //5
    //    MAIN_G,                 //6
    //    MAIN_H,                 //7
    //    MAIN_I,                 //8
    //    MAIN_J,                 //9
    //    MAIN_K,                 //10
    //    PC_TO_PC_1,             //11
    //    PC_TO_PC_2,             //12
    //    PC_TO_PC_3,             //13
    //    PC_TO_PC_4,             //14
    //    PC_TO_MAIN_A,           //15
    //    MAR_TO_MAIN_A,          //16
    //    IR_TO_MAIN_C,           //17
    //    MDR_TO_MAIN_C,          //18
    //    CU_TO_MAIN_E,           //19
    //    GPA_TO_MAIN_G,          //20
    //    GPB_TO_MAIN_I,          //21
    //    ALUX_TO_MAIN_G_1,       //22
    //    ALUX_TO_MAIN_G_2,       //23
    //    ALUX_TO_MAIN_G_3,       //24
    //    ALUY_TO_MAIN_K_1,       //25
    //    ALUY_TO_MAIN_K_2,       //26
    //    ALUY_TO_MAIN_K_3,       //27
    //    ALUZ_TO_MAIN_I,         //28
    //    ALUZ_TO_PSR_1,          //29
    //    ALUZ_TO_PSR_2,          //30
    //    IR_TO_CU,               //31
    //    MAR_TO_MEMORY,          //32
    //    MDR_TO_MEMORY           //33
    //}

    public enum BUS_ROUTE {
        PC_PC,                  //0      PATH{ 11 -> 12 -> 13 -> 14 }
        PC_MAR,                 //1      PATH{ 15 -> 0  -> 16 }
        PC_MDR,                 //2      PATH{ 15 -> 0  -> 1  -> 2  -> 18 }
        PC_IR,                  //3      PATH{ 15 -> 0  -> 1  -> 2  -> 17 }
        PC_CU,                  //4      PATH{ 15 -> 0  -> 1  -> 2  -> 3  -> 4  -> 19 }
        PC_GPA,                 //5      PATH{ 15 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 20 }
        PC_GPB,                 //6      PATH{ 15 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 21 }
        PC_ALUX,                //7      PATH{ 15 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 24 -> 23 -> 22 }
        PC_ALUY,                //8      PATH{ 15 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        PC_ALUZ,                //9      PATH{ 15 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 28 }
        MAR_MEMORY,             //10     PATH{ 32 }
        MAR_MDR,                //11     PATH{ 16 -> 0  -> 1  -> 2  -> 18 }
        MAR_IR,                 //12     PATH{ 16 -> 0  -> 1  -> 2  -> 17 }
        MAR_CU,                 //13     PATH{ 16 -> 0  -> 1  -> 2  -> 3  -> 4  -> 19 }
        MAR_GPA,                //14     PATH{ 16 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 20 }
        MAR_GPB,                //15     PATH{ 16 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 21 }
        MAR_ALUX,               //16     PATH{ 16 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 24 -> 23 -> 22 }
        MAR_ALUY,               //17     PATH{ 16 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        MAR_ALUZ,               //18     PATH{ 16 -> 0  -> 1  -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 28 }
        MDR_MEMORY,             //19     PATH{ 33 }
        MDR_IR,                 //20     PATH{ 18 -> 2  -> 17 }
        MDR_CU,                 //21     PATH{ 18 -> 2  -> 3  -> 4  -> 19 }
        MDR_GPA,                //22     PATH{ 18 -> 2  -> 3  -> 4  -> 5  -> 6  -> 20 }
        MDR_GPB,                //23     PATH{ 18 -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 21 }
        MDR_ALUX,               //24     PATH{ 18 -> 2  -> 3  -> 4  -> 5  -> 6  -> 24 -> 23 -> 22 }
        MDR_ALUY,               //25     PATH{ 18 -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        MDR_ALUZ,               //26     PATH{ 18 -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 28 }
        IR_CU,                  //27     PATH{ 31 }
        IR_GPA,                 //28     PATH{ 17 -> 2  -> 3  -> 4  -> 5  -> 6  -> 20 }
        IR_GPB,                 //29     PATH{ 17 -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 21 }
        IR_ALUX,                //30     PATH{ 17 -> 2  -> 3  -> 4  -> 5  -> 6  -> 24 -> 23 -> 22 }
        IR_ALUY,                //31     PATH{ 17 -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        IR_ALUZ,                //32     PATH{ 17 -> 2  -> 3  -> 4  -> 5  -> 6  -> 7  -> 8  -> 28 }
        CU_IR,                  //33     PATH{ 19 -> 4  -> 3  -> 2  -> 17 }
        CU_GPA,                 //34     PATH{ 19 -> 4  -> 5  -> 6  -> 20 }
        CU_GPB,                 //35     PATH{ 19 -> 4  -> 5  -> 6  -> 7  -> 8  -> 21 }
        CU_ALUX,                //36     PATH{ 19 -> 4  -> 5  -> 6  -> 24 -> 23 -> 22 }
        CU_ALUY,                //37     PATH{ 19 -> 4  -> 5  -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        CU_ALUZ,                //38     PATH{ 19 -> 4  -> 5  -> 6  -> 7  -> 8  -> 28 }
        GPA_GPB,                //39     PATH{ 20 -> 6  -> 7  -> 8  -> 21 }
        GPA_ALUX,               //40     PATH{ 20 -> 6  -> 24 -> 23 -> 22 }
        GPA_ALUY,               //41     PATH{ 20 -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        GPA_ALUZ,               //42     PATH{ 20 -> 6  -> 7  -> 8  -> 28 }
        GPB_ALUX,               //43     PATH{ 21 -> 8  -> 7  -> 6  -> 24 -> 23 -> 22 }
        GPB_ALUY,               //44     PATH{ 21 -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        GPB_ALUZ,               //45     PATH{ 21 -> 8  -> 28 }
        ALUX_ALUY,              //46     PATH{ 22 -> 23 -> 24 -> 6  -> 7  -> 8  -> 9  -> 10 -> 27 -> 26 -> 25 }
        ALUX_ALUZ,              //47     PATH{ 22 -> 23 -> 24 -> 6  -> 7  -> 8  -> 28 }
        ALUZ_PSR,               //48     PATH{ 29 -> 30 }
        NONE                    //49     USE WHEN NO ROUTE IS APPLICABLE
    }

    //Define routes for each bus.
    //PC
    private readonly int[] pc_to_pc         = { 11, 12, 13, 14 };                                       //0
    private readonly int[] pc_to_mar        = { 15, 0, 16 };                                            //1
    private readonly int[] pc_to_mdr        = { 15, 0, 1, 2, 18 };                                      //2
    private readonly int[] pc_to_ir         = { 15, 0, 1, 2, 17 };                                      //3
    private readonly int[] pc_to_cu         = { 15, 0, 1, 2, 3, 4, 19 };                                //4
    private readonly int[] pc_to_gpa        = { 15, 0, 1, 2, 3, 4, 5, 6, 20 };                          //5
    private readonly int[] pc_to_gpb        = { 15, 0, 1, 2, 3, 4, 5, 6, 7, 8, 21 };                    //6
    private readonly int[] pc_to_alux       = { 15, 0, 1, 2, 3, 4, 5, 6, 24, 23, 22 };                  //7
    private readonly int[] pc_to_aluy       = { 15, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 27, 26, 25 };     //8
    private readonly int[] pc_to_aluz       = { 15, 0, 1, 2, 3, 4, 5, 6, 7, 8, 28 };                    //9
    //MAR
    private readonly int[] mar_to_memory    = { 32 };                                                   //10
    private readonly int[] mar_to_mdr       = { 16, 0, 1, 2, 18 };                                      //11
    private readonly int[] mar_to_ir        = { 16, 0, 1, 2, 17 };                                      //12
    private readonly int[] mar_to_cu        = { 16, 0, 1, 2, 3, 4, 19 };                                //13
    private readonly int[] mar_to_gpa       = { 16, 0, 1, 2, 3, 4, 5, 6, 20 };                          //14
    private readonly int[] mar_to_gpb       = { 16, 0, 1, 2, 3, 4, 5, 6, 7, 8, 21 };                    //15
    private readonly int[] mar_to_alux      = { 16, 0, 1, 2, 3, 4, 5, 6, 24, 23, 22 };                  //16
    private readonly int[] mar_to_aluy      = { 16, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 27, 26, 25 };     //17
    private readonly int[] mar_to_aluz      = { 16, 0, 1, 2, 3, 4, 5, 6, 7, 8, 28 };                    //18
    //MDR
    private readonly int[] mdr_to_memory    = { 33 };                                                   //19
    private readonly int[] mdr_to_ir        = { 18, 2, 17 };                                            //20
    private readonly int[] mdr_to_cu        = { 18, 2, 3, 4, 19 };                                      //21
    private readonly int[] mdr_to_gpa       = { 18, 2, 3, 4, 5, 6, 20 };                                //22
    private readonly int[] mdr_to_gpb       = { 18, 2, 3, 4, 5, 6, 7, 8, 21 };                          //23
    private readonly int[] mdr_to_alux      = { 18, 2, 3, 4, 5, 6, 24, 23, 22 };                        //24
    private readonly int[] mdr_to_aluy      = { 18, 2, 3, 4, 5, 6, 7, 8, 9, 10, 27, 26, 25 };           //25
    private readonly int[] mdr_to_aluz      = { 18, 2, 3, 4, 5, 6, 7, 8, 28 };                          //26
    //IR
    private readonly int[] ir_to_cu         = { 31 };                                                   //27
    private readonly int[] ir_to_gpa        = { 17, 2, 3, 4, 5, 6, 20 };                                //28
    private readonly int[] ir_to_gpb        = { 17, 2, 3, 4, 5, 6, 7, 8, 21 };                          //29
    private readonly int[] ir_to_alux       = { 17, 2, 3, 4, 5, 6, 24, 23, 22 };                        //30
    private readonly int[] ir_to_aluy       = { 17, 2, 3, 4, 5, 6, 7, 8, 9, 10, 27, 26, 25 };           //31
    private readonly int[] ir_to_aluz       = { 17, 2, 3, 4, 5, 6, 7, 8, 28 };                          //32
    //CU
    private readonly int[] cu_to_ir         = { 19, 4, 3, 2, 17 };                                      //33
    private readonly int[] cu_to_gpa        = { 19, 4, 5, 6, 20 };                                      //34
    private readonly int[] cu_to_gpb        = { 19, 4, 5, 6, 7, 8, 21 };                                //35
    private readonly int[] cu_to_alux       = { 19, 4, 5, 6, 24, 23, 22 };                              //36
    private readonly int[] cu_to_aluy       = { 19, 4, 5, 6, 7, 8, 9, 10, 27, 26, 25 };                 //37
    private readonly int[] cu_to_aluz       = { 19, 4, 5, 6, 7, 8, 28 };                                //38
    //GPA
    private readonly int[] gpa_to_gpb       = { 20, 6, 7, 8, 21 };                                      //39
    private readonly int[] gpa_to_alux      = { 20, 6, 24, 23, 22 };                                    //40
    private readonly int[] gpa_to_aluy      = { 20, 6, 7, 8, 9, 10, 27, 26, 25 };                       //41
    private readonly int[] gpa_to_aluz      = { 20, 6, 7, 8, 28 };                                      //42
    //GPB
    private readonly int[] gpb_to_alux      = { 21, 8, 7, 6, 24, 23, 22 };                              //43
    private readonly int[] gpb_to_aluy      = { 21, 8, 9, 10, 27, 26, 25 };                             //44
    private readonly int[] gpb_to_aluz      = { 21, 8, 28 };                                            //45
    //ALUX
    private readonly int[] alux_to_aluy     = { 22, 23, 24, 6, 7, 8, 9, 10, 27, 26, 25 };               //46
    private readonly int[] alux_to_aluz     = { 22, 23, 24, 6, 7, 8, 28 };                              //47
    //ALUZ
    private readonly int[] aluz_to_psr      = { 29, 30 };                                               //48



    //[routes] Holds all bus routes.
    private List<int[]> routes = new List<int[]>();

    //[busPaths] A collection of all the bus objects to be controlled.
    [SerializeField] GameObject[] busPaths = null;
    
    //[staticSprite] The sprite to use when a bus shouldn't be animated.
    private readonly Sprite staticSprite = null;
    //[animatedSprite] The sprite to use when a bus should be animated.
    private Sprite animatedSprite = null;

    /**
     * @brief Initalises the Bus Controller
     */
    private void Start() {
        //Set up the animated sprite.
        animatedSprite = busPaths[0].GetComponent<Image>().sprite;

        //Add all routes.
        //PC
        routes.Add(pc_to_pc);
        routes.Add(pc_to_mar);
        routes.Add(pc_to_mdr);
        routes.Add(pc_to_ir);
        routes.Add(pc_to_cu);
        routes.Add(pc_to_gpa);
        routes.Add(pc_to_gpb);
        routes.Add(pc_to_alux);
        routes.Add(pc_to_aluy);
        routes.Add(pc_to_aluz);
        //MAR
        routes.Add(mar_to_memory);
        routes.Add(mar_to_mdr);
        routes.Add(mar_to_ir);
        routes.Add(mar_to_cu);
        routes.Add(mar_to_gpa);
        routes.Add(mar_to_gpb);
        routes.Add(mar_to_alux);
        routes.Add(mar_to_aluy);
        routes.Add(mar_to_aluz);
        //MDR
        routes.Add(mdr_to_memory);
        routes.Add(mdr_to_ir);
        routes.Add(mdr_to_cu);
        routes.Add(mdr_to_gpa);
        routes.Add(mdr_to_gpb);
        routes.Add(mdr_to_alux);
        routes.Add(mdr_to_aluy);
        routes.Add(mdr_to_aluz);
        //IR
        routes.Add(ir_to_cu);
        routes.Add(ir_to_gpa);
        routes.Add(ir_to_gpb);
        routes.Add(ir_to_alux);
        routes.Add(ir_to_aluy);
        routes.Add(ir_to_aluz);
        //CU
        routes.Add(cu_to_ir);
        routes.Add(cu_to_gpa);
        routes.Add(cu_to_gpb);
        routes.Add(cu_to_alux);
        routes.Add(cu_to_aluy);
        routes.Add(cu_to_aluz);
        //GPA
        routes.Add(gpa_to_gpb);
        routes.Add(gpa_to_alux);
        routes.Add(gpa_to_aluy);
        routes.Add(gpa_to_aluz);
        //GPB
        routes.Add(gpb_to_alux);
        routes.Add(gpb_to_aluy);
        routes.Add(gpb_to_aluz);
        //ALUX
        routes.Add(alux_to_aluy);
        routes.Add(alux_to_aluz);
        //ALUZ
        routes.Add(aluz_to_psr);

        //Initially turn off all animations.
        for (int i = 0; i < routes.Count; i++) {
            ToggleAnimations((BUS_ROUTE)i, false);
        }
    }

    /**
     * @brief Toogles the animation of a given route.
     * @param route - The route who's animation will be toggled.
     * @param active - If 'true' the animation will be played, 
     *                 otherwise stop playing it.
     */
    private void ToggleAnimations(BUS_ROUTE route, bool active) {
        //Loop through every bus object in the route.
        for (int i = 0; i < routes[(int)route].Length; i++) {
            if (active) {
                //Add the animation.
                busPaths[routes[(int)route][i]].GetComponent<Image>().sprite = animatedSprite;
            } else {
                //Remove the animation.
                busPaths[routes[(int)route][i]].GetComponent<Image>().sprite = staticSprite;
            }
            //Play/Stop the animation.
            busPaths[routes[(int)route][i]].GetComponent<Animator>().enabled = active;
        }
    }

    /**
     * @brief Starts transferring data along a specific bus route.
     * @param route - The route to transfer the data along.
     */
    public void StartTransferringData(BUS_ROUTE route) {
        //Only take the route if it is valid.
        if(!route.Equals(BUS_ROUTE.NONE)) {
            ToggleAnimations(route, true);
        }
    }  

    /**
     * @brief Stops transferring data along a specific bus route.
     * @param route - The route to stop transferring data along.
     */
    public void StopTransferringData(BUS_ROUTE route) {
        //Only take the route if it is valid.
        if(!route.Equals(BUS_ROUTE.NONE)) {
            ToggleAnimations(route, false);
        }
    }
}
