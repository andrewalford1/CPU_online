using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    /**
     * @brief Run the testing script.
     */
    void Start()
    {
        Carry_and_overflow();
    }

    private void Carry_and_overflow()
    {
        Debug.Log("----Carry/Overflow-Test-Begin----");

        Number C8 = new Number(0x7FFF);
        C8.Add(new Number(0x0001));
        Debug.Log("Test case C8:\n" + C8.ToString());

        Number C9 = new Number(0x8000);
        C9.Add(new Number(0xFFFF));
        Debug.Log("Test case C9:\n" + C9.ToString());

        Number C10 = new Number(0xFFFF);
        C10.Add(new Number(0x0001));
        Debug.Log("Test case C10:\n" + C10.ToString());

        Number C11 = new Number(0x7FFF);
        C11.Subtract(new Number(0xFFFF));
        Debug.Log("Test case C11:\n" + C11.ToString());

        Number C12 = new Number(0x8000);
        C12.Subtract(new Number(0x0001));
        Debug.Log("Test case C12:\n" + C12.ToString());

        Number C13 = new Number(0x7FFF);
        C13.Multiply(new Number(0x0002));
        Debug.Log("Test case C13:\n" + C13.ToString());

        Number C14 = new Number(0x8000);
        C14.Multiply(new Number(0xFFFE));
        Debug.Log("Test case C14:\n" + C14.ToString());

        Number C15 = new Number(0xFFFF);
        C15.Multiply(new Number(0x0002));
        Debug.Log("Test case C15:\n" + C15.ToString());

        Debug.Log("----Carry/Overflow-Test-End----");
    }
}
