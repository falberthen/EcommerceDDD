// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace EcommerceDDD.ServiceClients.ApiGateway.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class QuoteViewModel : IParsable
    #pragma warning restore CS1591
    {
        /// <summary>The createdAt property</summary>
        public DateTimeOffset? CreatedAt { get; set; }
        /// <summary>The currencyCode property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? CurrencyCode { get; set; }
#nullable restore
#else
        public string CurrencyCode { get; set; }
#endif
        /// <summary>The currencySymbol property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? CurrencySymbol { get; set; }
#nullable restore
#else
        public string CurrencySymbol { get; set; }
#endif
        /// <summary>The customerId property</summary>
        public Guid? CustomerId { get; set; }
        /// <summary>The items property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteItemViewModel>? Items { get; set; }
#nullable restore
#else
        public List<global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteItemViewModel> Items { get; set; }
#endif
        /// <summary>The quoteId property</summary>
        public Guid? QuoteId { get; set; }
        /// <summary>The quoteStatus property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? QuoteStatus { get; set; }
#nullable restore
#else
        public string QuoteStatus { get; set; }
#endif
        /// <summary>The totalPrice property</summary>
        public double? TotalPrice { get; private set; }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteViewModel"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteViewModel CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteViewModel();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "createdAt", n => { CreatedAt = n.GetDateTimeOffsetValue(); } },
                { "currencyCode", n => { CurrencyCode = n.GetStringValue(); } },
                { "currencySymbol", n => { CurrencySymbol = n.GetStringValue(); } },
                { "customerId", n => { CustomerId = n.GetGuidValue(); } },
                { "items", n => { Items = n.GetCollectionOfObjectValues<global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteItemViewModel>(global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteItemViewModel.CreateFromDiscriminatorValue)?.AsList(); } },
                { "quoteId", n => { QuoteId = n.GetGuidValue(); } },
                { "quoteStatus", n => { QuoteStatus = n.GetStringValue(); } },
                { "totalPrice", n => { TotalPrice = n.GetDoubleValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteDateTimeOffsetValue("createdAt", CreatedAt);
            writer.WriteStringValue("currencyCode", CurrencyCode);
            writer.WriteStringValue("currencySymbol", CurrencySymbol);
            writer.WriteGuidValue("customerId", CustomerId);
            writer.WriteCollectionOfObjectValues<global::EcommerceDDD.ServiceClients.ApiGateway.Models.QuoteItemViewModel>("items", Items);
            writer.WriteGuidValue("quoteId", QuoteId);
            writer.WriteStringValue("quoteStatus", QuoteStatus);
        }
    }
}
#pragma warning restore CS0618
