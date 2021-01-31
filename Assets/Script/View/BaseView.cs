using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseView<T> : MonoBehaviour where T : BaseViewModel
{
    public T ViewModel { get; protected set; }
    /*[SerializeField]*/ public DataBindInfo[] dataBindConfigs;
    /*[SerializeField]*/ public EventBindInfo[] eventBindConfigs;

    DataBindingConnection[] _data_connections;
    EventBindingConnection[] _event_connections;


    private void OnEnable()
    {
        Bind();
    }

    private void OnDisable()
    {
        UnBind();
    }

    private void Bind()
    {
        if (_data_connections == null)
        {
            _data_connections = new DataBindingConnection[dataBindConfigs.Length];
            for (int i = 0; i < _data_connections.Length; i++)
            {
                _data_connections[i] = new DataBindingConnection(ViewModel, dataBindConfigs[i]);
                _data_connections[i].Bind();
            }
        }

        if (_event_connections == null)
        {
            _event_connections = new EventBindingConnection[eventBindConfigs.Length];
            for (int i = 0; i < _event_connections.Length; i++)
            {
                _event_connections[i] = GetConnectionByComponent(eventBindConfigs[i]);
                _event_connections[i].Bind();
            }
        }
    }

    private void UnBind()
    {
        for (int i = 0; i < _data_connections.Length; i++)
        {
            _data_connections[i].UnBind();
        }

        for (int i = 0; i < _event_connections.Length; i++)
        {
            _event_connections[i].UnBind();
        }
    }

    EventBindingConnection GetConnectionByComponent(EventBindInfo config) 
    {
        if (config.component is Button)
        {
            return new ButtonEventBindingConnection(ViewModel, config);
        }
        else if (config.component is Toggle)
        {
            return new ToggleEventBindingConnection(ViewModel, config);
        }
        else if (config.component is Slider)
        {
            return new SliderEventBindingConnection(ViewModel, config);
        }

        return null;
    }
}
