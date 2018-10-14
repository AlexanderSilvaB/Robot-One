using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : Token
{
    public float Value;
    public Number(float value) : base(Types.Number)
    {
        Value = value;
    }

    public override string ToString()
    {
        return "[Number] (Value = " + Value + ")";
    }
}