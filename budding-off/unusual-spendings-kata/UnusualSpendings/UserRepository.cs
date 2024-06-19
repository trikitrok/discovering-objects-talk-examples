namespace UnusualSpendings;

public interface UserRepository
{
    UserContactData GetContactData(User user);
}