using NSubstitute;
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
        var metricsSender = Substitute.For<MetricsSender>();
        var apiClient = Substitute.For<ApiClient<PostData>>();
        _apiPostsGateway = new ApiPostsGateway(ApiBaseUrl, apiClient, metricsSender);
    }

    // some unit tests
    // 
}