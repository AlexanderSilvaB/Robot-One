using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable : Token
{
    public string Name;
    public Variable(string name) : base(Types.Variable)
    {
        Name = name;
    }

    public override string ToString()
    {
        return "[Variable] (Name = " + Name + ")";
    }
}
