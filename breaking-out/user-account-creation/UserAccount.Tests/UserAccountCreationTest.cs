using NSubstitute;
using NUnit.Framework;

namespace UserAccount.Tests;

public class UserAccountCreationTest
{
    private AccountsRepository _accountsRepository;
    private Notifier _notifier;
    private UserAccountCreation _userAccountCreation;

    [SetUp]
    public void Setup()
    {
        _notifier = Substitute.For<Notifier>();
        _accountsRepository = Substitute.For<AccountsRepository>();
        _userAccountCreation = new UserAccountCreation(_notifier, _accountsRepository, new Encrypter());
    }
    
    // a couple of tests to test drive UserAccountCreation  
    // ...
    
    // gazillions of tests through UserAccountCreation that test drive user data validation
    // ...
}