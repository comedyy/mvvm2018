public enum Fruit
{
    Apple,
    Orange,
    Banana,
    Invalid,
}

public class BuyInfo : BaseViewModel
{
    Fruit fruit = Fruit.Invalid;
    string fruitName;
    string fruitPath;
    int count;
    MyViewModel _parent;
    int price;

    public BuyInfo(Fruit fruit, int count, MyViewModel parent) 
    {
        _parent = parent;
        Fruit = fruit;
        Count = count;
    }

    void UpdateFruit() 
    {
        switch (this.fruit)
        {
            case Fruit.Apple:
                FruitName = "Apple";
                FruitPath = "apple";
                Price = 4;
                break;
            case Fruit.Orange:
                FruitName = "Orange";
                FruitPath = "orange";
                Price = 5;
                break;
            case Fruit.Banana:
                FruitName = "Banana";
                FruitPath = "banana";
                Price = 3;
                break;
            default:
                break;
        }
    }

    public Fruit Fruit { 
        get { return fruit; }
        set {
            if (fruit != value)
            {
                fruit = value;
                UpdateFruit();
                OnPropertyChange(nameof(Fruit));
            }
        }
    }

    public string FruitName
    {
        get { return fruitName; }
        set
        {
            if (fruitName != value)
            {
                fruitName = value;
                OnPropertyChange(nameof(FruitName));
            }
        }
    }

    public string FruitPath
    {
        get { return fruitPath; }
        set
        {
            if (fruitPath != value)
            {
                fruitPath = value;
                OnPropertyChange(nameof(FruitPath));
            }
        }
    }

    public int Count
    {
        get { return count; }
        set
        {
            if (count != value)
            {
                count = value;
                _parent.OnCountChange();
                OnPropertyChange(nameof(Count));
            }
        }
    }

    public int Price
    {
        get { return price; }
        set
        {
            if (price != value)
            {
                price = value;
                OnPropertyChange(nameof(Price));
            }
        }
    }


    public void IncrCount() { Count++; }
    public void DecrCount() { Count--; }
}
