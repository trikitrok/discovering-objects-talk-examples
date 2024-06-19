using System.Globalization;
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
            var categoryTotalAmountSpent = 928.00m;
            var categoryName = "travel";
            var currencySymbol = "$";
            var alertText =
                $"Hello card user!\n\nWe have detected unusually high spending on your card in these categories:\n\n* You spent {currencySymbol}{categoryTotalAmountSpent.ToString("f2", new CultureInfo("en-US"))} on {categoryName}\n\nLove,\n\nThe Credit Card Company\n";
            var user = new User(new UserId("userId"));
            var userContactData = new UserContactData("user@user.com");
            _unusualSpendingsDetector.Detect(user).Returns(new List<UnsusualSpending>
            {
                new UnsusualSpending(
                    new List<SpendingCategory>
                    {
                        new SpendingCategory(categoryName, new Money(categoryTotalAmountSpent, currencySymbol))
                    }
                )
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
    }
}