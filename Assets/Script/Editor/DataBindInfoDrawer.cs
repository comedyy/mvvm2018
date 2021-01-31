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
            Debug.Log(bindingInfo.invokeFunctionName);
            var amountRect = new Rect(position.x, position.y + 20, position.width, 20);
            var unitRect = new Rect(position.x, position.y + 40, position.width, 20);
            var nameRect = new Rect(position.x, position.y + 60, position.width, 20);

            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("component"), GUIContent.none);

            SerializedProperty methodMethod = property.FindPropertyRelative("invokeFunctionName");
            List<string> methodList = EditorPropertyCache.GetComponentOpts(bindingInfo.component.GetType());
            SerializedObject o = property.serializedObject;

            int index;
            if (methodList.Count > 0)
            {
                index = methodList.IndexOf(methodMethod.stringValue);
                index = Mathf.Max(0, index);
                index = EditorGUI.Popup(unitRect, index, methodList.ToArray());
                methodMethod.stringValue = methodList[index];
            }
            else
            {
                methodMethod.stringValue = "";
                o.ApplyModifiedProperties();
            }

            Type tO = o.targetObject.GetType();
            PropertyInfo viewModelProperty = tO.GetProperty("ViewModel", BindingFlags.Public | BindingFlags.Instance);
            string typeModel = viewModelProperty.PropertyType.Name;
            SerializedProperty propertyProperty = property.FindPropertyRelative("propertyName");
            List<string> propertyList = EditorPropertyCache.GetPropertys(typeModel, bindingInfo.component.GetType(), methodMethod.stringValue);

            if (propertyList.Count > 0)
            {
                index = propertyList.IndexOf(propertyProperty.stringValue);
                index = Mathf.Max(0, index);
                index = EditorGUI.Popup(nameRect, index, propertyList.ToArray());
                propertyProperty.stringValue = propertyList[index];
            }
            else
            {
                propertyProperty.stringValue = "";
                o.ApplyModifiedProperties();
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return 80;
        }

        return 20;
    }
}


