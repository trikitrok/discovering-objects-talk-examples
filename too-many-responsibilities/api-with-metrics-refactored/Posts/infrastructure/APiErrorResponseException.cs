using System;

namespace Posts.infrastructure;

public class APiErrorResponseException : Exception
{
    public APiErrorResponseException(string message) : base("Received error response: " + message)
    {
    }
}