using System;
using System.Linq;

namespace Offers.infrastructure;

public class APiOffersGateway : OffersGateway
{
    private const int ApiVersion = 2;
    private readonly string _apiBaseUrl;
    private readonly ApiClient<OfferResponseData> _apiClient;
    private readonly OffersValidator _offersValidator;

    public APiOffersGateway(string apiBaseUrl, ApiClient<OfferResponseData> apiClient, OffersValidator offersValidator)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiClient = apiClient;
        _offersValidator = offersValidator;
    }

    public Offer Retrieve(OfferId id)
    {
        try
        {
            var uri = CreateUriFor(id);
            var responseData = _apiClient.GetApiResponse(uri);
            var offer = CreateOfferFrom(responseData.First());
            _offersValidator.Validate(offer);
            return offer;
        }
        catch (InvalidOfferException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new OfferRetrievalException(e);
        }
    }

    private Offer CreateOfferFrom(OfferResponseData response)
    {
        return Offer.NotYetAccepted(new OfferId(response.OfferId), response.Price, response.ProductId);
    }

    private string CreateUriFor(OfferId id)
    {
        return $"{_apiBaseUrl}/offers/?version={ApiVersion}&offerId={id.AsText()}";
    }
}

public class OfferResponseData
{
    public OfferResponseData(string offerId, decimal price, string productId)
    {
        OfferId = offerId;
        Price = price;
        ProductId = productId;
    }

    public string OfferId { get; }
    public string ProductId { get; }
    public decimal Price { get; }
}