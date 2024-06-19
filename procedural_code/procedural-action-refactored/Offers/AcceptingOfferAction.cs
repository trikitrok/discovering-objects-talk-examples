namespace Offers;

public class AcceptingOfferAction
{
    private readonly OffersGateway _offersGateway;

    private readonly OffersRepository _offersRepository;

    public AcceptingOfferAction(
        OffersGateway offersGateway,
        OffersRepository offersRepository)
    {
        _offersGateway = offersGateway;
        _offersRepository = offersRepository;
    }

    public void DoAction(OfferId id)
    {
        var offer = _offersGateway.Retrieve(id);
        
        _offersRepository.Save(offer.Accept());
    }
}