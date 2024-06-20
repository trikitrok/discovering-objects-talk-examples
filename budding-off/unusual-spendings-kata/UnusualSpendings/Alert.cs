using System;

namespace UnusualSpendings;

public class Alert
{
    private readonly string _alertText;
    private readonly UserContactData _userContactData;

    public Alert(string alertText, UserContactData userContactData)
    {
        _alertText = alertText;
        _userContactData = userContactData;
    }

    public override string ToString()
    {
        return $"{nameof(_alertText)}: {_alertText}, {nameof(_userContactData)}: {_userContactData}";
    }

    protected bool Equals(Alert other)
    {
        return _alertText == other._alertText && Equals(_userContactData, other._userContactData);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Alert)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_alertText, _userContactData);
    }
}