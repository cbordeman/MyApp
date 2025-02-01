using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Diagnostics;
using TagAuc.Data;

namespace TagAuc.Components.Account;

// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
internal sealed class IdentityAzureEmailSender : IEmailSender<ApplicationUser>
{
    //private const string SupportEmailAddress = "cbordeman@outlook.com";

    public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        await SendEmailAsync(email, "Confirm your email",
            $"<p>Welcome, {user.FirstName} {user.LastName}!</p><p>Please confirm your email by <a href='{confirmationLink}'>clicking here</a>.</p>",
            $"Hello, {user.FirstName} {user.LastName}.\n\n\tPlease confirm your email by following this link: {confirmationLink}");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset your password",
            $"<p>Hello, {user.FirstName} {user.LastName}.</p><p>Please reset your password by <a href='{resetLink}'>clicking here</a></p>.",
            $"Hello, {user.FirstName} {user.LastName}.\n\n\tPlease reset your password by following this link: {resetLink}");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Reset your password",
            $"<p>Hello, {user.FirstName} {user.LastName}.</p><p>Please reset your password using the following code: {resetCode}</p>",
            $"Hello, {user.FirstName} {user.LastName}.\n\n\tPlease reset your password using the following code: {resetCode}");

    private async Task SendEmailAsync(string recipient,
        string subject, string htmlBody, string plainTextBody)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(htmlBody));
        Debug.Assert(!string.IsNullOrWhiteSpace(plainTextBody));

        var connectionString = "endpoint=https://tag-comsvc.unitedstates.communication.azure.com/;accesskey=5y1qqtBsBX5HTZAmBBFzuxlW0mLgyewh1QFxsMmS6k5bH4WfRLhjJQQJ99BAACULyCplImYuAAAAAZCS3pxe";
        var emailClient = new EmailClient(connectionString);

        var emailMessage = new EmailMessage(
            senderAddress: "DoNotReply@acc122b5-89ff-4b23-ad38-f36338a2ec51.azurecomm.net",
            content: new EmailContent(subject)
            {
                PlainText = $"Thomas Auto Group Auctions\n\n{plainTextBody}",
                Html = @$"
		            <html>
			            <body style=""font-family: 'Courier New'"">
				            <h1>Thomas Auto Group Auctions</h1>
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