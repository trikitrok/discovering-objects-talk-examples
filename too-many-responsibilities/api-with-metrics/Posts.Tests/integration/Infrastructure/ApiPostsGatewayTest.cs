using NUnit.Framework;
using Posts.infrastructure;

namespace Posts.Tests.integration.Infrastructure;

public class ApiPostsGatewayTest
{
    private const string ApiBaseUrl = "blabla";
    private ApiPostsGateway _apiPostsGateway;

    [SetUp]
    public void SetUp()
    {
        _apiPostsGateway = new ApiPostsGateway(ApiBaseUrl);
    }
    
    // some integration tests
    // 
}