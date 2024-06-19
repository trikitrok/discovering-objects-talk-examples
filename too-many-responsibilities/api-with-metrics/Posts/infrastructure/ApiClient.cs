using System.Collections.Generic;

namespace Posts.infrastructure;

public interface ApiClient<T>
{
    List<T> GetApiResponse(string uri);
}