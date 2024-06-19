using NSubstitute;
using NUnit.Framework;

namespace Offers.Tests.unit;

public class AcceptingOfferActionTest
{
    private OffersGateway _offersGateway;
    private OffersRepository _offersRepository;
    private AcceptingOfferAction _acceptingOfferAction;
    private OfferId _anOfferId;

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
        _offersGateway.Retrieve(_anOfferId).Returns(Offer.NotYetAccepted(_anOfferId));

        _acceptingOfferAction.DoAction(_anOfferId);

        _offersRepository.Received(1).Save(Offer.Accepted(_anOfferId));
    }

    [Test]
    public void accepting_an_offer_fails()
    {
        _offersGateway.Retrieve(_anOfferId).Returns(
            _ => throw new InvalidOfferException(Offer.NotYetAccepted(_anOfferId))
        );

        Assert.Throws<InvalidOfferException>(
            () => _acceptingOfferAction.DoAction(_anOfferId)
        );

        _offersRepository.Received(0).Save(Arg.Any<Offer>());
    }
}