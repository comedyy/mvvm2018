using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCollectionViewItem<TData> : BaseView<TData> where TData : BaseViewModel
{
    public void SetData(TData data) 
    {
        ViewModel = data;
    }
}
