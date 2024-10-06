namespace api_notification.Models
{
    public class SmtpOptions
    {
        public SmtpOptions()
        {
            UseDefaultCredential = false;
            IsProduction = false;
            IsActive = false;
        }

        /// <summary>
        /// Default false
        /// </summary>
        /// <value></value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Default false
        /// </summary>
        /// <value></value>
        public bool IsProduction { get; set; }

        public string Host { get; set; }

        public string EmailSender { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int? Port { get; set; }

        public bool? EnableSsl { get; set; }

        /// <summary>
        /// Default false
        /// </summary>
        /// <value></value>
        public bool UseDefaultCredential { get; set; }
    }
}
