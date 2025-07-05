using NSubstitute;
using NUnit.Framework;

namespace UserAccount.Tests;

public class UserAccountCreationTest
{
    private AccountsRepository _accountsRepository;
    private Notifier _notifier;
    private UserAccountCreation _userAccountCreation;
    private UsersDataValidation _usersDataValidation;

    [SetUp]
    public void Setup()
    {
        _notifier = Substitute.For<Notifier>();
        _accountsRepository = Substitute.For<AccountsRepository>();
        _usersDataValidation = Substitute.For<UsersDataValidation>();
        _userAccountCreation =
            new UserAccountCreation(_notifier, _accountsRepository, new Encrypter(), _usersDataValidation);
    }

    // Only a couple of tests that serve to test UserAccountCreation logic   

    [Test]
    public void Can_Create_User_Account_When_Valid_User_Data_Provided()
    {
        var validUserData = AnyUserData();
        _usersDataValidation.IsNotValid(validUserData).Returns(false);

        _userAccountCreation.CreateUserAccount(validUserData);

        _notifier.Received(1).Notify($"Saving Account for User ({validUserData.FullName()})\n");
        _accountsRepository.Received(1).Save(
            new Account(
                validUserData.FullName(),
                validUserData.Id(),
                validUserData.CreditCardNumber().Reverse().ToString())
        );
    }

    [Test]
    public void Can_Not_Create_Account_When_Valid_User_Data_Is_Not_Provided()
    {
        var anyUserData = AnyUserData();
        _usersDataValidation.IsNotValid(anyUserData).Returns(true);

        _userAccountCreation.CreateUserAccount(anyUserData);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    private static UserData AnyUserData()
    {
        return new UserData(
            new Id("anyId"),
            "Pepe",
            "157895723057029"
        );
    }
}