namespace Offers;

public class AcceptingOfferAction
{
    private readonly OffersGateway _offersGateway;
    private readonly OffersValidator _offersValidator;
    private readonly OffersAcceptor _offersAcceptor;
    private readonly OffersRepository _offersRepository;

    public AcceptingOfferAction(
        OffersGateway offersGateway,
        OffersValidator offersValidator,
        OffersAcceptor offersAcceptor,
        OffersRepository offersRepository)
    {
        _offersGateway = offersGateway;
        _offersValidator = offersValidator;
        _offersAcceptor = offersAcceptor;
        _offersRepository = offersRepository;
    }

    public void DoAction(OfferId id)
    {
        var offer = _offersGateway.Retrieve(id);
        _offersValidator.Validate(offer);
        var acceptedOffer = _offersAcceptor.Accept(offer);
        _offersRepository.Save(acceptedOffer);
    }
}