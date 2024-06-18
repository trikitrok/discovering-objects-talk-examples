using NSubstitute;
using NUnit.Framework;

namespace Security.Tests.Acceptance;

public class SimulatedSecurityManagerTest
{
    private const string Username = "Pepe";
    private const string FullName = "Pepe Garcia";
    private InputReader _inputReader;
    private Notifier _notifier;
    private SimulatedSecurityManager _simulatedSecurityManager;

    [SetUp]
    public void Setup()
    {
        _notifier = Substitute.For<Notifier>();
        _inputReader = Substitute.For<InputReader>();
        _simulatedSecurityManager = new SimulatedSecurityManager(_notifier, new TextUserDataRequester(_inputReader));
    }

    [Test]
    public void do_not_save_user_when_password_and_confirm_password_are_not_equals()
    {
        IntroducingUserDataWithPasswords(password: "Pepe1234", confirmationPassword: "Pepe1234.");

        _simulatedSecurityManager.CreateValidUser();

        Notified("The passwords don't match");
    }

    [Test]
    public void do_not_save_user_when_password_too_short()
    {
        IntroducingUserDataWithPasswords(password: "Pepe123", confirmationPassword: "Pepe123");

        _simulatedSecurityManager.CreateValidUser();

        Notified("Password must be at least 8 characters in length");
    }

    [Test]
    public void save_user()
    {
        IntroducingUserDataWithPasswords(password: "Pepe1234", confirmationPassword: "Pepe1234");

        _simulatedSecurityManager.CreateValidUser();

        var encryptedPassword = "4321epeP";
        Notified($"Saving Details for User ({Username}, {FullName}, {encryptedPassword})\n");
    }

    private void IntroducingUserDataWithPasswords(string password, string confirmationPassword)
    {
        _inputReader.Read("Enter a username").Returns(Username);
        _inputReader.Read("Enter your full name").Returns(FullName);
        _inputReader.Read("Enter your password").Returns(password);
        _inputReader.Read("Re-enter your password").Returns(confirmationPassword);
    }

    private void Notified(string message)
    {
        _notifier.Received(1).Notify(message);
        _notifier.Received(1).Notify(Arg.Any<string>());
    }
}