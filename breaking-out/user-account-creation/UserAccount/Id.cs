namespace UserAccount;

public class Id
{
    private string _digits;

    public Id(string digits)
    {
        _digits = digits;
    }

    public string Digits()
    {
        return _digits;
    }
}