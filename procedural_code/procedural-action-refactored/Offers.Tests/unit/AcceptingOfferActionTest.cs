using NSubstitute;
using NUnit.Framework;

namespace Offers.Tests.unit;

public class AcceptingOfferActionTest
{
    private const decimal Price = 1.0m;
    private const string ProductId = "productId";
    private AcceptingOfferAction _acceptingOfferAction;
    private OfferId _anOfferId;
    private OffersGateway _offersGateway;
    private OffersRepository _offersRepository;

    [SetUp]
    public void SetUp()
    {
        _anOfferId = new OfferId("anOfferId");
        _offersGateway = Substitute.For<OffersGateway>();
        _offersRepository = Substitute.For<OffersRepository>();
        _acceptingOfferAction = new AcceptingOfferAction(_offersGateway, _offersRepository);
    }


    [Test]
    public void accepting_an_offer()
    {
        _offersGateway.Retrieve(_anOfferId).Returns(Offer.NotYetAccepted(_anOfferId, Price, ProductId));

        _acceptingOfferAction.DoAction(_anOfferId);

        _offersRepository.Received(1).Save(Offer.Accepted(_anOfferId, Price, ProductId));
    }

    [Test]
    public void accepting_an_invalid_offer_fails()
    {
        _offersGateway.Retrieve(_anOfferId).Returns(
            _ => throw new InvalidOfferException(Offer.NotYetAccepted(_anOfferId, Price, ProductId))
        );

        Assert.Throws<InvalidOfferException>(
            () => _acceptingOfferAction.DoAction(_anOfferId)
        );

        _offersRepository.Received(0).Save(Arg.Any<Offer>());
    }

    [Test]
    public void accepting_an_offer_fails_when_it_cannot_be_retrieved()
    {
        _offersGateway.Retrieve(_anOfferId).Returns(
            _ => throw new OfferRetrievalException(new Exception())
        );

        Assert.Throws<OfferRetrievalException>(
            () => _acceptingOfferAction.DoAction(_anOfferId)
        );

        _offersRepository.Received(0).Save(Arg.Any<Offer>());
    }
}