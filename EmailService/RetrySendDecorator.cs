using Polly.Retry;
using Polly;
using Microsoft.Extensions.Options;
using EmailService.Configurations;

namespace EmailService
{
    public class RetrySendDecorator : IEmailSender
    {

        private readonly IEmailSender _inner;
        private readonly ILogger<RetrySendDecorator> _logger;
        private readonly AsyncRetryPolicy _policy;
        private readonly SmtpOptions _options;
        public RetrySendDecorator(IEmailSender _inner, ILogger<RetrySendDecorator> _logger, IOptionsSnapshot<SmtpOptions> options)
        {
            ArgumentNullException.ThrowIfNull(_inner);
            ArgumentNullException.ThrowIfNull(_logger);
            ArgumentNullException.ThrowIfNull(options);

            this._inner = _inner;
            this._logger = _logger;
            _options = options.Value;


            _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_options.RetryLimit, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
           onRetry: (ex, retryAttempt) =>
           {
               _logger.LogWarning(
                 ex, "Error while sending email. Retrying: {Attempt}", retryAttempt);
           });
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
            PolicyResult? result = await _policy.ExecuteAndCaptureAsync(
            () => _inner.SendEmailAsync(fromName, fromEmail, toName, toEmail, subject, body, default));
            if (result.Outcome == OutcomeType.Failure)
            {
                _logger.LogError(result.FinalException, "There was an error while sending email");
                throw result.FinalException;

                //int attemptsLimit = 3;
                //for (int currentAttempt = 1; currentAttempt <= attemptsLimit; currentAttempt++)
                //{
                //    try
                //    {
                //        await _inner.SendEmailAsync(fromName, fromEmail, toName, toEmail, subject, body, cancellationToken);
                //        break;
                //    }
                //    catch (Microsoft.AnalysisServices.ConnectionException ex) when (currentAttempt < attemptsLimit)
                //    {
                //        _logger.LogWarning(ex, $"Error while sending email {toEmail}, {subject}", "Connection");
                //        await Task.Delay(TimeSpan.FromSeconds(30));
                //    }
                //    catch (Exception ex)
                //    {
                //        _logger.LogError(ex, $"Error ocured, the letter hasn't been sent: {toEmail}, {subject}", "Connection");
                //        throw;
                //    }
                //}
            }
        }

    }
}
