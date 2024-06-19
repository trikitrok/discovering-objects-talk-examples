using NSubstitute;
using NUnit.Framework;
using Posts.infrastructure;
using UserAccount;

namespace Posts.Tests.unit.Infrastructure;

public class ApiPostsGatewayTest
{
    private const string ApiBaseUrl = "blabla";
    private const int ApiVersion = 2;
    private ApiPostsGateway _apiPostsGateway;
    private ApiClient<PostData>? _apiClient;
    private MetricsSender? _metricsSender;

    [SetUp]
    public void SetUp()
    {
        _metricsSender = Substitute.For<MetricsSender>();
        _apiClient = Substitute.For<ApiClient<PostData>>();
        _apiPostsGateway = new ApiPostsGateway(ApiBaseUrl, _apiClient, _metricsSender);
    }

    [Test]
    public void retrieve_posts_for_a_user_successfully()
    {
        var userId = "userId";
        var uri = $"{ApiBaseUrl}/posts/?version={ApiVersion}&userId={userId}";
        _apiClient.GetApiResponse(uri).Returns(
            new List<PostData>
            {
                new PostData("userId", "postId1", "title1", "text1"),
                new PostData("userId", "postId2", "title2", "text2")
            });

        var posts = _apiPostsGateway.RetrievePostsFor(new User(new Id(userId)));

        _metricsSender.Received(1).StartResponseTime("posts_api.response_time");
        _metricsSender.Received(1).EndResponseTime("posts_api.response_time");
        _metricsSender.Received(1).IncrementCount("posts_api.success.get");
        Assert.That(posts, Is.EquivalentTo(
            new List<Post>
        {
            new Post(new Id("postId1"), "title1", "text1", new Id("userId")),
            new Post(new Id("postId2"), "title2", "text2", new Id("userId"))
        }));
    }

    // some unit tests
}