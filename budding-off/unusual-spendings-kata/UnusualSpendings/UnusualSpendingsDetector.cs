using System.Collections.Generic;

namespace UnusualSpendings;

public interface UnusualSpendingsDetector
{
    UnsusualSpendings Detect(User user);
}