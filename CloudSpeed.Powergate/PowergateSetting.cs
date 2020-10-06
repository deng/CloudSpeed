namespace CloudSpeed.Powergate
{
    public class PowergateSetting
    {
        public string ServerAddress { get; set; }

        public string RootCertificates { get; set; }

        public string ClientCert { get; set; }

        public string ClientKey { get; set; }

        public string BotToken { get; set; }

        public bool Enabled { get; set; }
    }
}
