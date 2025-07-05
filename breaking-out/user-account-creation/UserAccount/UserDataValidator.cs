using System;
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
            _dniRegex = new Regex(@"^(([KLM]\d{7})|(\d{8}))([A-Z])$");
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
                return new CifValidator(digits);
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
            private const string ControlLettersCif = "JABCDEFGHI";

            private static readonly Regex CifRegex =
                new Regex(@"^[ABCDEFGHJNPQRSUVW]\d{7}[A-J0-9]$", RegexOptions.IgnoreCase);


            public CifValidator(string digits)
            {
                _digits = digits;
            }
            
            public bool IsValid()
            {
                return IsValidCif(_digits);
            }
            
            private bool IsValidCif(string cif)
            {
                if (!CifRegex.IsMatch(cif))
                    return false;

                var numbers = cif.Substring(1, 7); // Extraer los 7 números
                var controlCharacter = cif[^1]; // Último carácter

                var sumPairs = SumEvenPositions(numbers);
                var sumOdds = SumOddPositions(numbers);

                var totalSum = sumPairs + sumOdds;
                var controlNumber = CalculateControlNumber(totalSum);

                if (char.IsDigit(controlCharacter))
                    return controlNumber.ToString() == controlCharacter.ToString();

                if (char.IsLetter(controlCharacter))
                    return ControlLettersCif[controlNumber].ToString() == controlCharacter.ToString();

                return false;
            }

            private int SumEvenPositions(string numbers)
            {
                return int.Parse(numbers[1].ToString()) +
                       int.Parse(numbers[3].ToString()) +
                       int.Parse(numbers[5].ToString());
            }

            private int SumOddPositions(string numbers)
            {
                return SumDigits(int.Parse(numbers[0].ToString()) * 2) +
                       SumDigits(int.Parse(numbers[2].ToString()) * 2) +
                       SumDigits(int.Parse(numbers[4].ToString()) * 2) +
                       SumDigits(int.Parse(numbers[6].ToString()) * 2);
            }

            private int SumDigits(int number)
            {
                if (number < 10) return number;

                var numStr = number.ToString();
                return int.Parse(numStr[0].ToString()) + int.Parse(numStr[1].ToString());
            }

            private int CalculateControlNumber(int totalSum)
            {
                var remainder = totalSum % 10;
                return remainder == 0 ? 0 : 10 - remainder;
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

            private const string DniControlLetters = "TRWAGMYFPDXBNJZSQVHLCKE";

            private static readonly Regex DniRegex =
                new Regex(@"^([KLM]\d{7}|\d{8})[TRWAGMYFPDXBNJZSQVHLCKE]$", RegexOptions.IgnoreCase);

            public DniValidator(string digits)
            {
                _digits = digits;
            }

            public bool IsValid()
            {
                return DniRegex.IsMatch(_digits) && IsValidDniLetter();
            }

            private bool IsValidDniLetter()
            {
                var numberPart = Regex.Replace(_digits, "[^\\d]", "");
                var number = _digits.StartsWith("K", StringComparison.OrdinalIgnoreCase) ||
                             _digits.StartsWith("L", StringComparison.OrdinalIgnoreCase) ||
                             _digits.StartsWith("M", StringComparison.OrdinalIgnoreCase)
                    ? int.Parse("0" + numberPart)
                    : int.Parse(numberPart);

                var letterIndex = number % 23;
                var letter = _digits[^1];
                return DniControlLetters[letterIndex] == letter;
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
            if (string.IsNullOrWhiteSpace(creditCardNumber) || creditCardNumber.Any(c => !char.IsDigit(c)))
            {
                return false;
            }

            return creditCardNumber
                .Reverse()
                .Select(c => c - '0')
                .Select((digit, index) => index % 2 == 0 
                    ? digit 
                    : (digit * 2 > 9 ? digit * 2 - 9 : digit * 2))
                .Sum() % 10 == 0;
        }
    }
}