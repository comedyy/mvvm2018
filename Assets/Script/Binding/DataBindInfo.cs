using System;
using UnityEngine;

[Serializable]
public class DataBindInfo
{
    public Component component;
    public string invokeFunctionName;
    public string propertyName;
}

[Serializable]
public class EventBindInfo
{
    public Component component;
    public string className;
    public string invokeFunctionName;
}