using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PrimeService.Model.Common;

    /// <summary>
    /// Account is also called as Organization
    /// </summary>
    public class Account
    {
        /// <summary>
        /// A Unique Id to get account details.
        /// </summary>
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = string.Empty;

        [Required]
        [StringLength(150, ErrorMessage = "Name length can't be more than 150.")]
        public string BusinessName { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// The exact full domain for the application it can be custom domain url or our own domain from netlify.
        /// </summary>
        [Required]
        public string? ServiceDomain { get; set; }

        /// <summary>
        /// Full database name eg. "AVS-DB"
        /// </summary>
        [Required]
        public string? ClientDbName { get; set; }

        /// <summary>
        /// Full connection string value with the formated one
        /// eg. mongodb+srv://fc_client_admin:fc.clients.mongo@cluster0.acxm4.mongodb.net/{0}?retryWrites=true&w=majority&connect=replicaSet
        /// </summary>
        [Required]
        public string? ClientConnectionString { get; set; }

        #region Can be made in future

        #endregion
        
        /// <summary>
        /// Subscription Plan
        /// </summary>
        public SubscriptionPlan Subscription { get; set; }

        

    }
    
    /// <summary>
    /// Customer Subscribed Service
    /// </summary>
    public class SubscriptionPlan
    {
        public string PlanName { get; set; } = "Pay-as-you-use";
        public IList<SubscribedService> Services { get; set; }
    }

    public class SubscribedService
    {
        public string ServiceName { get; set; } = string.Empty;
        /// <summary>
        /// Zero indicates unlimited.
        /// </summary>
        public long QuantityLimit { get; set; } = long.MaxValue;
        public double CostPerQuantity { get; set; } = 0.0d;
        public string CostSuffix { get; set; } = "data";

    }

    public class BusinessType
    {
        public string ID { get; set; }
        public string Text { get; set; }
    }