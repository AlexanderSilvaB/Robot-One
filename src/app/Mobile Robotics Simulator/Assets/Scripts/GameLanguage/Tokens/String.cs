using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class String : Token
{
    public string Value;
    public String(string value) : base(Types.String)
    {
        Value = value;
    }

    public override string ToString()
    {
        return "[String] (Value = " + Value + ")";
    }
}