namespace Offers;

public interface OffersGateway
{
    Offer Retrieve(OfferId id);
}