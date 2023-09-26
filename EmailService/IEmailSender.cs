namespace EmailService
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(
           string fromName,
           string fromEmail,
           string toName,
           string toEmail,
           string subject,
           string body,
           CancellationToken cancellationToken
           );

    }
}