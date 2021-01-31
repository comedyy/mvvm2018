using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public interface IListSetter 
{
    void SetListData(IList data);
}


public class BaseCollectionView<TViewItem, TData> : MonoBehaviour , IListSetter
    where TViewItem : BaseCollectionViewItem<TData>
    where TData : BaseViewModel
{
    List<TViewItem> _children = new List<TViewItem>();
    [SerializeField] TViewItem Prefab;
    [SerializeField] Transform ItemRoot;

    public void SetListData(IList list)
    {
        ObservableCollection<TData> lstData = list as ObservableCollection<TData>;
        lstData.CollectionChanged += OnCollectionChange;

        for (int i = 0; i < lstData.Count; i++)
        {
            OnAddElement(i, lstData[i]);
        }
    }

    void OnAddElement(int index, TData data) 
    {
        TViewItem item = GameObject.Instantiate(Prefab.gameObject, ItemRoot).GetComponent<TViewItem>();
        item.SetData(data);
        item.gameObject.SetActive(true);
        item.transform.SetSiblingIndex(index);

        _children.Insert(index, item);
    }

    void OnRemoveElement(int index)
    {
        TViewItem item = _children[index];

        GameObject.Destroy(item.gameObject);

        _children.RemoveAt(index);
    }

    void OnClearElements() 
    {
        foreach (var item in _children)
        {
            GameObject.Destroy(item.gameObject);
        }

        _children.Clear();
    }

    private void OnCollectionChange(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnAddElement(e.NewStartingIndex, (TData)e.NewItems[0]);
                break;
            case NotifyCollectionChangedAction.Remove:
                OnRemoveElement(e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                OnClearElements();
                break;
            case NotifyCollectionChangedAction.Move:
                throw new Exception("No Support Move");
            case NotifyCollectionChangedAction.Replace:
                throw new Exception("No Support Replace");
            default:
                break;
        }
    }


}
