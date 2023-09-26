using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using System.Threading;
using Microsoft.Extensions.Options;
using EmailService.Configurations;

namespace EmailService
{
    public class MailKitSmtpEmailSender : IEmailSender
    {

        readonly MailKit.Net.Smtp.SmtpClient smtpClient = new();
        readonly SmtpOptions smtpConfig;

        public MailKitSmtpEmailSender(IOptionsSnapshot<SmtpOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);    
            smtpConfig = options.Value;

        }


        private async Task EnsureConnectedAndAuthenticated(CancellationToken cancellationToken)
        {
            if (!smtpClient.IsConnected)
                await smtpClient.ConnectAsync(smtpConfig.Host, smtpConfig.Port, smtpConfig.useSsl, cancellationToken);
            if (!smtpClient.IsAuthenticated)
                await smtpClient.AuthenticateAsync(smtpConfig.UserName, smtpConfig.Password, cancellationToken);
            //try
            //{
            //    if (!smtpClient.IsAuthenticated)
            //        await smtpClient.AuthenticateAsync("pau", "sB3vF9dL6bmY7yP0", cancellationToken);
            //}
            //catch (Exception e)
            //{
            //   
            //}
        }
        public async Task SendEmailAsync(
            string fromName,
            string fromEmail,
            string toName,
            string toEmail,
            string subject,
            string body,
            CancellationToken cancellationToken
            )
        {
            await EnsureConnectedAndAuthenticated(cancellationToken);
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(fromName, fromEmail));
            mimeMessage.To.Add(new MailboxAddress(toName, toEmail));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = body
            };
            try
            {
                var response = await smtpClient.SendAsync(mimeMessage, cancellationToken);
                Console.WriteLine(response);    
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
               

        }

        public async ValueTask DisposeAsync()
        {
            await smtpClient.DisconnectAsync(true);
            smtpClient.Dispose();

        }
    }
}