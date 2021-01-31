using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEventBindingConnection : EventBindingConnection<Button>
{
    public ButtonEventBindingConnection(BaseViewModel viewModel, EventBindInfo eventBindInfo) : base(viewModel, eventBindInfo)
    {
    }

    private void OnClick()
    {
        ((MethodInfo)_invokeMethod).Invoke(viewModel, null);
    }

    public override void Bind()
    {
        Component.onClick.AddListener(OnClick);
    }

    public override void UnBind()
    {
        Component.onClick.RemoveListener(OnClick);
    }
}
