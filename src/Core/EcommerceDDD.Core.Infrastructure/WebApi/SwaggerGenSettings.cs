namespace EcommerceDDD.Core.Infrastructure.WebApi;

public record class SwaggerGenSettings
{
    public string Version { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}