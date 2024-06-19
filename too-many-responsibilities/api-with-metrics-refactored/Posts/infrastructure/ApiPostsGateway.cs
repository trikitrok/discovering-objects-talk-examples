using System;
using System.Collections.Generic;
using System.Linq;
using UserAccount;

namespace Posts.infrastructure;

public class ApiPostsGateway : PostsGateway
{
    private const int ApiVersion = 2;
    private readonly string _apiBaseUrl;
    private readonly ApiClient<PostResponseData> _apiClient;

    public ApiPostsGateway(string apiBaseUrl, ApiClient<PostResponseData> apiClient)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiClient = apiClient;
    }

    public List<Post> RetrievePostsFor(User user)
    {
        try
        {
            var uri = CreateUriFor(user);
            var responseData = _apiClient.GetApiResponse(uri);
            return responseData.Select(CreatePostFrom).ToList();
        }
        catch(Exception e)
        {
            throw new PostRetrievalException(e);
        }
    }

    private Post CreatePostFrom(PostResponseData response)
    {
        return new Post(new Id(response.PostId), response.Title, response.Text, new Id(response.UserId));
    }

    private string CreateUriFor(User user)
    {
        return $"{_apiBaseUrl}/posts/?version={ApiVersion}&userId={user.Id().AsText()}";
    }
}

public class PostResponseData
{
    public string UserId { get; }
    public string PostId { get; }
    public string Title { get; }
    public string Text { get; }

    public PostResponseData(string userId, string postId, string title, string text)
    {
        UserId = userId;
        PostId = postId;
        Title = title;
        Text = text;
    }
}