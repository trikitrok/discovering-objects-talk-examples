using NSubstitute;
using NUnit.Framework;

namespace Offers.Tests.unit;

public class AcceptingOfferActionTest
{
    private const decimal Price = 1.0m;
    private const string ProductId = "productId";
    private AcceptingOfferAction _acceptingOfferAction;
    private OfferId _anOfferId;
    private OffersAcceptor _offersAcceptor;
    private OffersGateway _offersGateway;
    private OffersRepository _offersRepository;
    private OffersValidator _offersValidator;

    [SetUp]
    public void SetUp()
    {
        _anOfferId = new OfferId("anOfferId");
        _offersGateway = Substitute.For<OffersGateway>();
        _offersValidator = Substitute.For<OffersValidator>();
        _offersAcceptor = Substitute.For<OffersAcceptor>();
        _offersRepository = Substitute.For<OffersRepository>();
        _acceptingOfferAction =
            new AcceptingOfferAction(_offersGateway, _offersValidator, _offersAcceptor, _offersRepository);
    }

    [Test]
    public void accepting_an_offer()
    {
        var offerId = _anOfferId;
        var notYetAcceptedOffer = Offer.NotYetAccepted(offerId, Price, ProductId);
        var acceptedOffer = Offer.Accepted(offerId, Price, ProductId);
        _offersGateway.Retrieve(offerId).Returns(notYetAcceptedOffer);
        _offersAcceptor.Accept(notYetAcceptedOffer).Returns(acceptedOffer);

        _acceptingOfferAction.DoAction(offerId);

        Received.InOrder(() =>
        {
            _offersGateway.Received(1).Retrieve(offerId);
            _offersValidator.Received(1).Validate(notYetAcceptedOffer);
            _offersAcceptor.Received(1).Accept(notYetAcceptedOffer);
            _offersRepository.Received(1).Save(acceptedOffer);
        });
    }

    [Test]
    public void accepting_an_offer_fails()
    {
        var notYetAcceptedOffer = Offer.NotYetAccepted(_anOfferId, Price, ProductId);
        _offersGateway.Retrieve(_anOfferId).Returns(notYetAcceptedOffer);
        _offersValidator.When(x => x.Validate(notYetAcceptedOffer))
            .Do(x => throw new InvalidOfferException(notYetAcceptedOffer));

        Assert.Throws<InvalidOfferException>(() => _acceptingOfferAction.DoAction(_anOfferId));

        Received.InOrder(() =>
        {
            _offersGateway.Received(1).Retrieve(_anOfferId);
            _offersValidator.Received(1).Validate(notYetAcceptedOffer);
        });

        _offersAcceptor.Received(0).Accept(Arg.Any<Offer>());
        _offersRepository.Received(0).Save(Arg.Any<Offer>());
    }
}