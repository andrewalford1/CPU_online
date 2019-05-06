using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * @brief   Toggles a tool tip when a button is entered.
 * @extends MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
 * @author  Andrew Alford
 * @date    27/04/2019
 * @version 1.0 - 27/04/2019
 */
public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //[toolTip] The tooltip to be toggled.
    [SerializeField] private Image toolTip = null;

    //@brief Hides the tool tip by defualt.
    private void Start() {
        toolTip.gameObject.SetActive(false);
    }

    //@brief Shows the tool tip when the mouse enters the object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(true);
    }

    //@brief Hides the game object when the mouse exits the game object.
    public void OnPointerExit(PointerEventData eventData) {
        toolTip.gameObject.SetActive(false);
    }
}
