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
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        DataBindInfo bindingInfo = property.GetSerializedValue<DataBindInfo>();
        if (bindingInfo == null)
        {
            return;
        }
        string title = bindingInfo.component == null ? "NULL" : bindingInfo.component.ToString();
        GUIContent titleContent = new GUIContent(title);
        property.isExpanded = EditorGUI.Foldout(new Rect(position.position, new Vector2(position.width, 20)), property.isExpanded, titleContent, true);
        if (property.isExpanded)
        {
            var amountRect = new Rect(position.x, position.y + 20, position.width, 20);
            var unitRect = new Rect(position.x, position.y + 40, position.width, 20);
            var nameRect = new Rect(position.x, position.y + 60, position.width, 20);

            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("component"), GUIContent.none);

            SerializedProperty methodMethod = property.FindPropertyRelative("invokeFunctionName");
            List<string> methodList = EditorPropertyCache.GetComponentOpts(bindingInfo.component.GetType());

            int index = 0;
            if (methodList.Count > 0)
            {
                index = methodList.IndexOf(methodMethod.stringValue);
                index = EditorGUI.Popup(unitRect, index, methodList.ToArray());
                index = Mathf.Max(0, index);
                methodMethod.stringValue = methodList[index];
            }

            SerializedObject o = property.serializedObject;
            Type tO = o.targetObject.GetType();
            PropertyInfo viewModelProperty = tO.GetProperty("ViewModel", BindingFlags.Public | BindingFlags.Instance);
            string typeModel = viewModelProperty.PropertyType.Name;
            SerializedProperty propertyProperty = property.FindPropertyRelative("propertyName");
            List<string> propertyList = EditorPropertyCache.GetPropertys(typeModel, bindingInfo.component.GetType(), methodMethod.stringValue);

            if (propertyList.Count > 0)
            {
                index = propertyList.IndexOf(propertyProperty.stringValue);
                index = EditorGUI.Popup(nameRect, index, propertyList.ToArray());
                index = Mathf.Max(0, index);
                propertyProperty.stringValue = propertyList[index];
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


