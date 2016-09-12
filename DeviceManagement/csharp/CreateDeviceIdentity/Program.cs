using System;
using System.Linq;
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
            var configFilePath = @"../../../config.yaml";
            var config = configFilePath.GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            registryManager = RegistryManager.CreateFromConnectionString(azureConfig.ConnectionString);
            var task = AddDeviceAsync(testDevice.DeviceId);
            task.Wait();

            testDevice.DeviceKey = task.Result;

            if (configFilePath.UpdateIoTConfiguration(config).Item1)
            {
                Console.WriteLine($"DeviceId: {testDevice.DeviceId} has DeviceKey: {testDevice.DeviceKey}. Config file: {configFilePath} has been updated accordingly.");
            } else
            {
                Console.WriteLine($"Error writing DeviceKey: {testDevice.DeviceKey} for DeviceId: {testDevice.DeviceId} to config file: {configFilePath} ");
            }

            Console.ReadLine();
        }
    }
}
