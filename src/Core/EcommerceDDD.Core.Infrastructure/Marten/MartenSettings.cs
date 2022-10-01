namespace EcommerceDDD.Core.Infrastructure.Marten;

public record MartenSettings
{
    public string ConnectionString { get; set; }
    public string WriteSchema { get; set; }
    public string ReadSchema { get; set; }
}
