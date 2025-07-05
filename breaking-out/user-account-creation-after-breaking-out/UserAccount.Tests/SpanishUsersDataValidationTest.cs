using NUnit.Framework;

namespace UserAccount.Tests;

public class SpanishUsersDataValidationTest
{
    private const string Name = "some name";
    private const string ValidCreditCardNumber = "0000000000000000";
    private static readonly HashSet<string> InvalidPrefixes = new() { "X", "Y", "Z", "K", "L", "M" };

    private SpanishUsersDataValidation _spanishUsersDataValidation;

    [SetUp]
    public void Setup()
    {
        _spanishUsersDataValidation = new SpanishUsersDataValidation();
    }

    // Gazillions of tests that are directly testing the user data validation

    [TestCase("12345678Z")]
    [TestCase("87654321X")]
    [TestCase("11223344B")]
    [TestCase("56838246W")]
    public void User_Data_Is_Valid_When_Valid_Dni_Is_Provided(string validDni)
    {
        var userWithValidDni = new UserData(
            new Id(validDni),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(userWithValidDni), Is.False);
    }

    [TestCase("X1234567L")]
    [TestCase("Y8220072R")]
    [TestCase("Z1607027E")]
    [TestCase("Z5114162W")]
    public void User_Data_Is_Valid_When_Valid_Nie_Is_Provided(string validNie)
    {
        var userWithValidNie = new UserData(
            new Id(validNie),
            Name,
            ValidCreditCardNumber
        );


        Assert.That(_spanishUsersDataValidation.IsNotValid(userWithValidNie), Is.False);
    }

    [Test]
    [TestCase("V68592245")]
    [TestCase("N9546338F")]
    [TestCase("P3171012B")]
    public void User_Data_Is_Valid_When_Valid_Cif_Is_Provided(string validCif)
    {
        var userWithValidCif = new UserData(
            new Id(validCif),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(userWithValidCif), Is.False);
    }

    [Test]
    public void User_Data_Is_Not_Valid_When_Id_Contains_Whitespace()
    {
        var userWithWhitespaceId = new UserData(
            new Id(" 12345 678A "),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(userWithWhitespaceId), Is.True);
    }

    [Test]
    public void User_Data_Is_Not_Valid_When_Id_Is_Of_Unknown_Type()
    {
        var unknownTypeIdUser = new UserData(
            new Id("911111111"),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(unknownTypeIdUser), Is.True);
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

        Assert.That(_spanishUsersDataValidation.IsNotValid(invalidDniUser), Is.True);
    }

    [TestCase("X12345678")]
    [TestCase("A12#3456")]
    [TestCase("Z1234")]
    [TestCase("B999999999")]
    [TestCase("B12345679")]
    public void User_Data_Is_Not_Valid_When_Cif_Is_Invalid(string invalidCif)
    {
        var userWithInvalidCif = new UserData(
            new Id(invalidCif),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(userWithInvalidCif), Is.True);
    }

    [TestCase("X9999999T")]
    [TestCase("Y2345678M")]
    [TestCase("Z3456789N")]
    [TestCase("X4567890L")]
    [TestCase("ZABCDEFGH")]
    [TestCase("A1234567B")]
    public void User_Data_Is_Not_Valid_When_Nie_Dni_Digits_Invalid(string invalidNie)
    {
        var invalidNieUser = new UserData(
            new Id(invalidNie),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(invalidNieUser), Is.True);
    }

    [TestCaseSource(nameof(InvalidNieOrDniPrefixes))]
    public void User_Data_Is_Not_Valid_When_Nie_Prefix_Is_Invalid(string prefix)
    {
        var wrongNiePrefixUser = new UserData(
            new Id(prefix + "1234567L"),
            Name,
            ValidCreditCardNumber
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(wrongNiePrefixUser), Is.True);
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
    public void User_Data_Is_Valid_For_Many_Valid_Credit_Cards_And_Same_Valid_Id(string validCreditCard)
    {
        var userData = new UserData(
            new Id("12345678Z"),
            Name,
            validCreditCard
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(userData), Is.False);
    }

    [TestCase("1234567812345678")]
    [TestCase("6011261646972670")]
    [TestCase("4111111111111112")]
    [TestCase("123456789012345")]
    public void User_Data_Is_Not_Valid_Because_Of_Invalid_Credit_Cards(string invalidCreditCard)
    {
        var userData = new UserData(
            new Id("12345678Z"),
            Name,
            invalidCreditCard
        );

        Assert.That(_spanishUsersDataValidation.IsNotValid(userData), Is.True);
    }
}