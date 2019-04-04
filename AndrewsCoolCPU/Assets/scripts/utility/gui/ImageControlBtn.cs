using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @brief   Helper class to control the 
 *          image displayed by a button.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    04/04/19
 * @version 1.0 - 04/04/19
 */
public class ImageControlBtn : MonoBehaviour {

    //[button] The button this scrip affects.
    [SerializeField] private Button button = null;
    
    //[imageToControl] The buttons image being controlled.
    [SerializeField]  private Image imageToControl = null;
    
    [SerializeField] private Sprite imageA = null;
    [SerializeField] private Sprite imageB = null;

    /**
     * @brief Sets the index of the button.
     */
    public void SetImage(bool useImageA) {

        if(useImageA) {
            imageToControl.sprite = imageA;
        } else {
            imageToControl.sprite = imageB;
        }
    }
}
