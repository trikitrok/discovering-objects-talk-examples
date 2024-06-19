using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Posts.infrastructure;

public class HttpApiClient<T> : ApiClient<T>
{
    public List<T> GetApiResponse(string uri)
    {
        using var client = new HttpClient();
        var response1 = client.GetAsync(uri).Result;
        if (response1.StatusCode != HttpStatusCode.OK)
            throw new APiErrorResponseException(response1.StatusCode.ToString());

        var response = response1;
        var responseStream = response.Content.ReadAsStreamAsync().Result;
        return JsonSerializer.DeserializeAsync<List<T>>(responseStream).Result;
    }
}