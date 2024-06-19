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

        [SetUp]
        public void Setup()
        {
            _unusualSpendingsDetector = Substitute.For<UnusualSpendingsDetector>();
            _userRepository = Substitute.For<UserRepository>();
            _alertsSender = Substitute.For<AlertSender>();
            _unusualSpendingsService =
                new UnusualSpendingsService(_unusualSpendingsDetector, _userRepository, _alertsSender);
        }

        [Test]
        public void an_alert_message_is_sent_when_unusual_spendings_are_detected()
        {
            var spendingCategory = new SpendingCategory("travel", new Money(928.00m, "$"));
            var alertText =
                ComposeAlertText(spendingCategory);
            var user = new User(new UserId("userId"));
            var userContactData = new UserContactData("user@user.com");
            _unusualSpendingsDetector.Detect(user).Returns(new List<UnsusualSpending>
            {
                new UnsusualSpending(new List<SpendingCategory> { spendingCategory })
            });
            _userRepository.GetContactData(user).Returns(userContactData);

            _unusualSpendingsService.Alert(user);

            _alertsSender.Received(1).Send(new Alert(alertText, userContactData));
        }

        [Test]
        public void no_alert_message_is_sent_when_no_unusual_spendings_are_detected()
        {
            var user = new User(new UserId("userId"));
            _unusualSpendingsDetector.Detect(user).Returns(new List<UnsusualSpending>());

            _unusualSpendingsService.Alert(user);

            _alertsSender.Received(0).Send(Arg.Any<Alert>());
        }

        private string ComposeAlertText(params SpendingCategory[] spendingCategories)
        {
            var alertText = Introduction();
            
            foreach (var category in spendingCategories)
            {
                alertText += CategoryLine(category);    
            }
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