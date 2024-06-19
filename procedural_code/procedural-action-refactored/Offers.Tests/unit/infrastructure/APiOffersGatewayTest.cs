using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Offers.infrastructure;

namespace Offers.Tests.unit.infrastructure;

public class APiOffersGatewayTest
{
    private const string ApiBaseUrl = "baseUri";
    private const int ApiVersion = 2;
    private const string OfferId = "offerId";
    private const decimal Price = 1.0m;
    private const string ProductId = "productId";
    private ApiClient<OfferResponseData> _apiClient;
    private OffersGateway _offersGateway;
    private OffersValidator _offersValidator;
    private string _uri;

    [SetUp]
    public void SetUp()
    {
        _apiClient = Substitute.For<ApiClient<OfferResponseData>>();
        _offersValidator = Substitute.For<OffersValidator>();
        _offersGateway = new APiOffersGateway(ApiBaseUrl, _apiClient, _offersValidator);
        _uri = $"{ApiBaseUrl}/offers/?version={ApiVersion}&offerId={OfferId}";
    }

    [Test]
    public void retrieving_an_offer_succeeds()
    {
        var offerId = new OfferId(OfferId);
        _apiClient.GetApiResponse(_uri).Returns(new List<OfferResponseData>
        {
            new(OfferId, Price, ProductId)
        });

        var offer = _offersGateway.Retrieve(offerId);

        Assert.That(offer, Is.EqualTo(Offer.NotYetAccepted(offerId, Price, ProductId)));
    }

    [Test]
    public void retrieving_an_offer_fails()
    {
        var offerId = new OfferId(OfferId);
        _apiClient.GetApiResponse(_uri).Throws(_ => { throw new Exception(); });

        Assert.Throws<OfferRetrievalException>(() => _offersGateway.Retrieve(offerId));
    }

    [Test]
    public void retrieving_an_invalid_offer()
    {
        var offerId = new OfferId(OfferId);
        var notYetAcceptedOffer = Offer.NotYetAccepted(offerId, Price, ProductId);
        _apiClient.GetApiResponse(_uri).Returns(new List<OfferResponseData>
        {
            new(OfferId, Price, ProductId)
        });
        _offersValidator.When(
                x => x.Validate(notYetAcceptedOffer))
            .Do(
                _ => throw new InvalidOfferException(notYetAcceptedOffer)
            );

        Assert.Throws<InvalidOfferException>(() => _offersGateway.Retrieve(offerId));
    }
}