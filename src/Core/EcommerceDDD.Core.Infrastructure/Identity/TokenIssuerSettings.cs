namespace EcommerceDDD.Core.Infrastructure;

public record class TokenIssuerSettings
{
    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
}