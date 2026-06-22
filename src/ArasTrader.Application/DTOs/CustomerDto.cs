namespace ArasTrader.Application.DTOs;

public class CustomerDto
{
    public string NationalCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FatherName { get; set; }
    public string BirthCertificationNumber { get; set; }
    public string RegisterationNumber { get; set; }
    public DateOnly BirthDate { get; set; }
    public string BranchName { get; set; }
    public string MobileNumber { get; set; }
}
