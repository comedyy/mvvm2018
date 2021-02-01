using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EventBindInfo))]
public class EventBindInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DataBindInfoDrawer.InitErrorStyle();

        EditorGUI.BeginProperty(position, label, property);

        EventBindInfo bindingInfo = property.GetSerializedValue<EventBindInfo>();
        if(bindingInfo == null) return;

        string title = bindingInfo.component == null ? "NULL" : bindingInfo.component.ToString();

        bool error = bindingInfo.component == null
            || string.IsNullOrEmpty(bindingInfo.invokeFunctionName);

        GUIStyle style = error ? DataBindInfoDrawer.errorStyle : EditorStyles.foldout;
        GUIContent titleContent = new GUIContent(title);
        
        property.isExpanded = EditorGUI.Foldout(new Rect(position.position, new Vector2(position.width, 20)), 
            property.isExpanded, titleContent, true, style);

        if (property.isExpanded)
        {
            var amountRect = new Rect(position.x, position.y + 20, position.width, 20);
            var unitRect = new Rect(position.x, position.y + 40, position.width, 20);

            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("component"), GUIContent.none);
            if (bindingInfo.component != null)
            {
                SerializedObject o = property.serializedObject;
                Type tO = o.targetObject.GetType();
                FieldInfo viewModelProperty = tO.GetField("ViewModel", BindingFlags.NonPublic | BindingFlags.Instance);
                string typeModel = viewModelProperty.FieldType.Name;
                SerializedProperty propertyProperty = property.FindPropertyRelative("invokeFunctionName");
                List<string> propertyList = EditorPropertyCache.GetMethodAndPropertys(typeModel, bindingInfo.component.GetType());

                if (propertyList.Count > 0)
                {
                    int index = propertyList.IndexOf(propertyProperty.stringValue);
                    index = Mathf.Max(0, index);
                    index = EditorGUI.Popup(unitRect, index, propertyList.ToArray());
                    propertyProperty.stringValue = propertyList[index];
                }
                else
                {
                    propertyProperty.stringValue = "";
                }

                o.ApplyModifiedProperties();
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            return 60;
        }

        return 20;
    }
}


