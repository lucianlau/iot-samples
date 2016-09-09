namespace Azure.IoTHub.Examples.CSharp.Core
{
    public class Configuration
    {
        public AzureIoTHubConfig AzureIoTHubConfig { get; set; }
    }
    public class AzureIoTHubConfig
    {
        public string DeviceKey { get; set; }
        public string IoTHubUri { get; set; }
        public string DeviceId { get; set; }
        public string IotHubD2cEndpoint { get; set; }
        public string ConnectionString { get; set; }
    }
}
