namespace UnusualSpendings;

public class UserContactData
{
    private readonly string _email;

    public UserContactData(string email)
    {
        _email = email;
    }

    public override string ToString()
    {
        return $"{nameof(_email)}: {_email}";
    }

    protected bool Equals(UserContactData other)
    {
        return _email == other._email;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UserContactData)obj);
    }

    public override int GetHashCode()
    {
        return _email != null ? _email.GetHashCode() : 0;
    }
}