namespace EcommerceDDD.IdentityServer.Configurations;

public class EmailSettings
{
	public const string SectionName = "EmailSettings";

	public string SmtpHost { get; set; } = default!;
	public int SmtpPort { get; set; }
	public string From { get; set; } = default!;
	
	/// <summary>Where the links in the e-mails point to the SPA pages that complete each flow.</summary>
	public string SpaBaseUrl { get; set; } = default!;
}
