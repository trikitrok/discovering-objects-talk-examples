using System;
using System.Collections.Generic;
using UserAccount;

namespace Posts.infrastructure;

public class ApiPostsGateway : PostsGateway
{
    private const int ApiVersion = 2;
    private readonly string _apiBaseUrl;
    private readonly ApiClient<PostData> _apiClient;

    public ApiPostsGateway(string apiBaseUrl, ApiClient<PostData> apiClient)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiClient = apiClient;
    }

    public List<Post> RetrievePostsFor(User user)
    {
        var posts = new List<Post>();
        try
        {
            var uri = CreateUriFor(user);

            var responseData = _apiClient.GetApiResponse(uri);

            foreach (var postResponse in responseData)
            {
                var post = CreatePostFrom(postResponse);
                posts.Add(post);
            }
        }
        catch (Exception e)
        {
            throw new PostRetrievalException(e);
        }

        return posts;
    }

    private Post CreatePostFrom(PostData response)
    {
        return new Post(new Id(response.postId), response.title, response.text, new Id(response.userId));
    }

    private string CreateUriFor(User user)
    {
        return _apiBaseUrl + "/posts?version=" + ApiVersion + "&" + user.Id().AsText();
    }
}

public class PostData
{
    public string userId { get; }
    public string postId { get; }
    public string title { get; }
    public string text { get; }
}