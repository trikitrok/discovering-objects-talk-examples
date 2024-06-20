using System.Globalization;
using EmptyFiles;
using NSubstitute;
using NUnit.Framework;

namespace UnusualSpendings.Tests
{
    public class UnusualSpendingsServiceTest
    {
        private UnusualSpendingsService _unusualSpendingsService;
        private UnusualSpendingsDetector _unusualSpendingsDetector;
        private UserRepository _userRepository;
        private AlertSender _alertsSender;
        private User _user = new User(new UserId("userId"));
        private UserContactData _userContactData = new UserContactData("user@user.com");

        [SetUp]
        public void Setup()
        {
            _unusualSpendingsDetector = Substitute.For<UnusualSpendingsDetector>();
            _userRepository = Substitute.For<UserRepository>();
            _alertsSender = Substitute.For<AlertSender>();
            _unusualSpendingsService = new UnusualSpendingsService(_unusualSpendingsDetector, _userRepository, _alertsSender);
        }

        [Test]
        public void an_alert_message_is_sent_when_one_unusual_spendings_is_detected()
        {
            var spendingCategory = new SpendingCategory("travel", new Money(928.00m, "$"));
            var alertText = ComposeAlertText(spendingCategory);
            _unusualSpendingsDetector.Detect(_user).Returns(
                new UnsusualSpendings(new List<SpendingCategory> { spendingCategory })
            );
            _userRepository.GetContactData(_user).Returns(_userContactData);

            _unusualSpendingsService.Alert(_user);

            _alertsSender.Received(1).Send(new Alert(alertText, _userContactData));
        }

        [Test]
        public void no_alert_message_is_sent_when_no_unusual_spendings_are_detected()
        {
            _unusualSpendingsDetector.Detect(_user).Returns(new UnsusualSpendings(new List<SpendingCategory>()));

            _unusualSpendingsService.Alert(_user);

            _alertsSender.Received(0).Send(Arg.Any<Alert>());
        }
        
        [Test]
        public void an_alert_message_is_sent_when_several_unusual_spendings_are_detected()
        {
            var spendingCategory1 = new SpendingCategory("travel", new Money(928.00m, "$"));
            var spendingCategory2 = new SpendingCategory("groceries", new Money(800.33m, "$"));
            var alertText = ComposeAlertText(spendingCategory1, spendingCategory2);
            _unusualSpendingsDetector.Detect(_user).Returns(
                new UnsusualSpendings(new List<SpendingCategory> { spendingCategory1, spendingCategory2 })
            );
            _userRepository.GetContactData(_user).Returns(_userContactData);

            _unusualSpendingsService.Alert(_user);

            _alertsSender.Received(1).Send(new Alert(alertText, _userContactData));
        }

        private string ComposeAlertText(params SpendingCategory[] spendingCategories)
        {
            var alertText = Introduction();
            alertText = spendingCategories.Aggregate(alertText,
                (current, category) => current + CategoryLine(category));
            alertText += Footter();
            return alertText;
        }

        private static string Footter()
        {
            return "\nLove,\n\nThe Credit Card Company\n";
        }

        private static string Introduction()
        {
            return "Hello card user!\n\n" +
                   "We have detected unusually high spending on your card in these categories:\n\n";
        }

        private string CategoryLine(SpendingCategory category)
        {
            return FormatSpentMoney(category) + $"on {category.Name()}\n";
        }

        private static string FormatSpentMoney(SpendingCategory category)
        {
            return $"* You spent {category.CurrencySymbol()}{FormatAmount(category.TotalAmountspent())} ";
        }

        private static string FormatAmount(decimal amount)
        {
            return amount.ToString("f2", new CultureInfo("en-US"));
        }
    }
}