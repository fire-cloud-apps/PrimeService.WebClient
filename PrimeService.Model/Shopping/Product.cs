﻿using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace PrimeService.Model.Shopping;

/// <summary>
/// Products sold by the company
/// </summary>
public class Product
{
    #region Basic Product Details
    /// <summary>
    /// A Unique Id to get account details.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } = string.Empty;

    /// <summary>
    /// Product Name
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Product Name length can't be more than 150.")]
    public string? Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Product category
    /// </summary>
    [Required]
    public ProductCategory? Category { get; set; }
    
    /// <summary>
    /// A Bar code value of the product. Stock Keeping Unit/BarCode
    /// </summary>
    public string? Barcode { get; set; } = string.Empty;
    
    #endregion

    #region Quantity or Inventory Control
    /// <summary>
    /// Current Quantity in hand.
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Used to alert that we have reached the Minimal stock
    /// </summary>
    public int MinQuantity { get; set; }
    /// <summary>
    /// Used to alert that we have reached the Maximum stock
    /// </summary>
    public int MaxQuantity { get; set; }
    #endregion

    #region Price
    /// <summary>
    /// Selling price is also called as 'Retail Price', This price will be used to sell to the customer. 'Selling' price should be always greater then the 'Supplier' price.
    /// </summary>
    public double SellingPrice { get; set; }
    /// <summary>
    /// Supplier Price which is the one, to which the product has been 'Purchased' by the organization. 'Supplier' price should always be lesser then the 'Selling' Price.
    /// </summary>
    public double SupplierPrice { get; set; }
    
    /// <summary>
    /// Is there any additional cost involved with this product, we can use it. This should reflect in the selling price.
    /// </summary>
    public double Cost { get; set; }

    #endregion

    #region Other Product Info

    /// <summary>
    /// Product Warranty which the 'Supplier' or the 'Vendor' provided. This is usually in 'Days'
    /// </summary>
    public short Warranty
    {
        get;
        set;
    }

    /// <summary>
    /// Some General Description of the product.
    /// </summary>
    public string? Notes { get; set; } = string.Empty;

    #endregion
}