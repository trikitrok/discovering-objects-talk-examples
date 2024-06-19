using System;
using UserAccount;

namespace Posts;

public class Post
{
    private readonly Id _id;
    private readonly string _responseText;
    private readonly string _responseTitle;
    private readonly Id _userId;

    public Post(Id id, string responseTitle, string responseText, Id userId)
    {
        _responseTitle = responseTitle;
        _responseText = responseText;
        _userId = userId;
        _id = id;
    }

    protected bool Equals(Post other)
    {
        return Equals(_id, other._id) && _responseText == other._responseText && _responseTitle == other._responseTitle && Equals(_userId, other._userId);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Post)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_id, _responseText, _responseTitle, _userId);
    }

    public override string ToString()
    {
        return
            $"{nameof(_id)}: {_id}, {nameof(_responseText)}: {_responseText}, {nameof(_responseTitle)}: {_responseTitle}, {nameof(_userId)}: {_userId}";
    }
}