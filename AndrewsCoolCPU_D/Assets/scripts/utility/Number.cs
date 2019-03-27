using System;
using UnityEngine;

/**
 * @brief Defines a Number to be used in the simulator.
 * @author Andrew Alford
 * @date 14/03/2019
 * @version 2.1 - 21/03/2019
 */
public class Number
{
    //[DEFUALT] The defualt value of the Number.
    private const int DEFUALT = 0x0000;
    //[value] The current value of the Number.
    private short value = DEFUALT;

    //[overflow] 'true' when the Number is the 
    //result of an overflow.
    private bool overflow = false;
    //[carry] 'true' when the Number is the 
    //result of a carry.
    private bool carry = false;

    /**
     * @brief Constructor for a Number.
     * @param startingValue - The initial value of the Number.
     */
    public Number(int startingValue = DEFUALT)
    {
        //Initialise the number.
        SetNumber(startingValue);
    }

    /**
     * @brief Sets the PSR flags according to the state 
     *        of this Number.
     * @param PSR - The PSR to be set.
     */
    public void SetPSR(ProcessStatusRegister PSR)
    {
        PSR.SetFlag(ProcessStatusRegister.FLAGS.ZERO, IsZero());
        PSR.SetFlag(ProcessStatusRegister.FLAGS.NEGATIVE, IsNegative());
        PSR.SetFlag(ProcessStatusRegister.FLAGS.OVERFLOW, IsOverflow());
        PSR.SetFlag(ProcessStatusRegister.FLAGS.CARRY, IsCarry());
    }

    /**
     * @returns the value as a signed short.
     */
    public short GetSigned()
    {
        return value;
    }

    /**
     * @returns the value as an unsigned short.
     */
    public ushort GetUnsigned()
    {
        return (ushort)value;
    }

    /**
     * @returns the value as a signed string.
     */
    public string GetSignedString()
    {
        return InputValidation.FillBlanks(GetSigned().ToString("X"), 4);
    }

    /**
     * @returns the value as an unsigned string.
     */
    public string GetUnsignedString()
    {
        return InputValidation.FillBlanks(GetUnsigned().ToString("X"), 4);
    }

    /**
     * @returns the value as a signed string in hexadecimal format.
     */
    public string GetHex()
    {
        return InputValidation.FillBlanks(
            value.ToString("X"),
            4
        );
    }

    /**
     * @brief Allows the number to be set.
     * @param value - The value to set the number.
     */
    public void SetNumber(int value)
    {
        this.value = (short)value;
    }

    /**
     * @brief Allowd the number to be set.
     * @param hexValue - The value to set the number.
     */
    public void SetNumber(string hexValue)
    {
        SetNumber(Convert.ToInt16(hexValue, 16));
    }

    /**
     * @brief Resets the number to it's defualt value.
     */
    public void Reset()
    {
        value = DEFUALT;
    }

    /**
     * @returns 'true' if the number is the result of a carry.
     */
    public bool IsCarry()
    {
        return carry;
    }

    /**
     * @brief Allows the carry flag to be set.
     * @param flag - The new value of the carry flag.
     */
    public void SetCarry(bool flag)
    {
        carry = flag;
    }

    /**
     * @returns 'true' if the number is equal to zero.
     */
    public bool IsZero()
    {
        return value == 0x0000;
    }

    /**
     * @returns 'true' if the number is negative.
     */
    public bool IsNegative()
    {
        return value < 0x0000;
    }

    /**
     * @returns 'true' if the number is the result of an overflow or underflow.
     */
    public bool IsOverflow()
    {
        return overflow;
    }

    /**
     * @brief Allows the overflow flag to be set.
     * @param flag - The new value for the overflow flag.
     */
    public void SetOverflow(bool flag)
    {
        overflow = flag;
    }

    /**
     * @brief Adds two Numbers together.
     * @param x - The first Number in the calculation.
     * @param y - The second Number in the calculation.
     */
    public static Number Add(Number x, Number y)
    {
        Number result = new Number(x.value + y.value);

        ushort xUnsigned = (ushort)x.value;
        ushort yUnsigned = (ushort)y.value;

        if(x.value > 0)
        {
            if (x.value > (short.MaxValue - y.value))
            {
                Debug.Log("Addition Overflow");
                result.SetOverflow(true);   //Overflow
            }
        }
        else
        {
            if ((xUnsigned > 0) && (xUnsigned > (ushort.MaxValue - yUnsigned)))
            {
                Debug.Log("Addition Carry");
                result.SetCarry(true);      //Carry
            }

            if ((x.value != 0) && (x.value > (short.MinValue + y.value)))
            {
                Debug.Log("Addition Underflow");
                result.SetOverflow(true);   //Underflow
            }
        }

        return result;
    }

    /**
     * @brief Adds this Number to another Number.
     * @param otherNumber - The Number to add to this Number.
     */
    public void Add(Number otherNumber)
    {
        Clone(Add(this, otherNumber));
    }

    /**
     * @brief Subtracts one Number from another Number.
     * @param x - The first Number in the calculation.
     * @param y - The second Number in the calculation.
     */
    public static Number Subtract(Number x, Number y)
    {
        Number result = new Number(x.value - y.value);

        if ((x.value > 0) && (x.value > (short)(short.MaxValue + y.value)))
        {
            Debug.Log("Subtraction Overflow");
            result.SetOverflow(true); //Overflow
        }
        if ((x.value < 0) && (x.value < (short.MinValue + y.value)))
        {
            Debug.Log("Subtraction Underflow");
            result.SetOverflow(true); //Underflow
        }

        return result;
    }

    /**
     * @brief Subtracts this Number from another Number.
     * @param otherNumber - The Number to be subtracted from this Number.
     */
    public void Subtract(Number otherNumber)
    {
        Clone(Subtract(this, otherNumber));
    }

    /**
     * @brief Multiplies two Numbers together.
     * @param x - The first Number in the calculation.
     * @param y - The second Number in the calculation.
     */
    public static Number Multiply(Number x, Number y)
    {
        Number result = new Number(x.value * y.value);

        if(x.value > short.MaxValue / y.value)
        {
            Debug.Log("multiplication overflow");
            result.SetOverflow(true);
        }
        if(x.value < short.MinValue / y.value)
        {
            Debug.Log("multiplication underflow");
            result.SetOverflow(true);
        }
        if ((ushort)x.value > (ushort)(ushort.MaxValue / y.value))
        {
            Debug.Log("multiplication carry");
            result.SetCarry(true);
        }

        return result;
    }

    /**
     * @brief Multiplies this Numuber with another Number.
     * @param otherNumber - The Number to be multiplied with this Number.
     */
    public void Multiply(Number otherNumber)
    {
        Clone(Multiply(this, otherNumber));
    }

    /**
     * @brief Divides two Numbers.
     * @param x - The first Number in the calculation.
     * @param y - The second Number in the calculation.
     */
    public static Number Divide(Number x, Number y)
    {
        return new Number(x.value / y.value);
    }

    /**
     * @brief Divides this Number by another Number.
     * @param otherNumber - The Number to divide this Number with.
     */
    public void Divide(Number otherNumber)
    {
        Clone(Divide(this, otherNumber));
    }

    /**
     * @brief Copies the contents of a Number into this Number
     * @param otherNumber - The Number to clone from.
     */
    public void Clone(Number otherNumber)
    {
        SetNumber(otherNumber.value);
        SetOverflow(otherNumber.IsOverflow());
        SetCarry(otherNumber.IsCarry());
    }

    /**
     * @reutrns all the information about this Number 
     *          in string format.
     */
    public override string ToString()
    {
        return  "\n" +
                "hex:\t"        + GetHex()      + "\n" +
                "signed:\t"     + GetSigned()   + "\n" +
                "unsigned:\t"   + GetUnsigned() + "\n" +
                "Carry:\t"      + IsCarry()     + "\n" +
                "Zero:\t"       + IsZero()      + "\n" +
                "Negative:\t"   + IsNegative()  + "\n" +
                "Overflow:\t"   + IsOverflow()  + "\n";
    }
}

