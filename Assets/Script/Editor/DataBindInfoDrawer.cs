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
    static string[] dataFrom = new string[] { "VM", "Custom", "Language"};

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

            ReflectionMethodItem item = GetMethod(property);
            if (item.parameters.Length > 2)
            {
                SerializedProperty parameterProperty = property.FindPropertyRelative("parameters");
                parameterProperty.arraySize = item.parameters.Length - 2;
                for (int i = 0; i < parameterProperty.arraySize; i++)
                {
                    var rect = new Rect(position.x, position.y + 80 + i * 20, 50, 20);
                    var rectDetail = new Rect(position.x + 50, position.y + 80 + i * 20, position.width - 50, 20);
                    Type parameterType = item.parameters[i + 2];

                    SerializedProperty parameter = parameterProperty.GetArrayElementAtIndex(i);
                    SerializedProperty dataFromProperty = parameter.FindPropertyRelative("dataFrom");
                    SerializedProperty paramTypeProperty = parameter.FindPropertyRelative("paramType");
                    SerializedProperty stringParamProperty = parameter.FindPropertyRelative("paramStr");
                    paramTypeProperty.stringValue = parameterType.ToString();

                    dataFromProperty.intValue = EditorGUI.Popup(rect, dataFromProperty.intValue, dataFrom.ToArray());
                    if (dataFromProperty.intValue == (int)DataFrom.VM)
                    {
                        lstTemp.Clear();
                        ReflectionTool.GetVmPropertyByType(vmType, parameterType, lstTemp);

                        if (lstTemp.Count > 0)
                        {
                            index = lstTemp.IndexOf(stringParamProperty.stringValue);
                            index = Mathf.Max(0, index);
                            index = EditorGUI.Popup(rectDetail, index, lstTemp.ToArray());
                            stringParamProperty.stringValue = lstTemp[index];
                        }
                    }
                    else if (dataFromProperty.intValue == (int)DataFrom.Custom)
                    {
                        if (parameterType == typeof(string))
                        {
                            stringParamProperty.stringValue = EditorGUI.TextField(rectDetail, stringParamProperty.stringValue);
                        }
                        else 
                        {
                            object x = Convert.ChangeType(stringParamProperty.stringValue, parameterType);
                            if (parameterType == typeof(int))
                            {
                                x = EditorGUI.IntField(rectDetail, (int)x);
                            }
                            else if (parameterType == typeof(bool))
                            {
                                x = EditorGUI.Toggle(rectDetail, (bool)x);
                            }
                            else if (parameterType == typeof(float))
                            {
                                x = EditorGUI.FloatField(rectDetail, (float)x);
                            }
                            stringParamProperty.stringValue = (string)Convert.ChangeType(x, typeof(string));
                        }
                    }

                    EditorGUI.PropertyField(rect, dataFromProperty, GUIContent.none);
                }
            }


            o.ApplyModifiedProperties();
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
        {
            ReflectionMethodItem item = GetMethod(property);
            if (item == null)
            {
                return 60;
            }

            return item.parameters.Length * 20 + 40;
        }

        return 20;
    }

    ReflectionMethodItem GetMethod(SerializedProperty property) 
    {
        DataBindInfo bindingInfo = property.GetSerializedValue<DataBindInfo>();
        if (bindingInfo == null)
        {
            return null;
        }

        Type tO = property.serializedObject.targetObject.GetType();
        FieldInfo viewModelProperty = tO.GetField("ViewModel", BindingFlags.NonPublic | BindingFlags.Instance);
        Type vmType = viewModelProperty.FieldType;
        PropertyInfo propertyInfo = vmType.GetProperty(bindingInfo.propertyName);
        if (propertyInfo == null)
        {
            return null;
        }

        ReflectionMethodItem item = ReflectionTool.GetComponentMethod(bindingInfo.component.GetType(), bindingInfo.invokeFunctionName, propertyInfo.PropertyType);
        return item;
    }
}


