namespace UnusualSpendings;

public class UserId
{
    private readonly string _id;

    public UserId(string id)
    {
        _id = id;
    }

    protected bool Equals(UserId other)
    {
        return _id == other._id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UserId)obj);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{nameof(_id)}: {_id}";
    }
}