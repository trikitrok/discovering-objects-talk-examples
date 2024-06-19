using NUnit.Framework;
using Posts.infrastructure;

namespace Posts.Tests.unit.Infrastructure;

public class ApiPostsGatewayTest
{
    private const string ApiBaseUrl = "blabla";
    private ApiPostsGateway _apiPostsGateway;

    [SetUp]
    public void SetUp()
    {
        _apiPostsGateway = new ApiPostsGateway(ApiBaseUrl, new HttpApiClient<PostData>());
    }

    // some integration tests
    // 
}