using System;

namespace Offers;

public abstract class Offer
{
    private readonly OfferId _id;
    private readonly bool _accepted;

    protected Offer(OfferId id, bool accepted)
    {
        _id = id;
        _accepted = accepted;
    }

    public Offer Validate(OffersValidator offersValidator)
    {
        offersValidator.Validate(this);
        return this;
    }
    
    public Offer Accept()
    {
        return Accepted(_id);
    }

    public static Offer Accepted(OfferId id)
    {
        return new AcceptedOffer(id);
    }

    public static Offer NotYetAccepted(OfferId id)
    {
        return new NotYetAcceptedOffer(id);
    }

    private class NotYetAcceptedOffer : Offer
    {
        public NotYetAcceptedOffer(OfferId id) : base(id, false)
        {
        }
    }

    private class AcceptedOffer : Offer
    {
        public AcceptedOffer(OfferId id) : base(id, true)
        {
        }
    }

    protected bool Equals(Offer other)
    {
        return Equals(_id, other._id) && _accepted == other._accepted;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
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