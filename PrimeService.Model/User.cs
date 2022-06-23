using System.ComponentModel.DataAnnotations;

namespace PrimeService.Model;

/// <summary>
/// User is our 'Employee' of the organization
/// </summary>
public class User
{
    /// <summary>
    /// Unique Id of the user
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the user, Primarily used in External Authentication
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// A Unique username for the login user/employee.
    /// </summary>
    //[Required(ErrorMessage = "UserName is required.")]
    [StringLength(50, ErrorMessage = "UserName length can't be more than 50.")]
    public string? Username { get; set; } = string.Empty;

    /// <summary>
    /// Password for the Employee/User
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, ErrorMessage = "Password length can't be more than 50.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Activated Domain URL/Account URL
    /// </summary>
    public string DomainURL { get; set; } = string.Empty;

    /// <summary>
    /// User Type its a client user/FC User
    /// </summary>
    public string UserType { get; set; } = string.Empty;
    
    /// <summary>
    /// User is Active or not Active..
    /// </summary>
    public bool IsActive { get; set; }

    #region For Authentication

    /// <summary>
    /// This is not used to store in DB, but activated during user login.
    /// </summary>
    public string JwtToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token generated to get the new JWT Token
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Used to validate the request received is success/failed
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// An Error message or success message received by this field.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Used to get the 'connection string' to know from which account we receive this API call.
    /// </summary>
    public string ConnectionKey { get; set; } = string.Empty;

    #endregion

    /// <summary>
    /// User Mapped with the specific account
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    /// Employee Email id
    /// </summary>
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Employee Picture.
    /// </summary>
    public string Picture { get; set; } = string.Empty;


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