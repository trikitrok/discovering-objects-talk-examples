namespace Offers;

public class ValidatingOffersGateway : OffersGateway
{
    private readonly OffersGateway _offersGateway;
    private readonly OffersValidator _offersValidator;

    public ValidatingOffersGateway(OffersGateway offersGateway, OffersValidator offersValidator)
    {
        _offersGateway = offersGateway;
        _offersValidator = offersValidator;
    }

    public Offer Retrieve(OfferId id)
    {
        var offer = _offersGateway.Retrieve(id);
        return offer.Validate(_offersValidator);
    }
}