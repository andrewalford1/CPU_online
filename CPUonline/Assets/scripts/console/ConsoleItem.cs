using UnityEngine;
using UnityEngine.UI;

/**
 * @brief   Class describing items which are logged to the console.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    06/03/2019
 * @version 1.2 - 29/03/2019
 */
public class ConsoleItem : MonoBehaviour
{
    /**
     * @brief Sets the text for the console item.
     * @param message - The message to set the text to.
     * @param colour - The colour to set the text to.
     */
    public void SetText(string message, Color colour) {
        GetComponent<Text>().text = message;
        GetComponent<Text>().color = colour;
    }

    /**
     * @returns the console item's text component.
     */
    public string GetText() {
        return GetComponent<Text>().text;
    }
}
