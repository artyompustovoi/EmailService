using EmailService.Configurations;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
namespace EmailService
{
    public class Program
    {
        public static void Main(string[] args)
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
            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();

           

            //await using (var emailSender = new MailKitSmtpEmailSender())
            //{
            //    await emailSender.SendEmailAsync();
            //}



            app.MapGet("/get_current_utc_time", (
            [FromServices] IEmailSender sender) =>
            
            sender.SendEmailAsync("pau", "p@d.ru", "pustovojartem32", "pustovojartem32@gmail.com", "sbjct", "msg", new CancellationToken())
            );

            
        
        app.Run();
        }
}
}