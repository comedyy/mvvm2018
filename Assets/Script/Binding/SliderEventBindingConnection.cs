using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
public class SliderEventBindingConnection : EventBindingConnection<Slider>
{
    public SliderEventBindingConnection(BaseViewModel viewModel, EventBindInfo eventBindInfo) : base(viewModel, eventBindInfo)
    {
    }

    private void OnValueChange(float arg0)
    {
        if (_invokeMethod is PropertyInfo)
        {
            ((PropertyInfo)_invokeMethod).SetValue(viewModel, arg0);
        }
        else if (_invokeMethod is MethodInfo)
        {
            ((MethodInfo)_invokeMethod).Invoke(viewModel, new object[] { arg0 });
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
}
