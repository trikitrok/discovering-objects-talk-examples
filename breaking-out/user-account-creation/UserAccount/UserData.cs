namespace UserAccount;

public class UserData
{
    private readonly string _creditCardNumber;
    private readonly string _fullName;
    private readonly Id _id;

    public UserData(Id id, string fullName, string creditCardNumber)
    {
        _id = id;
        _fullName = fullName;
        _creditCardNumber = creditCardNumber;
    }

    public string FullName()
    {
        return _fullName;
    }

    public Id SpanishId()
    {
        return _id;
    }

    public string CreditCardNumber()
    {
        return _creditCardNumber;
    }
}