using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Posts.infrastructure;
using UserAccount;

namespace Posts.Tests.unit.Infrastructure;

public class PostGateWayWithMetricsTest
{
    private const string UserId = "userId";
    private PostsGateway _postsGateway;
    private PostsGateway _concretePostGateway;
    private MetricsSender _metricsSender;
    private User _user;

    [SetUp]
    public void SetUp()
    {
        _concretePostGateway = Substitute.For<PostsGateway>();
        _metricsSender = Substitute.For<MetricsSender>();
        _postsGateway = new PostGateWayWithMetrics(_concretePostGateway, _metricsSender);
        _user = new User(new Id(UserId));
    }

    [Test]
    public void retrieving_posts_for_a_user_successfully()
    {
        var retrievedPosts = new List<Post>
        {
            new Post(new Id("postId1"), "title1", "text1", new Id(UserId)),
            new Post(new Id("postId2"), "title2", "text2", new Id(UserId))
        };
        _concretePostGateway.RetrievePostsFor(_user).Returns(retrievedPosts);

        var posts = _postsGateway.RetrievePostsFor(_user);

        SuccessMetricWereSent();
        Assert.That(posts, Is.EquivalentTo(retrievedPosts));
    }

    [Test]
    public void retrieving_posts_for_a_user_fails()
    {
        _concretePostGateway.RetrievePostsFor(_user).Throws(x => { throw new PostRetrievalException(new Exception()); });

        Assert.Throws<PostRetrievalException>(() => _postsGateway.RetrievePostsFor(_user));
        
        FailureMetricsWereSent();
    }

    private void SuccessMetricWereSent()
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