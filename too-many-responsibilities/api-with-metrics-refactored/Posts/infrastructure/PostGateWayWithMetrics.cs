using System;
using System.Collections.Generic;

namespace Posts.infrastructure;

public class PostGateWayWithMetrics : PostsGateway
{
    private const string PostsApiResponseTimeMetricsKey = "posts_api.response_time";
    private const string PostsApiSuccessGetMetricsKey = "posts_api.success.get";
    private const string PostsApiErrorsGetMetricsKey = "posts_api.errors.get";
    
    private readonly PostsGateway _postsGateway;
    private readonly MetricsSender _metricsSender;

    public PostGateWayWithMetrics(PostsGateway postsGateway, MetricsSender metricsSender)
    {
        _postsGateway = postsGateway;
        _metricsSender = metricsSender;
    }
    
    public List<Post> RetrievePostsFor(User user)
    {
        _metricsSender.StartResponseTime(PostsApiResponseTimeMetricsKey);
        try
        {
            var posts = _postsGateway.RetrievePostsFor(user);
            _metricsSender.IncrementCount(PostsApiSuccessGetMetricsKey);
            return posts;
        }
        catch (PostRetrievalException e)
        {
            _metricsSender.IncrementCount(PostsApiErrorsGetMetricsKey);
            throw new PostRetrievalException(e);
        }
        finally
        {
            _metricsSender.EndResponseTime(PostsApiResponseTimeMetricsKey);    
        }
    }
}