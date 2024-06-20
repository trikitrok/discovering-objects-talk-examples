using System.Globalization;
using System.Linq;

namespace UnusualSpendings;

public class UnusualSpendingsService
{
    private readonly AlertSender _alertsSender;
    private readonly AlertTextComposer _alertTextComposer;
    private readonly UnusualSpendingsDetector _unusualSpendingsDetector;
    private readonly UserRepository _userRepository;

    public UnusualSpendingsService(UnusualSpendingsDetector unusualSpendingsDetector, UserRepository userRepository,
        AlertSender alertsSender)
    {
        _unusualSpendingsDetector = unusualSpendingsDetector;
        _userRepository = userRepository;
        _alertsSender = alertsSender;
        _alertTextComposer = new AlertTextComposer();
    }

    public void Alert(User user)
    {
        var unusualSpendings = _unusualSpendingsDetector.Detect(user);
        if (unusualSpendings.IsEmpty()) return;

        var alert = CreateAlert(unusualSpendings, _userRepository.GetContactData(user));
        _alertsSender.Send(alert);
    }

    private Alert CreateAlert(UnsusualSpendings unusualSpendings, UserContactData contactData)
    {
        return new Alert(ComposeText(unusualSpendings), contactData);
    }

    private string ComposeText(UnsusualSpendings unusualSpendings)
    {
        return _alertTextComposer.Compose(unusualSpendings);
    }

    private class AlertTextComposer
    {
        private readonly CultureInfo _cultureInfo;

        public AlertTextComposer()
        {
            _cultureInfo = new CultureInfo("en-US");
        }

        public string Compose(UnsusualSpendings unusualSpendings)
        {
            return Introduction() + CategoryLines(unusualSpendings) + Footer();
        }

        private string CategoryLines(UnsusualSpendings unusualSpendings)
        {
            return unusualSpendings.SpendingCategories().Aggregate("",
                (current, spendingCategory) => current + CategoryLine(spendingCategory));
        }

        private static string Introduction()
        {
            return "Hello card user!\n\n" +
                   "We have detected unusually high spending on your card in these categories:\n\n";
        }

        private string Footer()
        {
            return "\nLove,\n\nThe Credit Card Company\n";
        }

        private string CategoryLine(SpendingCategory category)
        {
            return FormatSpentMoney(category) + $"on {category.Name()}\n";
        }

        private string FormatSpentMoney(SpendingCategory category)
        {
            return $"* You spent {category.CurrencySymbol()}{FormatAmount(category.TotalAmountspent())} ";
        }

        private string FormatAmount(decimal amount)
        {
            return amount.ToString("f2", _cultureInfo);
        }
    }
}