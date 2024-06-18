using NSubstitute;
using NUnit.Framework;

namespace Security.Tests.Unit;

public class TextUserDataRequesterTest
{
    private InputReader _inputReader;
    private TextUserDataRequester _userDataRequester;

    [SetUp]
    public void SetUp()
    {
        _inputReader = Substitute.For<InputReader>();
        _userDataRequester = new TextUserDataRequester(_inputReader);
    }

    [Test]
    public void request_user_data()
    {
        InputByConsole(
            "username",
            "fullname",
            "password",
            "confirmationPassword"
        );

        var userData = _userDataRequester.Request();

        Assert.That(userData, Is.EqualTo(
                new UserData(
                    "username",
                    "fullname",
                    "password",
                    "confirmationPassword"
                )
            )
        );
    }

    private void InputByConsole(string username, string fullName, string password, string confirmPassword)
    {
        _inputReader.Read("Enter a username").Returns(username);
        _inputReader.Read("Enter your full name").Returns(fullName);
        _inputReader.Read("Enter your password").Returns(password);
        _inputReader.Read("Re-enter your password").Returns(confirmPassword);
    }
}