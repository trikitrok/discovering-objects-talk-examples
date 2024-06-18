namespace UserAccount;

public class Account
{
    private readonly Id _id;
    private readonly string _encryptedCreditCardNumber;
    private readonly string _fullName;

    public Account(string fullName, Id id, string encryptedCreditCardNumber)
    {
        _fullName = fullName;
        _id = id;
        _encryptedCreditCardNumber = encryptedCreditCardNumber;
    }
}