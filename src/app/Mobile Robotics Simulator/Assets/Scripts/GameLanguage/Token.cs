using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token
{
    public enum Types { None, Variable, String, Number, Assign };

    private Types type;
    public Types Type
    {
        get
        {
            return type;
        }
    }

    protected Token(Types type)
    {
        this.type = type;
    }

    public T Get<T>()
    {
        return (T)Convert.ChangeType(this, typeof(T));
    }

    public override string ToString()
    {
        return "[Token] (Type = " + type.ToString() + ")";
    }
}
