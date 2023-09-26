using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace EmailService.Configurations
{
    public class SmtpOptions
    {
        [Required] public string Host { get; set; }
        [EmailAddress] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        public int Port { get; set; }
        public bool useSsl { get; set; }



        //public bool enableLogging { get; set; } 

    }
}
