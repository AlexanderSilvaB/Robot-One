using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    public Dictionary<string, object> Variables;

    public Context()
    {
        Variables = new Dictionary<string, object>();
    }
}
