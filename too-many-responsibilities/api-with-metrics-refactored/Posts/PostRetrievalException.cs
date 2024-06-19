using System;

namespace Posts;

public class PostRetrievalException : Exception
{
    public PostRetrievalException(Exception exception) : base(exception.Message, exception)
    {
    }
}