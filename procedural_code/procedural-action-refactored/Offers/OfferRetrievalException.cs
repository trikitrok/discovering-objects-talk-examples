using System;

namespace Offers;

public class OfferRetrievalException : Exception
{
    public OfferRetrievalException(Exception exception) : base(exception.Message, exception)
    {
    }
}