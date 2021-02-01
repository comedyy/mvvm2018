using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ReflectionMethodItem 
{
    public string Name;
    public Type[] parameters;
    public MethodInfo methodInfo;
}

class ReflectionPropertyItem 
{
    public string Name;
    public Type propertyType;
    public PropertyInfo propertyInfo;
}

public static class ReflectionTool
{
    static Dictionary<Type, List<ReflectionMethodItem>> _dicComponentOpts = new Dictionary<Type, List<ReflectionMethodItem>>();
    static Dictionary<Type, List<ReflectionPropertyItem>> _dicVMProperty = new Dictionary<Type, List<ReflectionPropertyItem>>();
    static Dictionary<Type, List<ReflectionMethodItem>> _dicVMMethods = new Dictionary<Type, List<ReflectionMethodItem>>();

    static void InitViewModelPropertys(Type vmType)
    {
        if (!vmType.IsSubclassOf(typeof(BaseViewModel)))
        {
            Debug.LogError("vmType not BaseViewModel");
            return;
        }

        if (!_dicVMMethods.TryGetValue(vmType, out List<ReflectionMethodItem> lstMetheds))
        {
            lstMetheds = new List<ReflectionMethodItem>();
            _dicVMMethods.Add(vmType, lstMetheds);
        }

        if (!_dicVMProperty.TryGetValue(vmType, out List<ReflectionPropertyItem> lstPropertys))
        {
            lstPropertys = new List<ReflectionPropertyItem>();
            _dicVMProperty.Add(vmType, lstPropertys);
        }

        MethodInfo[] members = vmType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).ToArray();
        foreach (var member in members)
        {
            ReflectionMethodItem item = new ReflectionMethodItem()
            {
                methodInfo = member,
                Name = member.Name,
                parameters = member.GetParameters().Select(m => m.ParameterType).ToArray()
            };

            lstMetheds.Add(item);
        }

        PropertyInfo[] propertys = vmType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).ToArray();
        foreach (var property in propertys)
        {
            ReflectionPropertyItem item = new ReflectionPropertyItem()
            {
                propertyInfo = property,
                Name = property.Name,
                propertyType = property.PropertyType
            };

            lstPropertys.Add(item);
        }
    }

    static List<ReflectionMethodItem> InitComponentFuncs(Type componentType)
    {
        if (_dicComponentOpts.ContainsKey(componentType))
        {
            return null;
        }

        MethodInfo[] infos = typeof(UIBindingFunctions).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var info in infos)
        {
            ParameterInfo[] ps = info.GetParameters();
            if (ps.Length < 2)
            {
                continue;
            }

            Type t = ps[0].ParameterType;
            if (!_dicComponentOpts.TryGetValue(t, out List<ReflectionMethodItem> list))
            {
                list = new List<ReflectionMethodItem>();
                _dicComponentOpts.Add(t, list);
            }

            ReflectionMethodItem item = new ReflectionMethodItem()
            {
                methodInfo = info,
                Name = info.Name,
                parameters = info.GetParameters().Select(m=>m.ParameterType).ToArray()
            };

            list.Add(item);
        }

        _dicComponentOpts.TryGetValue(componentType, out List<ReflectionMethodItem> lst);
        return lst;
    }

    static List<ReflectionMethodItem> GetComponentMethods(Type compType) 
    {
        if (_dicComponentOpts.TryGetValue(compType, out List<ReflectionMethodItem> lst))
        {
            return lst;
        }

        return InitComponentFuncs(compType);
    }

    static List<ReflectionPropertyItem> GetVmPropertys(Type vmType) 
    {
        if (_dicVMProperty.TryGetValue(vmType, out List<ReflectionPropertyItem> lst))
        {
            return lst;
        }

        InitViewModelPropertys(vmType);

        return _dicVMProperty[vmType];
    }

    static List<ReflectionMethodItem> GetVmMethods(Type vmType)
    {
        if (_dicVMMethods.TryGetValue(vmType, out List<ReflectionMethodItem> lst))
        {
            return lst;
        }

        InitViewModelPropertys(vmType);

        return _dicVMMethods[vmType];
    }

    public static void GetComponentMethods(Type type, List<string> methods)
    {
        foreach (var key in _dicComponentOpts.Keys)
        {
            if (key.IsAssignableFrom(type))
            {
                methods.AddRange(_dicComponentOpts[key].Select(m => m.Name)); ;
            }
        }
    }

    public static ReflectionMethodItem GetComponentMethod(Type compType, string method, Type parameterType) 
    {
        // load component
        GetComponentMethods(compType);

        foreach (var keyvalue in _dicComponentOpts)
        {
            if (!keyvalue.Key.IsAssignableFrom(compType))
            {
                continue;
            }

            foreach (var item in keyvalue.Value)
            {
                if (item.Name == method && item.parameters[1].IsAssignableFrom(parameterType))
                {
                    return item;
                }
            }
        }

        return null;
    }

    public static void GetVmPropertysByMethod(Type vmType, Type componentType, string methodName, List<string> propertys)
    {
        List<ReflectionPropertyItem> lstProperty = GetVmPropertys(vmType);
        if (lstProperty == null)
        {
            return;
        }

        foreach (var item in lstProperty)
        {
            if (GetComponentMethod(componentType, methodName, item.propertyType) != null)
            {
                propertys.Add(item.Name);
            }
        }
    }

    public static void GetVmPropertyByType(Type vmType, Type type, List<string> lst)
    {
        List<ReflectionPropertyItem> lstProperty = GetVmPropertys(vmType);
        if (lstProperty == null)
        {
            return;
        }

        foreach (var item in lstProperty)
        {
            if (item.propertyType.IsAssignableFrom(type))
            {
                lst.Add(item.Name);
            }
        }
    }

    public static PropertyInfo GetVmPropertyByName(Type vmType, string name) 
    {
        List<ReflectionPropertyItem> lstProperty = GetVmPropertys(vmType);
        if (lstProperty == null)
        {
            return null;
        }

        foreach (var item in lstProperty)
        {
            if (item.Name == name)
            {
                return item.propertyInfo;
            }
        }

        return null;
    }

    public static void GetVmMethodByParameter1Type(Type vmType, Type type, List<string> lst) 
    {
        List<ReflectionMethodItem> lstMethod = GetVmMethods(vmType);
        if (lstMethod == null)
        {
            return;
        }

        foreach (var item in lstMethod)
        {
            if (item.parameters.Length == 0)
            {
                if (type == typeof(void))
                {
                    lst.Add(item.Name);
                }
            }
            else if(item.parameters.Length > 0)
            {
                if (item.parameters[0].IsAssignableFrom(type))
                {
                    lst.Add(item.Name);
                }
            }
        }
    }

    public static MethodInfo GetVmMethod(Type vmType, string methodName, Type[] parameterTypes ) 
    {
        List<ReflectionMethodItem> lstMethod = GetVmMethods(vmType);
        if (lstMethod == null)
        {
            return null;
        }

        foreach (var item in lstMethod)
        {
            if (item.Name != methodName )
            {
                continue;
            }

            bool allSame = true;
            for (int i = 0; i < item.parameters.Length; i++)
            {
                if (parameterTypes[i] != item.parameters[i])
                {
                    allSame = false;
                    break;
                }
            }

            if (allSame)
            {
                return item.methodInfo;
            }
        }

        return null;
    }
}
