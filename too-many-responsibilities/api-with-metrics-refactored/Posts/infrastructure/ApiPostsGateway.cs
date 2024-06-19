using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
            using var client = new HttpClient();
            var response = RequestResponse(client, CreateUriFor(user));
            var responseStream = response.Content.ReadAsStreamAsync().Result;
            var responseData = JsonSerializer.DeserializeAsync<List<PostApiResponse>>(responseStream).Result;
            
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

    private HttpResponseMessage RequestResponse(HttpClient client, string uri)
    {
        var response = client.GetAsync(uri).Result;
        if (response.StatusCode != HttpStatusCode.OK) {
            throw new APiErrorResponseException( response.StatusCode.ToString());
        }
        return response;
    }

    private Post CreatePostFrom(PostApiResponse response)
    {
        return new Post(new Id(response.postId), response.title, response.text, new Id(response.userId));
    }

    private string CreateUriFor(User user)
    {
        return _apiBaseUrl + "/posts?version=" + ApiVersion + "&" + user.Id().AsText();
    }
    
    private class PostApiResponse
    {
        public string userId { get; set; }
        public string postId { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }
}