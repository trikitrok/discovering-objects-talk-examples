namespace Security;

public class TextUserDataRequester : UserDataRequester
{
    private readonly InputReader _inputReader;

    public TextUserDataRequester(InputReader inputReader)
    {
        _inputReader = inputReader;
    }

    public UserData Request()
    {
        return new UserData(
            RequestUserName(),
            RequestFullName(),
            RequestPassword(),
            RequestPasswordConfirmation()
        );
    }

    private string RequestPasswordConfirmation()
    {
        return _inputReader.Read("Re-enter your password");
    }

    private string RequestPassword()
    {
        return _inputReader.Read("Enter your password");
    }

    private string RequestFullName()
    {
        return _inputReader.Read("Enter your full name");
    }

    private string RequestUserName()
    {
        return _inputReader.Read("Enter a username");
    }
}