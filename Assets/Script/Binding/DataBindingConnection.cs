using System;
using System.Reflection;
using UnityEngine;

internal class DataBindingConnection
{
    private DataBindInfo dataBindInfo;
    private BaseViewModel viewModel;

    PropertyInfo _getProperty;
    MethodInfo _invokeMethod;

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
    }

    void OnChange(string changedProperty) 
    {
        if (changedProperty != dataBindInfo.propertyName)
        {
            return;
        }

        object data = _getProperty.GetValue(viewModel);
        _invokeMethod.Invoke(null, new object[] { dataBindInfo.component, data});
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