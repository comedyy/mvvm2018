using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public class ToggleEventBindingConnection : EventBindingConnection<Toggle>
{
    public ToggleEventBindingConnection(BaseViewModel viewModel, EventBindInfo eventBindInfo) : base(viewModel, eventBindInfo)
    {
    }

    private void OnValueChange(bool arg0)
    {
        if (_invokeMethod is PropertyInfo info1)
        {
            info1.SetValue(viewModel, arg0 );
        }
        else if (_invokeMethod is MethodInfo info)
        {
            info.Invoke(viewModel, new object[] { arg0 });
        }
    }

    public override void Bind()
    {
        Component.onValueChanged.AddListener(OnValueChange);
    }

    public override void UnBind()
    {
        Component.onValueChanged.RemoveListener(OnValueChange);
    }

    protected override Type[] GetMethodParams()
    {
        return new Type[] { typeof(bool) };
    }

    protected override Type GetPropertyType()
    {
        return typeof(bool);
    }
}
