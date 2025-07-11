namespace UserAccount;

public class UserAccountCreation
{
    private readonly AccountsRepository _accountsRepository;
    private readonly Encrypter _encrypter;
    private readonly Notifier _notifier;
    private readonly UsersDataValidation _userDataValidation;

    public UserAccountCreation(Notifier notifier, AccountsRepository accountsRepository, Encrypter encrypter,
        UsersDataValidation userDataValidation)
    {
        _notifier = notifier;
        _accountsRepository = accountsRepository;
        _encrypter = encrypter;
        _userDataValidation = userDataValidation;
    }

    public void CreateUserAccount(UserData userData)
    {
        if (_userDataValidation.IsNotValid(userData))
        {
            NotifyWrongUserData();
            return;
        }

        CreateAccountFrom(userData);
        NotifyUserAccountCreation(userData.FullName());
    }

    private void CreateAccountFrom(UserData userData)
    {
        _accountsRepository.Save(
            new Account(
                userData.FullName(),
                userData.Id(),
                _encrypter.Encrypt(userData.CreditCardNumber())
            )
        );
    }

    private void NotifyWrongUserData()
    {
        _notifier.Notify("Could not create account due to invalid user data.\n");
    }

    private void NotifyUserAccountCreation(string fullName)
    {
        _notifier.Notify($"Saving Account for User ({fullName})\n");
    }
}