using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Azure.IoTHub.Examples.CSharp.Core;


namespace Azure.IoTHub.Examples.CSharp.CreateDeviceIdentity
{
    /// <summary>
    /// Sample program to create a device identity in Azure IoT Hub.
    /// </summary>
    public class Program
    {
        private static RegistryManager _registryManager;

        private static async Task<string> AddDeviceAsync(string deviceId)
        {
            Device device;
            try
            {
                device = await _registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceId);
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public static void Main(string[] args)
        {
            const string configFilePath = @"../../../config/config.yaml";
            var config = configFilePath.GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            _registryManager = RegistryManager.CreateFromConnectionString(azureConfig.ConnectionString);
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
