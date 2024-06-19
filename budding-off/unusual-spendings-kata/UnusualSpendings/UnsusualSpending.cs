using System.Collections.Generic;
using System.Linq;

namespace UnusualSpendings;

public class UnsusualSpending
{
    private readonly List<SpendingCategory> _spendingCategories;

    public UnsusualSpending(List<SpendingCategory> spendingCategories)
    {
        _spendingCategories = spendingCategories;
    }
    
    public List<SpendingCategory> SpendingCategories()
    {
        return _spendingCategories;
    }

    public override string ToString()
    {
        return $"{nameof(_spendingCategories)}: {_spendingCategories}";
    }

    protected bool Equals(UnsusualSpending other)
    {
        return _spendingCategories.SequenceEqual(other._spendingCategories);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((UnsusualSpending)obj);
    }

    public override int GetHashCode()
    {
        return (_spendingCategories != null ? _spendingCategories.GetHashCode() : 0);
    }
}