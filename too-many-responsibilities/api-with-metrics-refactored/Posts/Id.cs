namespace UserAccount;

public class Id
{
    private string _id;

    public Id(string id)
    {
        _id = id;
    }

    public string AsText()
    {
        return _id;
    }
}