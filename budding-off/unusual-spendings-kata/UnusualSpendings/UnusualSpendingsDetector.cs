using System.Collections.Generic;

namespace UnusualSpendings;

public interface UnusualSpendingsDetector
{
    List<UnsusualSpending> Detect(User user);
}