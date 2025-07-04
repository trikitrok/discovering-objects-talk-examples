using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Posts.infrastructure;
using UserAccount;

namespace Posts.Tests.unit.Infrastructure;


// kept only for comparison purposes
// we wouldn't write this test
// an acceptance test would cover this integration
public class ApiPostsGatewayOldIntegratedTest
{
    private const string ApiBaseUrl = "baseUri";
    private const int ApiVersion = 2;
    private const string UserId = "userId";
    private PostsGateway _postsGateway;
    private ApiClient<PostResponseData>? _apiClient;
    private MetricsSender? _metricsSender;
    private string _uri;

    [SetUp]
    public void SetUp()
    {
        _metricsSender = Substitute.For<MetricsSender>();
        _apiClient = Substitute.For<ApiClient<PostResponseData>>();
        _postsGateway = new PostGateWayWithMetrics(new ApiPostsGateway(ApiBaseUrl, _apiClient), _metricsSender);
        _uri = $"{ApiBaseUrl}/posts/?version={ApiVersion}&userId={UserId}";
    }

    [Test]
    public void retrieving_posts_for_a_user_successfully()
    {
        _apiClient.GetApiResponse(_uri).Returns(
            new List<PostResponseData>
            {
                new PostResponseData("userId", "postId1", "title1", "text1"),
                new PostResponseData("userId", "postId2", "title2", "text2")
            });

        var posts = _postsGateway.RetrievePostsFor(new User(new Id(UserId)));

        SuccessMetricsWereSent();
        Assert.That(posts, Is.EquivalentTo(
            new List<Post>
        {
            new Post(new Id("postId1"), "title1", "text1", new Id("userId")),
            new Post(new Id("postId2"), "title2", "text2", new Id("userId"))
        }));
    }

    [Test]
    public void retrieving_posts_for_a_user_fails()
    {
        _apiClient.GetApiResponse(_uri).Throws(x => { throw new Exception(); });

        Assert.Throws<PostRetrievalException>(() => _postsGateway.RetrievePostsFor(new User(new Id(UserId))));
        
        FailureMetricsWereSent();
    }

    private void SuccessMetricsWereSent()
    {
        PerformanceMetricsWereSent();
        _metricsSender.Received(1).IncrementCount("posts_api.success.get");
    }

    private void FailureMetricsWereSent()
    {
        PerformanceMetricsWereSent();
        _metricsSender.Received(1).IncrementCount("posts_api.errors.get");
    }
    
    private void PerformanceMetricsWereSent()
    {
        _metricsSender.Received(1).StartResponseTime("posts_api.response_time");
        _metricsSender.Received(1).EndResponseTime("posts_api.response_time");
    }
}