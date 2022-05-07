namespace PrimeService.Model;

public class User
{
    /// <summary>
    /// Unique Id of the user
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the user, Primarily used in External Authentication
    /// </summary>
    public string? Name { get; set; } = string.Empty;
    /// <summary>
    /// A Unique username for the login user/employee.
    /// </summary>
    public string? Username { get; set; }
    public string Password { get; set; }
    public string DomainURL { get; set; }
    public string UserType { get; set; }
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    
    public string AccountId { get; set; }
    public string Email { get; set; }

    public string Picture { get; set; }
    public string ConnectionKey { get; set; }

    #region Currently Not in Use
    // public string LastName { get; set; }
    // public string FirstName { get; set; }
    // public string Gender { get; set; }
    // public string Status { get; set; }

    #endregion
}

public enum UserCategory
{
    ClientUser,
    FcUser 
}