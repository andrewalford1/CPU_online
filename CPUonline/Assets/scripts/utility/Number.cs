using UnityEngine;

/**
 * @brief Defines a Number to be used in the simulator.
 * @author Andrew Alford
 * @date 14/03/2019
 * @version 2.0 - 18/03/2019
 */
public class Number
{
    private const int DEFUALT = 0x0000;

    private short value = DEFUALT;

    private bool overflow = false;
    private bool carry = false;

    public Number(int startingValue = DEFUALT)
    {
        //Initialise the number.
        SetNumber(startingValue);
    }

    public void SetPSR(ProcessStatusRegister PSR)
    {
        PSR.SetFlag(ProcessStatusRegister.FLAGS.ZERO, IsZero());
        PSR.SetFlag(ProcessStatusRegister.FLAGS.NEGATIVE, IsNegative());
        PSR.SetFlag(ProcessStatusRegister.FLAGS.OVERFLOW, IsOverflow());
        PSR.SetFlag(ProcessStatusRegister.FLAGS.CARRY, IsCarry());
    }

    public short GetSigned()
    {
        return value;
    }

    public ushort GetUnsigned()
    {
        return (ushort)value;
    }

    public string GetHex()
    {
        return InputValidation.FillBlanks(
            value.ToString("X"),
            4
        );
    }

    public void SetNumber(int value)
    {
        this.value = (short)value;
    }

    public void SetNumber(string hexValue)
    {
        SetNumber(System.Convert.ToInt16(hexValue, 16));
    }

    public void Reset()
    {
        value = DEFUALT;
    }

    public bool IsCarry()
    {
        return carry;
    }

    public void SetCarry(bool flag)
    {
        carry = flag;
    }

    public bool IsZero()
    {
        return value == 0x0000;
    }

    public bool IsNegative()
    {
        return value < 0x0000;
    }

    public bool IsOverflow()
    {
        return overflow;
    }

    public void SetOverflow(bool flag)
    {
        overflow = flag;
    }

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

            if (x.value > (short.MinValue + y.value))
            {
                Debug.Log("Addition Underflow");
                result.SetOverflow(true);   //Underflow
            }
        }

        return result;
    }

    public void Add(Number otherNumber)
    {
        Clone(Add(this, otherNumber));
    }

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

    public void Subtract(Number otherNumber)
    {
        Clone(Subtract(this, otherNumber));
    }

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

    public void Multiply(Number otherNumber)
    {
        Clone(Multiply(this, otherNumber));
    }

    public static Number Divide(Number x, Number y)
    {
        return new Number(x.value / y.value);
    }

    public void Divide(Number otherNumber)
    {
        Clone(Divide(this, otherNumber));
    }

    public void Clone(Number otherNumber)
    {
        SetNumber(otherNumber.value);
        SetOverflow(otherNumber.IsOverflow());
        SetCarry(otherNumber.IsCarry());
    }

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

