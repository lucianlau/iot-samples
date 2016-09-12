using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
namespace Azure.IoTHub.Examples.CSharp.Core
{
    public class Configuration
    {
        public AzureIoTHubConfig AzureIoTHubConfig { get; set; }
        public List<DeviceConfig> DeviceConfigs { get; set; }
    }
    public class AzureIoTHubConfig
    {
        public string IoTHubUri { get; set; }
        public string IotHubD2cEndpoint { get; set; }
        public string ConnectionString { get; set; }
    }

    public class DeviceConfig
    {
        public string DeviceNickName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceKey { get; set; }
    }
}
