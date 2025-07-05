namespace UserAccount;

public class Id
{
    private string _digits;

    public Id(string digits)
    {
        _digits = digits;
    }

    public string Digits()
    {
        return _digits;
    }


    protected bool Equals(Id other)
    {
        return _digits == other._digits;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Id)obj);
    }

    public override int GetHashCode()
    {
        return (_digits != null ? _digits.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return $"{nameof(_digits)}: {_digits}";
    }
}