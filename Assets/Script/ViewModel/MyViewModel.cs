using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class MyViewModel : BaseViewModel
{
    public MyViewModel() 
    {
        shoppingBag = false;
        discount = 100;

        RecalcTotal();
    }

    int total;
    bool shoppingBag;
    float discount;
    string vipIcon;
    ObservableCollection<BuyInfo> lst = new ObservableCollection<BuyInfo>();

    public ObservableCollection<BuyInfo> BuyInfoList { get { return lst; } }

    public int Total
    {
        get { return total; }
        set
        {
            if (total != value)
            {
                total = value;
                OnPropertyChange(nameof(Total));
            }
        }
    }

    public bool ShoppingBag
    {
        get { return shoppingBag; }
        set
        {
            if (shoppingBag != value)
            {
                shoppingBag = value;
                RecalcTotal();
                OnPropertyChange(nameof(ShoppingBag));
            }
        }
    }

    public float Discount
    {
        get { return discount; }
        set
        {
            if (discount != value)
            {
                discount = value;

                RecalcTotal();
                OnPropertyChange(nameof(Discount));
            }
        }
    }

    public string VipIcon { 
        get { return vipIcon; }
        set {
            if (vipIcon != value)
            {
                vipIcon = value;
                OnPropertyChange(nameof(VipIcon));
            }
        }
    }

    internal void OnCountChange()
    {
        RecalcTotal();
        for (int i = lst.Count - 1; i >= 0; i--)
        {
            if (lst[i].Count == 0)
            {
                lst.RemoveAt(i);
            }
        }
    }

    public void AddAppleToShopList() 
    {
        lst.Add(new BuyInfo(Fruit.Apple, 1, this));
        RecalcTotal();
    }

    public void AddOrangeToShopList() 
    {
        lst.Add(new BuyInfo(Fruit.Orange, 1, this));
        RecalcTotal();
    }

    public void AddBananaToShopList() 
    {
        lst.Add(new BuyInfo(Fruit.Banana, 1, this));
        RecalcTotal();
    }

    void RecalcTotal() 
    {
        int value = 0;
        foreach (var item in lst)
        {
            value += item.Price * item.Count;
        }

        if (shoppingBag)
        {
            value++;
        }

        Total = (int)(value * discount / 100f);

        VipIcon = total > 100 ? "Vip2" : "Vip1";
    }
}
