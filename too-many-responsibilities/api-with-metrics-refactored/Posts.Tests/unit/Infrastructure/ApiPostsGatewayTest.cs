using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using Posts.infrastructure;
using UserAccount;

namespace Posts.Tests.unit.Infrastructure;

public class ApiPostsGatewayTest
{
    private const string ApiBaseUrl = "baseUri";
    private const int ApiVersion = 2;
    private const string UserId = "userId";
    private PostsGateway _postsGateway;
    private ApiClient<PostResponseData> _apiClient;
    private string _uri;
    private User _user;

    [SetUp]
    public void SetUp()
    {
        _user = new User(new Id(UserId));
        _apiClient = Substitute.For<ApiClient<PostResponseData>>();
        _postsGateway = new ApiPostsGateway(ApiBaseUrl, _apiClient);
        _uri = $"{ApiBaseUrl}/posts/?version={ApiVersion}&userId={UserId}";
    }

    [Test]
    public void retrieving_posts_for_a_user_successfully()
    {
        _apiClient.GetApiResponse(_uri).Returns(
            new List<PostResponseData>
            {
                new PostResponseData(UserId, "postId1", "title1", "text1"),
                new PostResponseData(UserId, "postId2", "title2", "text2")
            });

        var posts = _postsGateway.RetrievePostsFor(_user);
        
        Assert.That(posts, Is.EquivalentTo(
            new List<Post>
        {
            new Post(new Id("postId1"), "title1", "text1", new Id(UserId)),
            new Post(new Id("postId2"), "title2", "text2", new Id(UserId))
        }));
    }

    [Test]
    public void retrieving_posts_for_a_user_fails()
    {
        _apiClient.GetApiResponse(_uri).Throws(x => { throw new Exception(); });

        Assert.Throws<PostRetrievalException>(() => _postsGateway.RetrievePostsFor(_user));
    }
}