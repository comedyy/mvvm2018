using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class EventBindingConnection<T> : EventBindingConnection where T : Component
{
    protected T Component;

    public EventBindingConnection(BaseViewModel viewModel, EventBindInfo eventBindInfo) : base (viewModel, eventBindInfo)
    {
        Component = (T)eventBindInfo.component;
    }
}

public abstract class EventBindingConnection
{
    private EventBindInfo eventBindInfo;
    protected BaseViewModel viewModel;

    protected MemberInfo _invokeMethod;

    public EventBindingConnection(BaseViewModel viewModel, EventBindInfo eventBindInfo)
    {
        this.eventBindInfo = eventBindInfo;
        this.viewModel = viewModel;

        _invokeMethod = ReflectionTool.GetVmMethod(viewModel.GetType(), eventBindInfo.invokeFunctionName, GetMethodParams());
        if (_invokeMethod == null)
        {
            PropertyInfo info = ReflectionTool.GetVmPropertyByName(viewModel.GetType(), eventBindInfo.invokeFunctionName);
            if (info != null )
            {
                if (info.PropertyType != GetPropertyType())
                {
                    Debug.LogErrorFormat("get invokeMethod null {0}", eventBindInfo.invokeFunctionName);
                }
                else 
                {
                    _invokeMethod = info;
                }
            }
        }

        if (_invokeMethod == null)
        {
            Debug.LogErrorFormat("get invokeMethod null {0}", eventBindInfo.invokeFunctionName);
            return;
        }
    }

    public abstract void Bind();
    public abstract void UnBind();

    protected abstract Type[] GetMethodParams();
    protected abstract Type GetPropertyType();
}