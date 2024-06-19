namespace UserAccount;

public class Id
{
    private readonly string _id;

    public Id(string id)
    {
        _id = id;
    }

    public string AsText()
    {
        return _id;
    }

    protected bool Equals(Id other)
    {
        return _id == other._id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Id)obj);
    }

    public override int GetHashCode()
    {
        return (_id != null ? _id.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return $"{nameof(_id)}: {_id}";
    }
}