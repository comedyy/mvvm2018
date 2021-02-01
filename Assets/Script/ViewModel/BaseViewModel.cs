using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseViewModel 
{
    public Action<string> Callback;

    protected void OnPropertyChange(string name) 
    {
        Callback?.Invoke(name);
    }
}
