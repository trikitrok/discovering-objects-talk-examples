namespace Offers;

public class OfferId
{
    private readonly string _offerId;

    public OfferId(string offerId)
    {
        _offerId = offerId;
    }

    protected bool Equals(OfferId other)
    {
        return _offerId == other._offerId;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OfferId)obj);
    }

    public override int GetHashCode()
    {
        return (_offerId != null ? _offerId.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return $"{nameof(_offerId)}: {_offerId}";
    }
}