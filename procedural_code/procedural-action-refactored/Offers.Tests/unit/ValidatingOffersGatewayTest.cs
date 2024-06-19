using NSubstitute;
using NUnit.Framework;

namespace Offers.Tests.unit;

public class ValidatingOffersGatewayTest
{
    private OffersGateway _offersGateway;
    private OffersValidator _offersValidator;
    private ValidatingOffersGateway _validatingOffersGateway;
    private OfferId _anOfferId;

    [SetUp]
    public void SetUp()
    {
        _anOfferId = new OfferId("anOfferId");
        _offersGateway = Substitute.For<OffersGateway>();
        _offersValidator = Substitute.For<OffersValidator>();
        _validatingOffersGateway = new ValidatingOffersGateway(_offersGateway, _offersValidator);
    }

    [Test]
    public void retrieving_valid_offer()
    {
        _offersGateway.Retrieve(_anOfferId).Returns(Offer.NotYetAccepted(_anOfferId));

        var offer = _validatingOffersGateway.Retrieve(_anOfferId);

        Assert.That(offer, Is.EqualTo(Offer.NotYetAccepted(_anOfferId)));
    }

    [Test]
    public void retrieving_an_invalid_offer()
    {
        var notYetAcceptedOffer = Offer.NotYetAccepted(_anOfferId);
        _offersGateway.Retrieve(_anOfferId).Returns(notYetAcceptedOffer);
        _offersValidator.When(
                x => x.Validate(notYetAcceptedOffer))
            .Do(
                _ => throw new InvalidOfferException(notYetAcceptedOffer)
            );
        
        Assert.Throws<InvalidOfferException>(() => { _validatingOffersGateway.Retrieve(_anOfferId); });
    }
}