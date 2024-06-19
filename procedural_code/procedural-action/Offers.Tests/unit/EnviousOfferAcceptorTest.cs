using NUnit.Framework;

namespace Offers.Tests.unit;

public class EnviousOfferAcceptorTest
{
    private const decimal Price = 1.0m;
    private const string ProductId = "productId";

    [Test]
    public void accepts_offer()
    {
        var offerId = new OfferId("offerId");
        var notYetAcceptedOffer = Offer.NotYetAccepted(offerId, Price, ProductId);
        var enviousOfferAcceptor = new EnviousOfferAcceptor();

        var acceptedOffer = enviousOfferAcceptor.Accept(notYetAcceptedOffer);

        Assert.That(acceptedOffer, Is.EqualTo(Offer.Accepted(offerId, Price, ProductId)));
    }
}