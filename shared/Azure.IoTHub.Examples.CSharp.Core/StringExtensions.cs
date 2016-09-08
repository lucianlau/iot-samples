using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Azure.IoTHub.Examples.CSharp.Core
{
    public static class StringExtensions
    {
        public static Configuration GetIoTConfiguration(this string configFilePath)
        {
            var deserializer = new Deserializer();
            using (var reader = File.OpenText(configFilePath))
            {
                return deserializer.Deserialize<Configuration>(reader);
            }
        }

        public static Tuple<bool, Exception> UpdateIoTConfiguration(this Configuration config, string configFilePath)
        {
            try
            {
                var serializer = new Serializer();
                using (var writer = File.CreateText(configFilePath))
                {
                    serializer.Serialize(writer, config);
                }
                return new Tuple<bool, Exception>(true, null);

            } catch (Exception e)
            {
                return new Tuple<bool, Exception>(false, e);
            }
        }
    }
}
