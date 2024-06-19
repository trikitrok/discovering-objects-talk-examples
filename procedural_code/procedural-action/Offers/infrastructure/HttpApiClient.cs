using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Offers.infrastructure;

using static System.Text.Json.JsonSerializer;

public class HttpApiClient<T> : ApiClient<T>
{
    public List<T> GetApiResponse(string uri)
    {
        using var client = new HttpClient();
        var response = client.GetAsync(uri).Result;
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new APiErrorResponseException(response.StatusCode.ToString());
        }
        var responseStream = response.Content.ReadAsStreamAsync().Result;
        return DeserializeAsync<List<T>>(responseStream).Result;
    }
}