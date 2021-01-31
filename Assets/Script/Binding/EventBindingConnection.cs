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

        MemberInfo[] infos = viewModel.GetType().GetMember(eventBindInfo.invokeFunctionName, BindingFlags.Public | BindingFlags.Instance);
        if (infos == null || infos.Length == 0)
        {
            Debug.LogErrorFormat("get invokeMethod null {0}", eventBindInfo.invokeFunctionName);
            return;
        }

        _invokeMethod = infos[0];
    }

    public abstract void Bind();
    public abstract void UnBind();
}