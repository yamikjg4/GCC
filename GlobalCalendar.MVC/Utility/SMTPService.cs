using GlobalCalendar.MVC.DTO;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GCC.Utility.Utility
{
    public static class SMTPService
    {
        private static readonly object _lock = new object();
        private static EmailSettings _emailSettings;

        // Static constructor to initialize _emailSettings
        static SMTPService()
        {
            lock (_lock)
            {
                if (_emailSettings == null)
                {
                    _emailSettings = new EmailSettings
                    {
                        Smtp = new SmtpSettings
                        {
                            Host = "10.102.5.101",
                            Port = 25,
                            UserName = "Suzlon\\appalert",
                            Password = "India123"
                        },
                        From = "application_alert@suzlon.com"
                    };
                }
            }
        }

        // Configure method to set the configuration settings
        public static void Configure(IOptions<EmailSettings> emailSettings)
        {
            lock (_lock)
            {
                if (_emailSettings == null)
                {
                    _emailSettings = emailSettings.Value;
                }
            }
        }

        public static async Task SendEmail(string to, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.Smtp.Host, _emailSettings.Smtp.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.From, _emailSettings.Smtp.Password);
                smtpClient.EnableSsl = true;

                // Bypass SSL certificate validation (use with caution)
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_emailSettings.From);
                    mailMessage.To.Add(to);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        Console.WriteLine("Email sent successfully!");
                    }
                    catch (SmtpException ex)
                    {
                        Console.WriteLine($"SMTP Exception: {ex.StatusCode} - {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }
            }
        }
        public static async Task SendEmailCC(string to, string cc, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.Smtp.Host, _emailSettings.Smtp.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.From, _emailSettings.Smtp.Password);
                smtpClient.EnableSsl = true;

                // Bypass SSL certificate validation (use with caution)
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_emailSettings.From);
                    mailMessage.To.Add(to);

                    // Add CC recipients if provided
                    if (!string.IsNullOrEmpty(cc))
                    {
                        mailMessage.CC.Add(cc);
                    }

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        Console.WriteLine("Email sent successfully!");
                    }
                    catch (SmtpException ex)
                    {
                        Console.WriteLine($"SMTP Exception: {ex.StatusCode} - {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }
            }
        }

        public static void SendEmailService(string to, string subject, string body)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.Smtp.Host, _emailSettings.Smtp.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.From, _emailSettings.Smtp.Password);
                smtpClient.EnableSsl = true;

                // Bypass SSL certificate validation (use with caution)
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_emailSettings.From);
                    mailMessage.To.Add(to);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        smtpClient.SendMailAsync(mailMessage);
                        Console.WriteLine("Email sent successfully!");
                    }
                    catch (SmtpException ex)
                    {
                        Console.WriteLine($"SMTP Exception: {ex.StatusCode} - {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                }
            }
        }


    }
}
