namespace Security;

public class SimulatedSecurityManager
{
    private readonly Encrypter _encrypter;
    private readonly Notifier _notifier;
    private readonly UserDataRequester _userDataRequester;

    public SimulatedSecurityManager(Notifier notifier, UserDataRequester userDataRequester)
    {
        _notifier = notifier;
        _userDataRequester = userDataRequester;
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
        _notifier.Notify(
            $"Saving Details for User ({userData.UserName()}, {userData.FullName()}, {userData.EncryptPassword(_encrypter)})\n"
        );
    }
}