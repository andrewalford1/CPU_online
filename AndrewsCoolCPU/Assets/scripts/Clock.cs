using System;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief   Class to control the CPU clock.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    27/03/2019
 * @version 1.0 - 27/03/2019
 */
public class Clock : MonoBehaviour
{
    //[control] The slider to control the clock speed.
    [SerializeField] private Slider control = null;
    //[display] Text to display the current clock speed.
    [SerializeField] private Text display = null;

    //[speed] The current clock speed.
    private float speed = 1.0f;

    /**
     * @brief Initalises the CPU clock.
     */
    void Start()
    {
        SetDisplay();
        control.onValueChanged.AddListener(delegate {
            SetSpeed();
            SetDisplay();
        });
    }

    /**
     * @returns The slider controlling the clock.
     */
    public Slider GetControl() {
        return control;
    }

    /**
     * @returns the Clock's current speed.
     */
    public float GetSpeed() {
        return speed;
    }

    /**
     * @brief Sets the clock speed from the slider's value.
     */
    private void SetSpeed() {
        speed = (float)(Math.Round((double)control.value, 2));
    }

    /**
     * @brief Sets the display to represent 
     *        the current clock speed.
     */
    private void SetDisplay() {
        display.text = speed.ToString();
    }

    public void Reset() {
        control.value = 1.0f;
    }
}
