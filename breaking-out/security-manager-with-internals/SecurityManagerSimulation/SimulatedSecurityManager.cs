namespace Security;

public class SimulatedSecurityManager
{
    private readonly Encrypter _encrypter;
    private readonly Notifier _notifier;
    private readonly UserDataRequester _userDataRequester;

    public SimulatedSecurityManager(Notifier notifier, InputReader input)
    {
        _notifier = notifier;
        _userDataRequester = new UserDataRequester(input);
        _encrypter = new Encrypter();
    }

    public void CreateValidUser()
    {
        var userData = _userDataRequester.Request();

        if (userData.PasswordsDoNotMatch())
        {
            NotifyPasswordDoNotMatch();
            return;
        }

        if (userData.IsPasswordToShort())
        {
            NotifyPasswordIsToShort();
            return;
        }

        NotifyUserCreation(userData);
    }

    private void NotifyPasswordIsToShort()
    {
        _notifier.Notify("Password must be at least 8 characters in length");
    }

    private void NotifyPasswordDoNotMatch()
    {
        _notifier.Notify("The passwords don't match");
    }

    private void NotifyUserCreation(UserData userData)
    {
        var encryptPassword = userData.EncryptPassword(_encrypter);
        _notifier.Notify(
            $"Saving Details for User ({userData.UserName()}, {userData.FullName()}, {encryptPassword})\n"
        );
    }
}