using System.Collections.Generic;

namespace Posts;

public interface PostsGateway
{
    public List<Post> RetrievePostsFor(User user);
}