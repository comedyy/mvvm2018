using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;



public static class EditorPropertyCache
{
    static Dictionary<Type, List<MethodInfo>> _dicComponentFuncs;
    static Dictionary<string, List<PropertyInfo>> _dicViewModelPropertys;
    static Dictionary<string, List<MethodInfo>> _dicViewModelMethods;

    static EditorPropertyCache() 
    {
        InitComponentFuncs();
        InitViewModelPropertys();
    }

    static void InitViewModelPropertys() 
    {
        if (_dicViewModelPropertys != null)
        {
            return;
        }

        _dicViewModelPropertys = new Dictionary<string, List<PropertyInfo>>();
        _dicViewModelMethods = new Dictionary<string, List<MethodInfo>>();

        Type[] types = typeof(BaseViewModel).Assembly.GetTypes();
        foreach (var type in types)
        {
            if (!type.IsSubclassOf(typeof(BaseViewModel)))
            {
                continue;
            }

            string name = type.Name;
            if (!_dicViewModelMethods.TryGetValue(name, out List<MethodInfo> lstMetheds))
            {
                lstMetheds = new List<MethodInfo>();
                _dicViewModelMethods.Add(name, lstMetheds);
            }

            if (!_dicViewModelPropertys.TryGetValue(name, out List<PropertyInfo> lstPropertys))
            {
                lstPropertys = new List<PropertyInfo>();
                _dicViewModelPropertys.Add(name, lstPropertys);
            }

            MethodInfo[] members = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m=>!m.IsSpecialName).ToArray();
            foreach (var member in members)
            {
                lstMetheds.Add(member);
            }

            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).ToArray();
            foreach (var property in propertys)
            {
                lstPropertys.Add(property);
            }
        }
    }

    static void InitComponentFuncs()
    {
        if (_dicComponentFuncs != null)
        {
            return;
        }

        _dicComponentFuncs = new Dictionary<Type, List<MethodInfo>>();

        MethodInfo[] infos = typeof(UIBindingFunctions).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var info in infos)
        {
            ParameterInfo[] ps = info.GetParameters();
            if (ps.Length < 2)
            {
                continue;
            }

            Type t = ps[0].ParameterType;
            if (!_dicComponentFuncs.TryGetValue(t, out List<MethodInfo> list))
            {
                list = new List<MethodInfo>();
                _dicComponentFuncs.Add(t, list);
            }

            list.Add(info);
        }
    }

    public static List<string> GetComponentOpts(Type type)
    {
        List<string> lstFunc = new List<string>();
        foreach (var key in _dicComponentFuncs.Keys)
        {
            if (key.IsAssignableFrom(type))
            {
                lstFunc.AddRange(_dicComponentFuncs[key].Select(m=>m.Name)); ;
            }
        }

        return lstFunc;
    }

    public static List<MethodInfo> GetMethodInfo(Type compType, string method) 
    {
        List<MethodInfo> lstMethod = new List<MethodInfo>();
        foreach (var key in _dicComponentFuncs.Keys)
        {
            if (key.IsAssignableFrom(compType))
            {
                foreach (var item in _dicComponentFuncs[key])
                {
                    if (item.Name == method)
                    {
                        lstMethod.Add(item);
                    }
                }
            }
        }

        return lstMethod;
    }

    public static List<string> GetPropertys(string type, Type componentType, string methodName) 
    {
        List<MethodInfo> methodInfos = GetMethodInfo(componentType, methodName);

        List<string> lstProperty = new List<string>();
        if (!_dicViewModelPropertys.TryGetValue(type, out List<PropertyInfo> lstPropertys)) 
        {
            return new List<string>();
        }
  
        foreach (var property in lstPropertys)
        {
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.IsGenericMethod)
                {
                    try
                    {  // TODO:ѡ����õķ�ʽ
                        MethodInfo makeMethod = methodInfo.MakeGenericMethod(new Type[] { property.PropertyType });
                        if (makeMethod != null)
                        {
                            lstProperty.Add(property.Name);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    Type argType = methodInfo.GetParameters()[1].ParameterType;
                    if (argType.IsAssignableFrom(property.PropertyType))
                    {
                        lstProperty.Add(property.Name);
                    }
                }
            }
        }

        return lstProperty;
    }

    public static List<string> GetMethodAndPropertys(string viewModelType, Type componentType)
    {
        if (!_dicViewModelMethods.TryGetValue(viewModelType, out List<MethodInfo> lstMethods))
        {
            return new List<string>();
        }

        bool exist = GetArgumentByComponentEvent(componentType, out Type argType);
        if (!exist)
        {
            return new List<string>();
        }

        List<string> lstRet = new List<string>();
        foreach (var item in lstMethods)
        {
            var ts = item.GetParameters();
            if (ts.Length == 0 && argType == typeof(void))
            {
                lstRet.Add(item.Name);
            }
            else if (ts.Length == 1 && ts[0].ParameterType.IsAssignableFrom(argType))
            {
                lstRet.Add(item.Name);
            }
        }

        if (_dicViewModelPropertys.TryGetValue(viewModelType, out List<PropertyInfo> lstPropertys))
        {
            foreach (var item in lstPropertys)
            {
                if (item.PropertyType.IsAssignableFrom(argType))
                {
                    lstRet.Add(item.Name);
                }
            }
        }

        return lstRet;
    }

    private static bool GetArgumentByComponentEvent(Type componentType, out Type type)
    {
        type = typeof(void);
        if (componentType == typeof(Slider))
        {
            type = typeof(float);
            return true;
        }
        else if (componentType == typeof(Toggle))
        {
            type = typeof(bool);
            return true;
        }
        else if (componentType == typeof(Button))
        {
            return true;
        }
        
        return false;
    }
}
