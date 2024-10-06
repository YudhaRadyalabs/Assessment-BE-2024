namespace api_notification.Models
{
    public class MessageEmail
    {
        public MessageEmail()
        {
            CCs = new List<string>();
        }
        public string? From { get; set; }

        public string? To { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        /// <summary>
        /// Only used when MessageEnum equal to E-Mail
        /// </summary>
        /// <value></value>
        public MailAttachmentLookUpDto[]? Attachments { get; set; }
        public List<string> CCs { get; set; }
    }

    public class MailAttachmentLookUpDto
    {
        public string? Filename { get; set; }

        public string? FileExtension { get; set; }

        public long FileContentLength { get; set; }

        public byte[]? File { get; set; }

        public string? FileContentType { get; set; }
    }
}
