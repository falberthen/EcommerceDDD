namespace EcommerceDDD.Quotes.Application.Products;

public record class GetProductsRequest(string CurrencyCode, Guid[] ProductIds);