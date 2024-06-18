using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UserAccount;

public class UserDataValidator
{
    private readonly IdValidator _idValidator;

    public UserDataValidator()
    {
        _idValidator = new IdValidator();
    }

    public bool IsNotValid(UserData userData)
    {
        return !(LuhnTestValidator.Passes(userData.CreditCardNumber()) && _idValidator.IsValid(userData.SpanishId()));
    }
    
    private class IdValidator
    {
        private readonly Regex _dniRegex;
        private readonly Regex _cifRegex;
        private readonly Regex _nieRegex;

        public IdValidator()
        {
            _dniRegex = new Regex(@"^(([KLM]\\d{7})|(\\d{8}))([A-Z])$");
            _cifRegex = new Regex(@"^([ABCDEFGHJNPQRSUVW])(\d{7})([0-9A-J])$");
            _nieRegex = new Regex(@"^[XYZ]\d{7,8}[A-Z]$");
        }

        public bool IsValid(Id id)
        {
            var str = RemoveWhitespaces(id.Digits().ToUpper());
            var idType = GetIdType(str);
            return idType.IsValid();
        }

        private DocumentValidator GetIdType(string digits)
        {
            if (_dniRegex.IsMatch(digits))
            {
                return new DniValidator(digits);
            }

            if (_cifRegex.IsMatch(digits))
            {
                return new CifValidator(digits, _cifRegex);
            }

            if (_nieRegex.IsMatch(digits))
            {
                return new NieValidator(digits);
            }

            return new NotMatchingValidator();
        }

        private static string RemoveWhitespaces(string str)
        {
            return Regex.Replace(str, @"\s", string.Empty);
        }

        private class NotMatchingValidator : DocumentValidator
        {
            public bool IsValid()
            {
                return false;
            }
        }

        private class CifValidator : DocumentValidator
        {
            private readonly string _digits;
            private readonly Regex _cifRegex;
            private readonly Regex _controlAsDigitRegex;
            private readonly Regex _controlAsLetterRegex;

            public CifValidator(string digits, Regex cifRegex)
            {
                _digits = digits;
                _cifRegex = cifRegex;
                _controlAsDigitRegex = new Regex(@"[ABEH]");
                _controlAsLetterRegex = new Regex(@"[PQSW]");
            }

            public bool IsValid()
            {
                var matches = _cifRegex.Matches(_digits);
                var letter = matches[1].Value;
                var number = matches[2].Value;
                var control = matches[3].Value;
                var lastDigit = ComputeLastDigit(number);
                var controlDigit = lastDigit != 0 ? (10 - lastDigit) : lastDigit;
                var controlLetter = "JABCDEFGHI".Substring(controlDigit, 1);

                if (_controlAsDigitRegex.IsMatch(letter))
                {
                    return controlDigit.ToString().Equals(control);
                }

                if (_controlAsLetterRegex.IsMatch(letter))
                {
                    return controlLetter.Equals(control);
                }

                return controlDigit.ToString().Equals(control) || controlLetter.Equals(control);
            }

            private static int ComputeLastDigit(string number)
            {
                var evenSum = 0;
                var oddSum = 0;

                for (var i = 0; i < number.Length; i++)
                {
                    var n = int.Parse(number.Substring(i, 1));

                    if (i % 2 == 0)
                    {
                        n *= 2;
                        oddSum += n < 10 ? n : n - 9;
                    }
                    else
                    {
                        evenSum += n;
                    }
                }

                return int.Parse((evenSum + oddSum).ToString()[^1..]);
            }
        }

        private class NieValidator : DocumentValidator
        {
            private readonly string _digits;

            public NieValidator(string digits)
            {
                _digits = digits;
            }

            public bool IsValid()
            {
                return new DniValidator(ComposeDniDigits()).IsValid();
            }

            private string ComposeDniDigits()
            {
                return GetCorrespondingDniPrefix() + _digits.Substring(1);
            }

            private string GetCorrespondingDniPrefix()
            {
                var niePrefix = _digits.Substring(0, 1);

                switch (niePrefix)
                {
                    case "X":
                        return "0";
                    case "Y":
                        return "1";
                    case "Z":
                        return "2";
                    default:
                        return niePrefix;
                }
            }
        }

        private class DniValidator : DocumentValidator
        {
            private readonly string _digits;
            private readonly Regex _dniFirstDigitRegex;

            public DniValidator(string digits)
            {
                _digits = digits;
                _dniFirstDigitRegex = new Regex(@"^(([KLM]\\d{7})|(\\d{8}))([A-Z])$");
            }

            public bool IsValid()
            {
                var dniLetters = "TRWAGMYFPDXBNJZSQVHLCKE";
                var dniDigits = _digits;
                if (_dniFirstDigitRegex.IsMatch(dniDigits[..1]))
                {
                    dniDigits = dniDigits[1..];
                }
                var number = int.Parse(dniDigits);
                var letter = dniLetters.Substring(number % 23, 1);
                return letter == dniDigits[^1..];
            }
        }

        private interface DocumentValidator
        {
            bool IsValid();
        }
    }

    private class LuhnTestValidator
    {
        public static bool Passes(string creditCardNumber)
        {
            var reversedNumbers = Reverse(creditCardNumber).Select(number => int.Parse(number.ToString()));
            return (LunhSumOddPositions(reversedNumbers) + LunhSumEvenPositions(reversedNumbers)) % 10 == 0;
        }

        private static int LunhSumOddPositions(IEnumerable<int> numbers)
        {
            return numbers.ToList().Where((c, i) => i % 2 != 0).Sum();
        }

        private static int LunhSumEvenPositions(IEnumerable<int> numbers)
        {
            return numbers.ToList().Where((c, i) => i % 2 == 0)
                .Select((number) => number * 2)
                .Select((number) => number.ToString().Split("").Select(int.Parse).Sum())
                .Sum();
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}

