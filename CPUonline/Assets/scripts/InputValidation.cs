using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class InputValidation : MonoBehaviour
{
    public InputField[] registers;

    //A regular expression for hexadecimal characters.
    private Regex hexCharacters = new Regex("([A-F]|[0-9])");

    /**
     * @brief Initialises the script
     */
    void Start()
    {
        //Delagate a method to validate register input fields.
        for(int i = 0; i < registers.Length; i++)
        {
            registers[i].onValidateInput += delegate (string input, int charIndex, char addedChar) { return validateAsHex(addedChar); };
        }
    }

    private char validateAsHex(char charToValidate)
    {
        //[character] Holds the char as a string.
        string character = charToValidate.ToString();

        //Convert the character to upper case.
        character = character.ToUpper();

        //Check the character against the predefined regular expression.
        if (hexCharacters.IsMatch(character))
        {
            //Return the correct character.
            charToValidate = character.ToCharArray()[0];
        }
        else
        {
            //If there is no match return an empty character.
            charToValidate = '\0';
        }

        return charToValidate;
    }

    /**
     * @brief Fills in the blank text on a register input field.
     * @param register - The register being checked.
     */
    public void fillBlanks_Register(InputField register)
    {
        //If not enough characters have been entered...
        if (register.text.Length < register.characterLimit)
        {
            //[blanks] The filler text for the input field. 
            //For registers these will be 0's
            string blanks = "";

            for (int i = 0; i < (register.characterLimit - register.text.Length); i++)
            {
                blanks += "0";
            }

            //Update the text.
            register.text = blanks += register.text;
        }
    }
}