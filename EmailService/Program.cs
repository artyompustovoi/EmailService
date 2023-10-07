using EmailService.Configurations;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;
namespace EmailService
{

    public class Program
    {

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateBootstrapLogger();
            Log.Information("Стартуем!");

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                //builder.Services.AddOptions<SmtpConfig>()
                //    .BindConfiguration("SmtpConfig")
                //    .ValidateDataAnnotations()
                //    .ValidateOnStart();

                //builder.Services.AddRazorPages();

                builder.Services.Configure<SmtpOptions>(
                    builder.Configuration.GetSection("SmtpConfig"));


                builder.Services.AddScoped<IEmailSender, MailKitSmtpEmailSender>();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Host.UseSerilog((_, conf) => conf.WriteTo.Console());

                var app = builder.Build();
                app.UseStaticFiles();
                app.UseSerilogRequestLogging();
                app.UseSwagger();
                app.UseSwaggerUI();
                int attempts = 0;


                //await using (var emailSender = new MailKitSmtpEmailSender())
                //{
                //    await emailSender.SendEmailAsync();
                //}


                Log.Information("Сервер запущен!");
                try { 
                    app.MapGet("/email_server", (
                    [FromServices] IEmailSender sender) =>
                    sender.SendEmailAsync("pau", "pau@delisdci.ru", "pustovojartem32", "pustovojartem32@gmail.com", "sbjct", "msg", new CancellationToken())

                );

                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Ошибка отправки!");
                    if (attempts < 2)
                    {
                        Log.Information("Повторная отправка сообщения");
                        attempts++;
                        app.MapGet("/email_server", (
                        [FromServices] IEmailSender sender) =>
                        sender.SendEmailAsync("pau", "pau@delisdci.ru", "pustovojartem32", "pustovojartem32@gmail.com", "sbjct", "msg", new CancellationToken()));
                    }
                    else
                        Log.Error(ex.Message);
                }
                app.Run();
            }

            catch (Exception ex)
            {
                Log.Fatal(ex, "Сервер рухнул!");
            }
            finally
            {
                Log.Information("Shut down complete");
                await Log.CloseAndFlushAsync();
            }

        }
    }
}
