namespace Posts.infrastructure;

public interface MetricsSender
{
    void IncrementCount(string key);

    void StartResponseTime(string key);

    void EndResponseTime(string key);
}