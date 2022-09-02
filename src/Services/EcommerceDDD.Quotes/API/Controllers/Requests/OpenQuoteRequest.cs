namespace EcommerceDDD.Quotes.API.Controllers.Requests;

public record OpenQuoteRequest(Guid CustomerId, string Currency);