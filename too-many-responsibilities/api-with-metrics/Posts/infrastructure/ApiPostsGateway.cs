using System;
using System.Collections.Generic;
using UserAccount;

namespace Posts.infrastructure;

public class ApiPostsGateway : PostsGateway
{
    private readonly string _apiBaseUrl;
    private const int ApiVersion = 2;
    
    public ApiPostsGateway(string apiBaseUrl)
    {
        _apiBaseUrl = apiBaseUrl;
    }

    public List<Post> RetrievePostsFor(User user)
    {
        var posts = new List<Post>();
        try
        {
            var apiClient = new HttpGenericApiClient<PostData>();

            var uri = CreateUriFor(user);
            
            var responseData = apiClient.GetApiResponse(uri);

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
    
    private class PostData
    {
        public string userId { get; set; }
        public string postId { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }
}