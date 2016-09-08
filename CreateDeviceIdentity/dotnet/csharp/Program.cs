using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Azure.IoTHub.Examples.CSharp.Core;


namespace Azure.IoTHub.Examples.CSharp.CreateDeviceIdentity
{
    class Program
    {

        static RegistryManager registryManager;

        private static async Task<string> AddDeviceAsync(string deviceId)
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        static void Main(string[] args)
        {
            var configFilePath = @"config.yaml";
            var config = configFilePath.GetIoTConfiguration();
            var azureIoTHubConfig = config.AzureIoTHubConfig;

            registryManager = RegistryManager.CreateFromConnectionString(azureIoTHubConfig.ConnectionString);
            var task = AddDeviceAsync(azureIoTHubConfig.DeviceId);
            task.Wait();

            azureIoTHubConfig.DeviceKey = task.Result;

            if (config.UpdateIoTConfiguration(configFilePath).Item1)
            {
                Console.WriteLine($"DeviceId: {azureIoTHubConfig.DeviceId} has DeviceKey: {azureIoTHubConfig.DeviceKey}. Config file: {configFilePath} has been updated accordingly.");
            } else
            {
                Console.WriteLine($"Error writing DeviceKey: {azureIoTHubConfig.DeviceKey} for DeviceId: {azureIoTHubConfig.DeviceId} to config file: {configFilePath} ");
            }

            Console.ReadLine();
        }
    }
}
