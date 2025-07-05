using NSubstitute;
using NUnit.Framework;

namespace UserAccount.Tests;

public class UserAccountCreationTest
{
    private static readonly HashSet<string> InvalidPrefixes = new() { "X", "Y", "Z", "K", "L", "M" };
    private const string Name = "some name";
    private const string ValidCreditCardNumber = "0000000000000000";
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
    
    // There are only a couple of tests that serve to test UserAccountCreation logic,    
    // and gazillions of tests that are actually testing the user data validation
    
    [TestCase("12345678Z")]
    [TestCase("87654321X")]
    [TestCase("11223344B")]
    [TestCase("56838246W")]
    public void Can_Create_User_Account_When_Valid_Dni_Is_Provided(string validDni)
    {
        var userWithValidDni = new UserData(
            new Id(validDni),
            Name,
            ValidCreditCardNumber
        );

        _userAccountCreation.CreateUserAccount(userWithValidDni);

        _notifier.Received(1).Notify($"Saving Account for User ({userWithValidDni.FullName()})\n");
        _accountsRepository.Received(1).Save(
            new Account(
                userWithValidDni.FullName(),
                userWithValidDni.SpanishId(),
                ValidCreditCardNumber.Reverse().ToString())
        );
    }

    [TestCase("X1234567L")]
    [TestCase("Y8220072R")]
    [TestCase("Z1607027E")]
    [TestCase("Z5114162W")]
    public void Can_Create_User_Account_When_Valid_Nie_Is_Provided(string validNie)
    {
        var userWithValidNie = new UserData(
            new Id(validNie),
            Name,
            ValidCreditCardNumber
        );
        _userAccountCreation.CreateUserAccount(userWithValidNie);
        _notifier.Received(1).Notify($"Saving Account for User ({userWithValidNie.FullName()})\n");
        _accountsRepository.Received(1).Save(
            new Account(
                userWithValidNie.FullName(),
                userWithValidNie.SpanishId(),
                ValidCreditCardNumber.Reverse().ToString())
        );
    }

    [Test]
    [TestCase("V68592245")]
    [TestCase("N9546338F")]
    [TestCase("P3171012B")]
    public void Can_Create_User_Account_When_Valid_Cif_Is_Provided(string validCif)
    {
        var userWithValidCif = new UserData(
            new Id(validCif),
            Name,
            ValidCreditCardNumber
        );

        _userAccountCreation.CreateUserAccount(userWithValidCif);

        _notifier.Received(1).Notify($"Saving Account for User ({userWithValidCif.FullName()})\n");
        _accountsRepository.Received(1).Save(
            new Account(
                userWithValidCif.FullName(),
                userWithValidCif.SpanishId(),
                ValidCreditCardNumber.Reverse().ToString())
        );
    }
    
    [Test]
    public void Can_Not_Create_Account_When_Id_Contains_Whitespace()
    {
        var userWithWhitespaceId = new UserData(
            new Id(" 12345 678A "),
            Name,
            ValidCreditCardNumber
        );

        _userAccountCreation.CreateUserAccount(userWithWhitespaceId);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    [Test]
    public void Can_Not_Create_Account_When_Id_Is_Of_Unknown_Type()
    {
        var unknownTypeIdUser = new UserData(
            new Id("911111111"),
            Name,
            ValidCreditCardNumber
        );

        _userAccountCreation.CreateUserAccount(unknownTypeIdUser);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    [TestCase("99887766M")]
    [TestCase("12345678X")]
    [TestCase("ABCDEFGH9")]
    [TestCase("")]
    public void Can_Not_Create_User_Account_When_Dni_Is_Invalid(string invalidDni)
    {
        var invalidDniUser = new UserData(
            new Id(invalidDni),
            Name,
            ValidCreditCardNumber
        );
        _userAccountCreation.CreateUserAccount(invalidDniUser);
        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    [TestCase("X12345678")]
    [TestCase("A12#3456")]
    [TestCase("Z1234")]
    [TestCase("B999999999")]
    [TestCase("B12345679")]
    public void Can_Not_Create_Account_When_Cif_Is_Invalid(string invalidCif)
    {
        var userWithInvalidCif = new UserData(
            new Id(invalidCif),
            Name,
            ValidCreditCardNumber
        );

        _userAccountCreation.CreateUserAccount(userWithInvalidCif);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    [TestCase("X9999999T")]
    [TestCase("Y2345678M")]
    [TestCase("Z3456789N")]
    [TestCase("X4567890L")]
    [TestCase("ZABCDEFGH")]
    [TestCase("A1234567B")]
    public void Can_Not_Create_Account_When_Nie_Dni_Digits_Invalid(string invalidNie)
    {
        var invalidNieUser = new UserData(
            new Id(invalidNie),
            Name,
            ValidCreditCardNumber
        );
        _userAccountCreation.CreateUserAccount(invalidNieUser);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    [TestCaseSource(nameof(InvalidNieOrDniPrefixes))]
    public void Can_Not_Create_Account_When_Nie_Prefix_Is_Invalid(string prefix)
    {
        var wrongNiePrefixUser = new UserData(
            new Id(prefix + "1234567L"),
            Name,
            ValidCreditCardNumber
        );

        _userAccountCreation.CreateUserAccount(wrongNiePrefixUser);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }

    private static IEnumerable<string> InvalidNieOrDniPrefixes()
    {
        return Enumerable.Range('A', 26)
            .Select(i => ((char)i).ToString())
            .Where(c => !InvalidPrefixes.Contains(c));
    }
    
    [TestCase("4539578763621486")]
    [TestCase("4716871917909534")]
    [TestCase("4539682995824395")] 
    [TestCase("6011111111111117")] 
    [TestCase("378282246310005")]  
    [TestCase("5105105105105100")] 
    [TestCase("371449635398431")]  
    [TestCase("5555555555554444")] 
    [TestCase("49927398716")]
    public void Can_Create_Account_When_Credit_Card_Is_Valid(string validCreditCard)
    {
        var user = new UserData(
            new Id("12345678Z"),
            Name,
            validCreditCard
        );

        _userAccountCreation.CreateUserAccount(user);

        _notifier.Received(1).Notify($"Saving Account for User ({user.FullName()})\n");
        _accountsRepository.Received(1).Save(
            new Account(
                user.FullName(),
                user.SpanishId(),
                ValidCreditCardNumber.Reverse().ToString())
        );
    }

    [TestCase("1234567812345678")]
    [TestCase("6011261646972670")]
    [TestCase("4111111111111112")]
    [TestCase("123456789012345")]
    public void Can_Not_Create_Account_When_Credit_Card_Is_Invalid(string invalidCreditCard)
    {
        var user = new UserData(
            new Id("12345678Z"),
            Name,
            invalidCreditCard
        );

        _userAccountCreation.CreateUserAccount(user);

        _notifier.Received(1).Notify("Could not create account due to invalid user data.\n");
        _accountsRepository.DidNotReceive().Save(Arg.Any<Account>());
    }
}