using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter
{
    private int hashCode = 0;
    private List<Token> tokens;

    public Interpreter()
    {
        tokens = new List<Token>();
    }

    public void Compile(string code)
    {
        int hashCode = code.GetHashCode();
        if (hashCode == this.hashCode)
            return;
        
        this.hashCode = hashCode;
        string[] lines = code.Split('\n');
        for(int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            line = TrimAll(line, ' ', '\t', '\r');
            string[] parts = line.Split('=');
            Variable var = new Variable(parts[0]);
            Token value;
            if((parts[1][0] >= '0' && parts[1][0] <= '9') || parts[1][0] == '+' || parts[1][0] == '-')
            {
                value = new Number(float.Parse(parts[1]));
            }
            else if(parts[1][0] == '"')
            {
                value = new String(parts[1].Substring(1, parts[1].Length - 2));
            }
            else
            {
                value = new Variable(parts[1]);
            }
            tokens.Add(new Assign(var, value));
        }
    }

    public void Execute(Context context)
    {
        for(int i = 0; i < tokens.Count; i++)
        {
            Token token = tokens[i];
            switch(token.Type)
            {
                case Token.Types.Assign:
                {
                    Assign assign = token.Get<Assign>();
                    if(assign.Value.Type == Token.Types.Number)
                    {
                        context.Variables[assign.Var.Name] = assign.Value.Get<Number>().Value;
                    }
                }
                break;
                default:
                {

                }
                break;
            }
        }
    }

    private string TrimAll(string text, params char[] chars)
    {
        for(int i = 0; i < chars.Length; i++)
        {
            while(text.IndexOf(chars[i]) >= 0)
            {
                text = text.Remove(text.IndexOf(chars[i]), 1);
            }
        }
        return text;
    }
}
