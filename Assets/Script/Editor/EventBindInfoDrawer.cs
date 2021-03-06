using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomPropertyDrawer(typeof(EventBindInfo))]
public class EventBindInfoDrawer : PropertyDrawer
{
    static List<string> lstTemp = new List<string>();

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
                Type vmType = viewModelProperty.FieldType;
                SerializedProperty propertyProperty = property.FindPropertyRelative("invokeFunctionName");

                GetArgumentByComponentEvent(bindingInfo.component.GetType(), out Type argType);
                lstTemp.Clear();
                ReflectionTool.GetVmMethodByParameter1Type(vmType, argType, lstTemp);
                ReflectionTool.GetVmPropertyByType(vmType, argType, lstTemp);

                if (lstTemp.Count > 0)
                {
                    int index = lstTemp.IndexOf(propertyProperty.stringValue);
                    index = Mathf.Max(0, index);
                    index = EditorGUI.Popup(unitRect, index, lstTemp.ToArray());
                    propertyProperty.stringValue = lstTemp[index];
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


