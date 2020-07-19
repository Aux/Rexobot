using Newtonsoft.Json;
using System;

namespace Rexobot.Gumroad
{
    public class GumroadSale
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("product_name")]
        public string ProductName { get; set; }
        [JsonProperty("price")]
        public double Price { get; set; }
        [JsonProperty("subscription_duration")]
        public string SubscriptionDuration { get; set; }
        [JsonProperty("formatted_display_price")]
        public string FormattedDisplayPrice { get; set; }
        [JsonProperty("formatted_total_price")]
        public string FormattedTotalPrice { get; set; }
        [JsonProperty("product_id")]
        public string ProductId { get; set; }
        [JsonProperty("purchase_email")]
        public string PurchaseEmail { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        [JsonProperty("paid")]
        public bool IsPaid { get; set; }
        //[JsonProperty("variants")]
        //public string Variants { get; set; }
        //[JsonProperty("variants_and_quantity")]
        //public string VariantsAndQuantity { get; set; }
        //[JsonProperty("has_custom_fields")]
        //public bool HasCustomFields { get; set; }
        //[JsonProperty("custom_fields")]
        //public object CustomFields { get; set; }
        [JsonProperty("order_id")]
        public ulong OrderId { get; set; }
        //[JsonProperty("is_product_physical")]
        //public bool IsProductPhysical { get; set; }
        [JsonProperty("purchaser_id")]
        public ulong PurchaserId { get; set; }
        //[JsonProperty("is_recurring_billing")]
        //public bool IsRecurringBilling { get; set; }
        //[JsonProperty("is_following")]
        //public bool IsFollowing { get; set; }
        //[JsonProperty("subscription_id")]
        //public string SubscriptionId { get; set; }
        [JsonProperty("cancelled")]
        public bool IsCancelled { get; set; }
        [JsonProperty("ended")]
        public bool IsEnded { get; set; }
        [JsonProperty("referrer")]
        public string Referrer { get; set; }
    }
}
