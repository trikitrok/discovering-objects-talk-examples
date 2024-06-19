namespace Offers;

public class EnviousOfferAcceptor : OffersAcceptor
{
    public Offer Accept(Offer offer)
    {
        offer.SetAccepted(true);
        return offer;
    }
}