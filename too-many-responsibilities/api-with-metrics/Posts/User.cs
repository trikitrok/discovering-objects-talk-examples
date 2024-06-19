using UserAccount;

namespace Posts;

public class User
{
    private readonly Id _id;

    public User(Id id)
    {
        _id = id;
    }

    public Id Id()
    {
        return _id;
    }
}