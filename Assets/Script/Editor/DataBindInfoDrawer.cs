using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DataBindInfo))]
public class DataBindInfoDrawer : PropertyDrawer
{
    public static GUIStyle errorStyle;
    static List<string> lstTemp = new List<string>();
    public static void InitErrorStyle(){
        if (errorStyle != null)
        {
            return;
        }

        errorStyle = new GUIStyle(EditorStyles.foldout);
        Color myStyleColor = Color.red;
        errorStyle.fontStyle = FontStyle.Bold;
        errorStyle.normal.textColor = myStyleColor;
        errorStyle.onNormal.textColor = myStyleColor;
        errorStyle.hover.textColor = myStyleColor;
        errorStyle.onHover.textColor = myStyleColor;
        errorStyle.focused.textColor = myStyleColor;
        errorStyle.onFocused.textColor = myStyleColor;
        errorStyle.active.textColor = myStyleColor;
        errorStyle.onActive.textColor = myStyleColor;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InitErrorStyle();

        EditorGUI.BeginProperty(position, label, property);

        DataBindInfo bindingInfo = property.GetSerializedValue<DataBindInfo>();
        if (bindingInfo == null)
        {
            return;
        }
        string title = bindingInfo.component == null ? "NULL" : bindingInfo.component.ToString();
        GUIContent titleContent = new GUIContent(title);

        bool error = bindingInfo.component == null 
            || string.IsNullOrEmpty(bindingInfo.invokeFunctionName) 
            || string.IsNullOrEmpty(bindingInfo.propertyName);

        GUIStyle style = error ? errorStyle : EditorStyles.foldout;
        property.isExpanded = EditorGUI.Foldout(new Rect(position.position, new Vector2(position.width, 20)),
             property.isExpanded, titleContent, true, style);


        if (property.isExpanded)
        {
            var amountRect = new Rect(position.x, position.y + 20, position.width, 20);
            var unitRect = new Rect(position.x, position.y + 40, position.width, 20);
            var nameRect = new Rect(position.x, position.y + 60, position.width, 20);

            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("component"), GUIContent.none);

            SerializedObject o = property.serializedObject;
            SerializedProperty methodMethod = property.FindPropertyRelative("invokeFunctionName");

            lstTemp.Clear();
            ReflectionTool.GetComponentMethods(bindingInfo.component.GetType(), lstTemp);

            int index;
            if (lstTemp.Count > 0)
            {
                index = lstTemp.IndexOf(methodMethod.stringValue);
                index = Mathf.Max(0, index);
                index = EditorGUI.Popup(unitRect, index, lstTemp.ToArray());
                methodMethod.stringValue = lstTemp[index];
            }
            else
            {
                methodMethod.stringValue = "";
            }

            Type tO = o.targetObject.GetType();
            FieldInfo viewModelProperty = tO.GetField("ViewModel", BindingFlags.NonPublic | BindingFlags.Instance);
            Type vmType = viewModelProperty.FieldType;

            lstTemp.Clear();
            ReflectionTool.GetVmPropertysByMethod(vmType, bindingInfo.component.GetType(), methodMethod.stringValue, lstTemp);

            SerializedProperty propertyProperty = property.FindPropertyRelative("propertyName");
            if (lstTemp.Count > 0)
            {
                index = lstTemp.IndexOf(propertyProperty.stringValue);
                index = Mathf.Max(0, index);
                index = EditorGUI.Popup(nameRect, index, lstTemp.ToArray());
                propertyProperty.stringValue = lstTemp[index];
            }
            else
            {
                propertyProperty.stringValue = "";
            }

            //int paramCount = GetParameterCount(property, out List<Type> lst);
            //if (paramCount > 2)
            //{
            //    SerializedProperty parameterProperty = property.FindPropertyRelative("parameters");
            //    parameterProperty.arraySize = paramCount - 2;
            //    for (int i = 0; i < parameterProperty.arraySize; i++)
            //    {
            //        var rect = new Rect(position.x, position.y + 80 + i* 20, 50, 20);
            //        var rectDetail = new Rect(position.x, position.y + 80 + i* 20, position.width - 50, 20);

            //        SerializedProperty parameter =  parameterProperty.GetArrayElementAtIndex(i);
            //        SerializedProperty dataFromProperty = parameter.FindPropertyRelative("dataFrom");
            //        SerializedProperty paramTypeProperty = parameter.FindPropertyRelative("paramType");
            //        SerializedProperty stringParamProperty = parameter.FindPropertyRelative("paramStr");
            //        SerializedProperty intParamProperty = parameter.FindPropertyRelative("paramInt");

            //        index = EditorGUI.Popup(nameRect, dataFromProperty.intValue, propertyList.ToArray());
            //        if (index == (int)DataFrom.VM)
            //        {
            //            propertyList = EditorPropertyCache.GetPropertys(typeModel, bindingInfo.component.GetType(), null);
            //            if (propertyList.Count > 0)
            //            {
            //                index = propertyList.IndexOf(stringParamProperty.stringValue);
            //                index = Mathf.Max(0, index);
            //                index = EditorGUI.Popup(nameRect, index, propertyList.ToArray());
            //                stringParamProperty.stringValue = propertyList[index];
            //            }
            //        }
            //        else if (index == (int)DataFrom.Custom)
            //        {

            //        }

            //        EditorGUI.PropertyField(rect, dataFromProperty, GUIContent.none);
            //    }
            //}

            o.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            int parameterCount = GetParameterCount(property, out List<Type> lst);
            if (parameterCount == 0)
            {
                return 60;
            }

            return parameterCount * 20 + 40;
        }

        return 20;
    }

    int GetParameterCount(SerializedProperty property, out List<Type> parameters) 
    {
        parameters = new List<Type>();
        DataBindInfo bindingInfo = property.GetSerializedValue<DataBindInfo>();
        if (bindingInfo == null)
        {
            return 0;
        }

        if (bindingInfo.component == null || string.IsNullOrEmpty(bindingInfo.invokeFunctionName))
        {
            return 0;
        }

        List<MethodInfo> lstInfo = EditorPropertyCache.GetMethodInfo(bindingInfo.component.GetType(), bindingInfo.invokeFunctionName);
        if (lstInfo.Count == 0)
        {
            return 0;
        }

        ParameterInfo[] ps = lstInfo[0].GetParameters();
        foreach (var item in ps)
        {
            parameters.Add(item.ParameterType);
        }
        return ps.Length;
    }
}


