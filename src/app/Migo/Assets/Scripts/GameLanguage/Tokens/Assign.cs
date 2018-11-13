using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assign : Token
{
    public Variable Var;
    public Token Value;

    public Assign(Variable var, Token value) : base(Types.Assign)
    {
        Var = var;
        Value = value;
    }

    public override string ToString()
    {
        return "[Assign] (Var = " + Var.Name + ", Value = " + Value.ToString() + ")";
    }
}