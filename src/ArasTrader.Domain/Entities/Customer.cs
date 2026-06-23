using ArasTrader.Domain.Common;
using ArasTrader.Domain.Exceptions;

namespace ArasTrader.Domain.Entities;

public class Customer
{
    public int Id { get; private set; }
    public string NationalCode { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string FatherName { get; private set; }
    public string BirthCertificationNumber { get; private set; }
    public string RegistrationNumber { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string BranchName { get; private set; }
    public string MobileNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Wallet Wallet { get; private set; }

    private Customer()
    {

    }

    internal Customer(
        string nationalCode,
        string firstName,
        string lastName,
        string fatherName,
        string birthCertificationNumber,
        string registrationNumber,
        DateOnly birthDate,
        string branchName,
        string mobileNumber)
    {
        NationalCode = nationalCode;
        FirstName = firstName;
        LastName = lastName;
        FatherName = fatherName;
        BirthCertificationNumber = birthCertificationNumber;
        RegistrationNumber = registrationNumber;
        BirthDate = birthDate;
        BranchName = branchName;
        MobileNumber = mobileNumber;
        CreatedAt = DateTime.UtcNow;
    }

    public static Customer Create(
        string nationalCode,
        string firstName,
        string lastName,
        string fatherName,
        string birthCertificationNumber,
        string registrationNumber,
        DateOnly birthDate,
        string branchName,
        string mobileNumber)
    {
        if (
            string.IsNullOrWhiteSpace(nationalCode) ||
            nationalCode.Length != 10 ||
            !nationalCode.All(char.IsDigit) ||
            string.IsNullOrWhiteSpace(firstName) ||
            string.IsNullOrWhiteSpace(lastName) ||
            string.IsNullOrWhiteSpace(fatherName) ||
            string.IsNullOrWhiteSpace(birthCertificationNumber) ||
            string.IsNullOrWhiteSpace(registrationNumber) ||
            birthDate > DateOnly.FromDateTime(DateTime.UtcNow) ||
            string.IsNullOrWhiteSpace(branchName) ||
            string.IsNullOrWhiteSpace(mobileNumber) ||
            mobileNumber.Length != 11 ||
            !mobileNumber.All(char.IsDigit))
            throw new DomainException(new DomainError(DomainErrorCodes.CannotCreateCustomer, DomainErrorCodes.CannotCreateCustomer));

        return new Customer(
            nationalCode,
            firstName,
            lastName,
            fatherName,
            birthCertificationNumber,
            registrationNumber,
            birthDate,
            branchName,
            mobileNumber);
    }
}
