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

        _getProperty = viewModel.GetType().GetProperty(dataBindInfo.propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (_getProperty == null)
        {
            Debug.LogErrorFormat("get property null {0}:{1}", viewModel.GetType(), dataBindInfo.propertyName);
        }

        _invokeMethod = typeof(UIBindingFunctions).GetMethod(dataBindInfo.invokeFunctionName, BindingFlags.Public | BindingFlags.Static);
        if (_invokeMethod == null)
        {
            Debug.LogErrorFormat("get invokeMethod null {0}", dataBindInfo.invokeFunctionName);
        }

        if (_invokeMethod.IsGenericMethod)
        {
            _invokeMethod = _invokeMethod.MakeGenericMethod(new Type[] { _getProperty.PropertyType });
        }
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