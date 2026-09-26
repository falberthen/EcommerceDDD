namespace EcommerceDDD.IdentityServer.Services;

public class SmtpEmailSender(IOptions<EmailSettings> emailSettings) : IEmailSender<ApplicationUser>
{
	private readonly EmailSettings _settings = emailSettings?.Value
		?? throw new ArgumentNullException(nameof(emailSettings));

	public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
		SendAsync(email, "Confirm your e-mail",
			$"""
			<p>Welcome to EcommerceDDD!</p>
			<p>Please <a href="{confirmationLink}">confirm your e-mail</a> to activate your account.</p>
			<p>If you did not create this account, you can ignore this message.</p>
			""");

	public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
		SendAsync(email, "Reset your password",
			$"""
			<p>A password reset was requested for your EcommerceDDD account.</p>
			<p><a href="{resetLink}">Choose a new password</a>.</p>
			<p>If you did not request it, you can ignore this message.</p>
			""");

	public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
		throw new NotSupportedException("Password resets are sent as links.");

	private async Task SendAsync(string to, string subject, string htmlBody)
	{
		using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort);
		using var message = new MailMessage(_settings.From, to, subject, htmlBody) { IsBodyHtml = true };
		await client.SendMailAsync(message);
	}
}
