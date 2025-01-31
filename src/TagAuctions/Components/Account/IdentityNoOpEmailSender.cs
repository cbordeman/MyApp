using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Diagnostics;
using TagAuctions.Data;

namespace TagAuctions.Components.Account;

// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
internal sealed class IdentityAzureEmailSender : IEmailSender<ApplicationUser>
{
    //private const string SupportEmailAddress = "cbordeman@outlook.com";

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        await SendEmailAsync(email, "Confirm your email", "", "");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password","", "");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password", "", "");

    private async Task SendEmailAsync(string recipient,
        string subject, string htmlBody, string plainTextBody)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(htmlBody));
        Debug.Assert(!string.IsNullOrWhiteSpace(plainTextBody));

        var connectionString = "censored";
        var emailClient = new EmailClient(connectionString);

        var emailMessage = new EmailMessage(
            senderAddress: "censored",
            content: new EmailContent(subject)
            {
                PlainText = $"MyApp\n\n{plainTextBody}",
                Html = @$"
		            <html>
			            <body style=""font-family: 'Courier New'"">
				            <h1>MyApp</h1>
                            <p>{htmlBody}</p>
			            </body>
		            </html>"
            },
            recipients: new EmailRecipients(new List<EmailAddress> { new(recipient) }));

        var emailSendOperation = await emailClient.SendAsync(WaitUntil.Completed, emailMessage);

        if (emailSendOperation.Value.Status == EmailSendStatus.Succeeded)
            Log.Information($"Email Sent: {recipient}");
        else
        {
            Log.Warning($"Failed to send email (Status = {emailSendOperation.Value.Status}).  Recipient: {recipient}");
            throw new Exception(@$"Failed to send email to recipient ""{recipient}.""");
        }
    }
}