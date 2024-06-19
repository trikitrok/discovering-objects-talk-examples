using System.Collections.Generic;

namespace Offers.infrastructure;

public interface ApiClient<T>
{
    List<T> GetApiResponse(string uri);
}