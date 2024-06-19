using System;
using System.Collections.Generic;
using UserAccount;

namespace Posts.infrastructure;

public class ApiPostsGateway : PostsGateway
{
    private const int ApiVersion = 2;
    private const string PostsApiResponseTimeMetricsKey = "posts_api.response_time";
    private const string PostsApiSuccessGetMetricsKey = "posts_api.success.get";
    private const string PostsApiErrorsGetMetricsKey = "posts_api.errors.get";
    private readonly string _apiBaseUrl;
    private readonly ApiClient<PostData> _apiClient;
    private readonly MetricsSender _metricsSender;

    public ApiPostsGateway(string apiBaseUrl, ApiClient<PostData> apiClient, MetricsSender metricsSender)
    {
        _apiBaseUrl = apiBaseUrl;
        _apiClient = apiClient;
        _metricsSender = metricsSender;
    }

    public List<Post> RetrievePostsFor(User user)
    {
        _metricsSender.StartResponseTime(PostsApiResponseTimeMetricsKey);
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

            _metricsSender.IncrementCount(PostsApiSuccessGetMetricsKey);
            return posts;
        }
        catch (Exception e)
        {
            _metricsSender.IncrementCount(PostsApiErrorsGetMetricsKey);
            throw new PostRetrievalException(e);
        }
        finally
        {
            _metricsSender.EndResponseTime(PostsApiResponseTimeMetricsKey);    
        }
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