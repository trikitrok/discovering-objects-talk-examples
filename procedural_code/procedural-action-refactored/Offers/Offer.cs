using System;

namespace Offers;

public class Offer
{
    private readonly bool _accepted;
    private readonly decimal _price;
    private readonly string _productId;
    private readonly OfferId _id;

    private Offer(OfferId id, bool accepted, decimal price,  string productId)
    {
        _id = id;
        _accepted = accepted;
        _price = price;
        _productId = productId;
    }

    public Offer Accept()
    {
        return Accepted(_id, _price, _productId);
    }

    public static Offer Accepted(OfferId id, decimal price, string productId)
    {
        return new Offer(id, true, price,  productId);
    }

    public static Offer NotYetAccepted(OfferId id, decimal price, string productId)
    {
        return new Offer(id, false, price,  productId);
    }

    protected bool Equals(Offer other)
    {
        return Equals(_id, other._id) && _accepted == other._accepted;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Offer)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_id, _accepted);
    }

    public override string ToString()
    {
        return $"{nameof(_id)}: {_id}, {nameof(_accepted)}: {_accepted}";
    }
}