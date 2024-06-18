using System;

namespace Security;

public class Encrypter
{
    public string Encrypt(string text)
    {
        var array = text.ToCharArray();
        Array.Reverse(array);
        var encryptedPassword = new string(array);
        return encryptedPassword;
    }
}