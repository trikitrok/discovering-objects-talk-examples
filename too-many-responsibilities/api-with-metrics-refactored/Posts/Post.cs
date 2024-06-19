using UserAccount;

namespace Posts;

public class Post
{
    private readonly Id _id;
    private readonly string _responseTitle;
    private readonly string _responseText;
    private readonly Id _userId;
    
    public Post(Id id, string responseTitle, string responseText, Id userId)
    {
        _responseTitle = responseTitle;
        _responseText = responseText;
        _userId = userId;
        _id = id;
    }
}