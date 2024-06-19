using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UnusualSpendings;

public class UnusualSpendingsService
{
    private readonly UserRepository _userRepository;
    private readonly AlertSender _alertsSender;
    private readonly UnusualSpendingsDetector _unusualSpendingsDetector;

    public UnusualSpendingsService(UnusualSpendingsDetector unusualSpendingsDetector, UserRepository userRepository,
        AlertSender alertsSender)
    {
        _unusualSpendingsDetector = unusualSpendingsDetector;
        _userRepository = userRepository;
        _alertsSender = alertsSender;
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
        var alertText = ComposeAlertText(unsusualSpendings, contactData, out var userContactData);
        var alert = new Alert(alertText, userContactData);
        return alert;
    }

    private string ComposeAlertText(List<UnsusualSpending> unsusualSpendings, UserContactData contactData,
        out UserContactData userContactData)
    {
        var alertText = Greeting();
        var unsusualSpending = unsusualSpendings.First();
        userContactData = contactData;
        var spendingCategories = unsusualSpending.SpendingCategories();
        var spendingCategory = spendingCategories.First();
        alertText += "We have detected unusually high spending on your card in these categories:\n\n" +
                     $"* You spent {spendingCategory.CurrencySymbol()}" +
                     $"{spendingCategory.TotalAmountspent().ToString("f2", new CultureInfo("en-US"))} " +
                     $"on {spendingCategory.Name()}\n";
        alertText += Footer();
        return alertText;
    }

    private string Footer()
    {
        return "\nLove,\n\nThe Credit Card Company\n";
    }

    private string Greeting()
    {
        return $"Hello card user!\n\n";
    }
    
    private bool IsEmpty(List<UnsusualSpending> unsusualSpendings)
    {
        return unsusualSpendings.Count == 0;
    }
}