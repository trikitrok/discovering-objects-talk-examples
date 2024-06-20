using System;

namespace UnusualSpendings;

public class Money
{
    private readonly decimal _amount;
    private readonly string _currencySymbol;

    public Money(decimal amount, string currencySymbol)
    {
        _amount = amount;
        _currencySymbol = currencySymbol;
    }

    public string CurrencySymbol()
    {
        return _currencySymbol;
    }

    public decimal Amount()
    {
        return _amount;
    }

    public override string ToString()
    {
        return $"{nameof(_amount)}: {_amount}, {nameof(_currencySymbol)}: {_currencySymbol}";
    }

    protected bool Equals(Money other)
    {
        return _amount == other._amount && _currencySymbol == other._currencySymbol;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Money)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_amount, _currencySymbol);
    }
}