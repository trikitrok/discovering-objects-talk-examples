using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UnusualSpendings;

public class UnusualSpendingsService
{
    private readonly UserRepository _userRepository;
    private readonly AlertSender _alertsSender;
    private readonly UnusualSpendingsDetector _unusualSpendingsDetector;
    private readonly AlertTextComposer _alertTextComposer;


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
        var unsusualSpendings = _unusualSpendingsDetector.Detect(user);
        if (IsEmpty(unsusualSpendings))
        {
            return;
        }

        var alert = CreateAlert(unsusualSpendings, _userRepository.GetContactData(user));
        _alertsSender.Send(alert);
    }

    private Alert CreateAlert(List<UnsusualSpending> unsusualSpendings, UserContactData contactData)
    {
        var alertText = _alertTextComposer.ComposeAlertText(unsusualSpendings, contactData, out var userContactData);
        var alert = new Alert(alertText, userContactData);
        return alert;
    }

    private bool IsEmpty(List<UnsusualSpending> unsusualSpendings)
    {
        return unsusualSpendings.Count == 0;
    }

    private class AlertTextComposer
    {
        private CultureInfo _cultureInfo;

        public AlertTextComposer()
        {
            _cultureInfo = new CultureInfo("en-US");
        }

        public string ComposeAlertText(List<UnsusualSpending> unsusualSpendings, UserContactData contactData,
            out UserContactData userContactData)
        {
            var alertText = Introduction();
            var unsusualSpending = unsusualSpendings.First();
            userContactData = contactData;
            var spendingCategories = unsusualSpending.SpendingCategories();
            var spendingCategory = spendingCategories.First();
            alertText += CategoryLine(spendingCategory);
            alertText += Footer();
            return alertText;
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

        private string Greeting()
        {
            return $"Hello card user!\n\n";
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