using ApprovalTests.Scrubber;
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
    private OffersGateway _offersGateway;
    private ApiClient<OfferResponseData> _apiClient;
    private string _uri;
    private OfferId _offerId;

    [SetUp]
    public void SetUp()
    {
        _offerId = new OfferId(OfferId);
        _apiClient = Substitute.For<ApiClient<OfferResponseData>>();
        _offersGateway = new APiOffersGateway(ApiBaseUrl, _apiClient);
        _uri = $"{ApiBaseUrl}/offers/?version={ApiVersion}&offerId={OfferId}";
    }

    [Test]
    public void retrieving_an_offer_succeeds()
    {
        _apiClient.GetApiResponse(_uri).Returns(new List<OfferResponseData>()
        {
            new OfferResponseData(OfferId, Price, ProductId)
        });

        var offer = _offersGateway.Retrieve(_offerId);

        Assert.That(offer, Is.EqualTo(Offer.NotYetAccepted(new OfferId(OfferId), Price, ProductId)));
    }

    [Test]
    public void retrieving_an_offer_fails()
    {
        _apiClient.GetApiResponse(_uri).Throws(_ => { throw new Exception(); });

        Assert.Throws<OfferRetrievalException>(() => _offersGateway.Retrieve(_offerId));
    }
}