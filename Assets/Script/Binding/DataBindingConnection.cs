using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

class BindingParameterInfo
{
    PropertyInfo propertyInfo;
    object constValue;
    BaseViewModel model;

    public BindingParameterInfo(BindingParameter paramter, BaseViewModel model)
    {
        this.model = model;
        if (paramter.dataFrom == DataFrom.VM)
        {
            propertyInfo = ReflectionTool.GetVmPropertyByName(model.GetType(), paramter.paramStr);
        }
        else
        {
            Type t = Type.GetType(paramter.paramType);
            constValue = Convert.ChangeType(paramter.paramStr, t);
        }
    }

    public object GetValue()
    {
        return propertyInfo != null ? propertyInfo.GetValue(model) : constValue;
    }
}

internal class DataBindingConnection
{
    private DataBindInfo dataBindInfo;
    private BaseViewModel viewModel;

    PropertyInfo _getProperty;
    MethodInfo _invokeMethod;

    List<BindingParameterInfo> _lstParameter = new List<BindingParameterInfo>();
    object[] ps;

    public DataBindingConnection(BaseViewModel viewModel, DataBindInfo dataBindInfo)
    {
        this.dataBindInfo = dataBindInfo;
        this.viewModel = viewModel;


        _getProperty = ReflectionTool.GetVmPropertyByName(viewModel.GetType(), dataBindInfo.propertyName);
        if (_getProperty == null)
        {
            Debug.LogErrorFormat("get property null {0}:{1}", viewModel.GetType(), dataBindInfo.propertyName);
        }
     
        ReflectionMethodItem item = ReflectionTool.GetComponentMethod(dataBindInfo.component.GetType(), dataBindInfo.invokeFunctionName, _getProperty.PropertyType);
        if (item == null)
        {
            Debug.LogErrorFormat("get invokeMethod null {0}", dataBindInfo.invokeFunctionName);
        }

        _invokeMethod = item.methodInfo;

        foreach (var bindingParameter in dataBindInfo.parameters)
        {
            _lstParameter.Add(new BindingParameterInfo(bindingParameter, viewModel));
        }

        ps = new object[dataBindInfo.parameters.Length + 2];
        ps[0] = dataBindInfo.component;
    }

    void OnChange(string changedProperty) 
    {
        if (changedProperty != dataBindInfo.propertyName)
        {
            return;
        }

        object data = _getProperty.GetValue(viewModel);
        ps[1] = data;
        for (int i = 0; i < _lstParameter.Count; i++)
        {
            ps[2 + i] = _lstParameter[i].GetValue();
        }

        _invokeMethod.Invoke(null,  ps);
    }

    internal void Bind()
    {
        viewModel.Callback += OnChange;
        OnChange(dataBindInfo.propertyName);
    }

    internal void UnBind()
    {
        viewModel.Callback -= OnChange;
    }
}