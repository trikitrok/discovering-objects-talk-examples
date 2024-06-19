using System;

namespace Offers;

public class InvalidOfferException : Exception
{
    private readonly Offer _notYetAcceptedOffer;

    public InvalidOfferException(Offer notYetAcceptedOffer)
    {
        _notYetAcceptedOffer = notYetAcceptedOffer;
    }
}