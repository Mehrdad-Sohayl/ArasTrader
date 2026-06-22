namespace ArasTrader.Infrastructure.ExternalApis.ArasApi;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresInSeconds { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenRequest
{
    public string Token { get; set; }
}

public class CustomerResponse
{
    public string NationalCode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FatherName { get; set; }
    public string BirthCertificationNumber { get; set; }
    public string RegistrationNumber { get; set; }
    public string BirthDate { get; set; }
    public string BranchName { get; set; }
    public string MobileNumber { get; set; }
}
