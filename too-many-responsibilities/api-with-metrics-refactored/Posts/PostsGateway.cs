using System.Collections.Generic;
using UserAccount;

namespace Posts;

public interface PostsGateway
{
    public List<Post> RetrievePostsFor(User user);
}