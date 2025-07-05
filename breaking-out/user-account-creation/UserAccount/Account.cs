using System;

namespace UserAccount;

public class Account
{
    private readonly Id _id;
    private readonly string _encryptedCreditCardNumber;
    private readonly string _fullName;

    public Account(string fullName, Id id, string encryptedCreditCardNumber)
    {
        _fullName = fullName;
        _id = id;
        _encryptedCreditCardNumber = encryptedCreditCardNumber;
    }

    protected bool Equals(Account other)
    {
        return Equals(_id, other._id) && _encryptedCreditCardNumber == other._encryptedCreditCardNumber && _fullName == other._fullName;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Account)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_id, _encryptedCreditCardNumber, _fullName);
    }

    public override string ToString()
    {
        return
            $"{nameof(_id)}: {_id}, {nameof(_encryptedCreditCardNumber)}: {_encryptedCreditCardNumber}, {nameof(_fullName)}: {_fullName}";
    }
}