using System;
using System.Linq;

namespace Offers.infrastructure;

public class APiOffersGateway : OffersGateway
{
    private const int ApiVersion = 2;
    private readonly string _apiBaseUrl;
    private readonly ApiClient<OfferResponseData> _apiClient;

    public APiOffersGateway(string apiBaseUrl, ApiClient<OfferResponseData> apiClient)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiClient = apiClient;
    }
    
    public Offer Retrieve(OfferId id)
    {
        try
        {
            var uri = CreateUriFor(id);
            var responseData = _apiClient.GetApiResponse(uri);
            return CreateOfferFrom(responseData.First());
        }
        catch(Exception e)
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
    public string OfferId { get; }
    public string ProductId { get; }
    public decimal Price { get; }

    public OfferResponseData(string offerId, decimal price, string productId)
    {
        OfferId = offerId;
        Price = price;
        ProductId = productId;
    }
}