using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/**
 * @brief   Utility class for validating user input.
 * @extends MonoBehaviour
 * @author  Andrew Alford
 * @date    19/02/2019
 * @version 2.1 - 02/04/2019
 */
public class InputValidation : MonoBehaviour {

    //[hexCharacters] A regular expression for hexadecimal characters.
    static private Regex hexCharacters = new Regex("([A-F]|[0-9])");
    //[hexString] A regular expression to validate hex strings.
    static private Regex hexString = new Regex("\\$([\\da-fA-F]{1,4})");
    //[decimalString] A regualar expression to validate decimal strings.
    static private Regex decimalString = new Regex("\\^(\\d)");

    /**
     * @brief Checks if a given char is a hexadeciamal value.
     *        (Also forces the character into upper-case).
     * @param charToValidate - This is the char to be validated.
     * @return Returns an empty character if the char is valid, 
     *         otherwise returns an empty character.
     */
    static public char ValidateAsHex(char charToValidate) {
        //[character] Holds the char as a string.
        string character = charToValidate.ToString();

        //Convert the character to upper case.
        character = character.ToUpper();

        //Check the character against the predefined regular expression.
        if (hexCharacters.IsMatch(character)) {
            //Return the correct character.
            charToValidate = character.ToCharArray()[0];
        } else {
            //If there is no match return an empty character.
            charToValidate = '\0';
        }
        return charToValidate;
    }

    /**
     * @brief Fills in the blank text on a register input field.
     * @param register - The register being checked.
     */
    static public void FillBlanks_Register(InputField register) {
        //If not enough characters have been entered...
        if (register.text.Length < register.characterLimit) {
            //[blanks] The filler text for the input field. 
            //For registers these will be 0's
            string blanks = "";

            for (int i = 0; i < (register.characterLimit - register.text.Length); i++) {
                blanks += "0";
            }

            //Update the text.
            register.text = blanks += register.text;
        }
    }

    /**
     * @brief Fills in the blanks with 0's to represent hex values.
     * @param content - This is the content to be filled.
     * @param characterLimit - How many characters the content should have.
     * @returns The filled content. E.g., "A" would become "000A".
     */
    static public string FillBlanks(string content, uint characterLimit) {
        //Fill in any blank spaces with 0's.
        if (content.Length < characterLimit) {
            string blankSpaces = "";
            for (int i = 0; i < (characterLimit - content.Length); i++) {
                blankSpaces += "0";
            }
            content = blankSpaces += content;
        }
        return content;
    }

    /**
     * @brief Validates a given string as hexidecimal.
     * @param content - The string to be validated.
     */
    static public bool IsHex(string content) {
        return hexString.IsMatch(content);
    }

    /**
     * @brief Validates a given string as decimal.
     * @param content - The string to be validated.
     */
    static public bool IsDecimal(string content) {
        return decimalString.IsMatch(content);
    }

    /**
     * @brief Converts a decimal string to a hexidecimal string.
     * @param decimalContent - The value to be converted.
     * @returns the hexidecimal equivelent to the decimal value given.
     */
    static public string DecimalToHex(string decimalContent) {
        //Convert the string to a number.
        Number number = new Number(int.Parse(decimalContent));

        return number.GetHex();
    }
}