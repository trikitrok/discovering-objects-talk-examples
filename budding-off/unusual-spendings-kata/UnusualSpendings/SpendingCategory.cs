namespace UnusualSpendings;

public class SpendingCategory
{
    private readonly string _name;
    private readonly Money _totalAmountspent;

    public SpendingCategory(string name, Money totalAmountspent)
    {
        _name = name;
        _totalAmountspent = totalAmountspent;
    }

    public string Name()
    {
        return _name;
    }

    public string CurrencySymbol()
    {
        return _totalAmountspent.CurrencySymbol();
    }

    public decimal TotalAmountspent()
    {
        return _totalAmountspent.Amount();
    }

    protected bool Equals(SpendingCategory other)
    {
        return _name == other._name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SpendingCategory)obj);
    }

    public override int GetHashCode()
    {
        return _name.GetHashCode();
    }

    public override string ToString()
    {
        return $"{nameof(_name)}: {_name}";
    }
}