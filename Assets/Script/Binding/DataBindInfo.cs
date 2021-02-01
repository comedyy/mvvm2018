using System;
using UnityEngine;

public enum DataFrom 
{
    VM,                 // vm
    Custom,             // 自定义
    MultiLanguage       // 多语言
}

public enum ParameterType 
{
    INT,
    STR,
    FLOAT,
    BOOL
}

public class BindingParameter 
{
    public DataFrom dataFrom;
    public ParameterType paramType;
    public string paramStr;
    public int paramInt;
}

[Serializable]
public class DataBindInfo
{
    public Component component;
    public string invokeFunctionName;
    public string propertyName;

    public BindingParameter[] parameters;
}

[Serializable]
public class EventBindInfo
{
    public Component component;
    public string className;
    public string invokeFunctionName;
}