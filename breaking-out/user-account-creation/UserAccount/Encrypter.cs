using System.Linq;

namespace UserAccount;

public class Encrypter
{
    public string Encrypt(string text)
    {
        return text.Reverse().ToString();
    }
}