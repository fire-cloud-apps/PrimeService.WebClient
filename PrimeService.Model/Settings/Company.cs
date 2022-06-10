using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FC.Common.Domain.Location;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrimeService.Model.Common;

namespace PrimeService.Model.Settings;

/// <summary>
/// Account is also called as Organization
/// </summary>
public class Company
{
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Business Name
    /// </summary>
    [Required]
    [StringLength(150, ErrorMessage = "Name length can't be more than 150.")]
    public string Title { get; set; } //Alternative of 'Title'

    /// <summary>
    /// Company Primary Mobile number
    /// </summary>
    [Required]
    public string PrimaryMobile { get; set; }

    /// <summary>
    /// Company Secondary Mobile number
    /// </summary>
    public string SecondaryMobile { get; set; }

    /// <summary>
    /// Company Address
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Business Type that the company engaged.
    /// </summary>
    [Required]
    public string BusinessType { get; set; }

    /// <summary>
    /// Company default country
    /// </summary>
    public Country Country { get; set; }

    /// <summary>
    /// Company Default currency
    /// </summary>
    public string Currency { get; set; }
    
    /// <summary>
    /// Company Registration Number/Company Identification Number
    /// '21' Digit Alpha Numeric value eg. 'U 67190 TN 2014 PTC 096 978'
    /// </summary>
    public string CIN { get; set; }

}