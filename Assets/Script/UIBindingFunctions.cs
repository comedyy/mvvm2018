using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public static class UIBindingFunctions
{
    public static void SetText(Text text, float value)
    {
        if (text == null)
        {
            Debug.LogError("SetText Error, Text Null");
            return;
        }

        text.text = ((int)value).ToString();
    }

    public static void SetText(Text text, int value)
    {
        if (text == null)
        {
            Debug.LogError("SetText Error, Text Null");
            return;
        }

        text.text = value.ToString();
    }

    public static void SetText(Text text, string value) 
    {
        if (text == null)
        {
            Debug.LogError("SetText Error, Text Null");
            return;
        }

        text.text = value;
    }

    public static void SetTextWithFormation(Text text, string value, string format)
    {
        if (text == null)
        {
            Debug.LogError("SetText Error, Text Null");
            return;
        }

        text.text = string.Format(format, value);
    }

    public static void SetTextWithFormation(Text text, float value, string format)
    {
        if (text == null)
        {
            Debug.LogError("SetText Error, Text Null");
            return;
        }

        text.text = string.Format(format, value);
    }

    public static void SetTextWithFormation(Text text, int value, string format)
    {
        if (text == null)
        {
            Debug.LogError("SetText Error, Text Null");
            return;
        }

        text.text = string.Format(format, value);
    }


    public static void SetToggle(Toggle toggle, bool value) 
    {
        toggle.isOn = value;
    }

    public static void SetSlider(Slider slider, float value) 
    {
        slider.value = value;
    }

    public static void SetImage(Image image, string path)
    {
        image.sprite = Resources.Load<Sprite>(path);
    }

    public static void SetList(IListSetter l, IList data) 
    {
        l.SetListData(data);
    }
}
