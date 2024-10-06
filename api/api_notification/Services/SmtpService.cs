using api_notification.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace api_notification.Services
{
    public class SmtpService
    {
        private readonly Stopwatch _timer;
        private readonly SmtpOptions _option;
        private readonly ILogger<SmtpService> _logger;

        public SmtpService(IOptions<SmtpOptions> options, ILogger<SmtpService> logger)
        {
            _option = options.Value;
            _logger = logger;

            if (_option == null)
            {
                throw new ArgumentNullException(paramName: nameof(options), message: "IOptions smtp can not be null or undeclared");
            }

            if (string.IsNullOrWhiteSpace(_option.Host))
            {
                throw new ArgumentException(message: "Host can not be null or empty", paramName: nameof(_option.Host));
            }

            if (string.IsNullOrWhiteSpace(_option.Username))
            {
                throw new ArgumentException(message: "Username can not be null or empty", paramName: nameof(_option.Username));
            }

            if (_option.Port == null)
            {
                throw new ArgumentException(message: "Port can not be null", paramName: nameof(_option.Port));
            }

            if (_option.EnableSsl == null)
            {
                throw new ArgumentException(message: "EnableSsl can not be null", paramName: nameof(_option.EnableSsl));
            }

            if (_option.UseDefaultCredential)
            {
                if (string.IsNullOrWhiteSpace(_option.Password))
                {
                    throw new ArgumentException(message: "Password can not be null or empty", paramName: nameof(_option.Username));
                }
            }

            _timer = new Stopwatch();
        }

        public async Task SendEmailAsync(MessageEmail MessageEmail)
        {
            _timer.Start();

            await LogicSendEmailAsync(MessageEmail);

            _timer.Stop();

            _logger.LogInformation("Send email to {To}, ({ElapsedMilliseconds} milliseconds)", _timer.ElapsedMilliseconds, MessageEmail.To);
        }

        private async Task LogicSendEmailAsync(MessageEmail MessageEmail)
        {
            try
            {
                //check parameter is null
                if (MessageEmail is null)
                {
                    throw new ArgumentNullException(nameof(MessageEmail));
                }

                //check parameter To is null
                if (string.IsNullOrWhiteSpace(MessageEmail.To))
                {
                    throw new ArgumentNullException(nameof(MessageEmail.To));
                }

                //check parameter Subject is null
                if (string.IsNullOrWhiteSpace(MessageEmail.Subject))
                {
                    throw new ArgumentNullException(nameof(MessageEmail.Subject));
                }

                //check parameter Body is null
                if (string.IsNullOrWhiteSpace(MessageEmail.Body))
                {
                    throw new ArgumentNullException(nameof(MessageEmail.Body));
                }
            }
            catch
            {
                _logger.LogWarning($"Failed to send email because validation model failed");
                throw;
            }

            try
            {
                // check if smtp option inactive, default success
                if (_option.IsActive == false)
                {
                    _logger.LogInformation($"Send email success to {MessageEmail.To}");
                    return;
                }

                using (MailMessage mailMessage = new MailMessage())
                {
                    if (string.IsNullOrWhiteSpace(MessageEmail.From))
                    {
                        mailMessage.From = new MailAddress(_option.EmailSender);
                    }
                    else
                    {
                        mailMessage.From = new MailAddress(_option.EmailSender, MessageEmail.From);
                    }

                    mailMessage.Subject = MessageEmail.Subject;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = MessageEmail.Body;
                    mailMessage.To.Add(MessageEmail.To);

                    //delivery error notification
                    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.None;

                    if (MessageEmail.Attachments != null && MessageEmail.Attachments.Length > 0)
                    {
                        foreach (MailAttachmentLookUpDto file in MessageEmail.Attachments)
                        {
                            // Create  the file attachment for this e-mail message.
                            Attachment data = new Attachment(new MemoryStream(file.File), file.Filename);

                            // Add time stamp information for the file.
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = File.GetCreationTime(file.Filename);
                            disposition.ModificationDate = File.GetLastWriteTime(file.Filename);
                            disposition.ReadDate = File.GetLastAccessTime(file.Filename);

                            // Add the file attachment to this e-mail message.
                            mailMessage.Attachments.Add(data);
                        }
                    }

                    foreach (var cc in MessageEmail.CCs)
                    {
                        mailMessage.CC.Add(cc);
                    }

                    using (SmtpClient smtp = new SmtpClient(_option.Host))
                    {
                        if (_option.UseDefaultCredential)
                        {
                            smtp.UseDefaultCredentials = true;
                        }
                        else
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(_option.Username, _option.Password);
                        }

                        smtp.EnableSsl = _option.EnableSsl.Value;

                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Port = _option.Port.Value;
                        _logger.LogInformation($"Try Send Email");
                        _logger.LogInformation($"EmailSender :{_option.EmailSender}");
                        _logger.LogInformation($"Username :{_option.Username}");
                        _logger.LogInformation($"Password :{_option.Password}");
                        _logger.LogInformation($"Port :{_option.Port}");
                        _logger.LogInformation($"EnableSsl :{_option.EnableSsl}");
                        _logger.LogInformation($"UseDefaultCredential :{_option.UseDefaultCredential}");

                        await smtp.SendMailAsync(mailMessage);
                        _logger.LogInformation($"Send email success to {MessageEmail.To}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email :{ex.ToString()}");
            }
        }
    }
}
