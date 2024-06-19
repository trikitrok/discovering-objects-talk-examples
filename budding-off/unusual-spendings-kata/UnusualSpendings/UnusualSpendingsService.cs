using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UnusualSpendings;

public class UnusualSpendingsService
{
    private readonly UserRepository _userRepository;
    private readonly AlertSender _alertsSender;
    private readonly UnusualSpendingsDetector _unusualSpendingsDetector;
    
    public UnusualSpendingsService(UnusualSpendingsDetector unusualSpendingsDetector, UserRepository userRepository, AlertSender alertsSender)
    {
        _unusualSpendingsDetector = unusualSpendingsDetector;
        _userRepository = userRepository;
        _alertsSender = alertsSender;
    }

    public void Alert(User user)
    {
        var unsusualSpendings = _unusualSpendingsDetector.Detect(user);
        if (unsusualSpendings.Count == 0)
        {
            return;
        }

        var unsusualSpending = unsusualSpendings.First();
        var userContactData = _userRepository.GetContactData(user);
        var spendingCategories = unsusualSpending.SpendingCategories();
        var spendingCategory = spendingCategories.First();
        var alertText =
            $"Hello card user!\n\n"+
            "We have detected unusually high spending on your card in these categories:\n\n"+
            $"* You spent {spendingCategory.CurrencySymbol()}"+
            $"{spendingCategory.TotalAmountspent().ToString("f2", new CultureInfo("en-US"))} "+
            $"on {spendingCategory.Name()}\n\nLove,\n\nThe Credit Card Company\n";
        _alertsSender.Send(new Alert(alertText, userContactData));
    }
}