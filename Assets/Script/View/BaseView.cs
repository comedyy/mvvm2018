using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseView<T> : MonoBehaviour where T : BaseViewModel
{
    [SerializeField] protected T ViewModel;
    public CombineRuntime _combineInstance;

    private void OnEnable()
    {
        _combineInstance.Init(ViewModel);
        _combineInstance.Bind();
    }

    private void OnDisable()
    {
        _combineInstance.UnBind();
    }
}
