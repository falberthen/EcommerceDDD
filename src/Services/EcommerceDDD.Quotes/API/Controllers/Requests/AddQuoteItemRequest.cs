using EcommerceDDD.Quotes.Domain;

namespace EcommerceDDD.Quotes.API.Controllers.Requests;

public record class AddQuoteItemRequest(Guid ProductId, int Quantity, string CurrencyCode);