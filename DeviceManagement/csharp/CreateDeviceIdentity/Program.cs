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

        /// <summary>
        /// Adds the device asynchronously.
        /// </summary>
        /// <param name="deviceConfig">The device configuration.</param>
        /// <returns></returns>
        private static async Task<string> AddDeviceAsync(DeviceConfig deviceConfig)
        {
            Device device;
            try
            {
                DeviceStatus status;
                if (!Enum.TryParse(deviceConfig.Status, true, out status))
                    status = DeviceStatus.Disabled;

                var d = new Device(deviceConfig.DeviceId)
                {
                    Status = status
                };
                device = await _registryManager.AddDeviceAsync(d);

                Console.WriteLine($"Device: {deviceConfig.DeviceId} created");
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceConfig.DeviceId);

                Console.WriteLine($"Device: {deviceConfig.DeviceId} already exist");

            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        public static void Main(string[] args)
        {
            const string configFilePath = @"../../../config/config.yaml";
            var config = configFilePath.GetIoTConfiguration();
            var testDevices = config.DeviceConfigs;
            var azureConfig = config.AzureIoTHubConfig;

            CommandLineParser.CommandLineParser parser = new CommandLineParser.CommandLineParser();
            var commandLineArguments = new CommandLineArguments();
            parser.ExtractArgumentAttributes(commandLineArguments);
            parser.ParseCommandLine(args);

            _registryManager = RegistryManager.CreateFromConnectionString(azureConfig.ConnectionString);

            if (commandLineArguments.NumberOfDeviceToCreate > 0)
            {
                var deviceNamePrefix = commandLineArguments.DeviceNamePrefix;
                testDevices = config.DeviceConfigs = new System.Collections.Generic.List<DeviceConfig>();
                for (int deviceNumber = 0; deviceNumber < commandLineArguments.NumberOfDeviceToCreate; deviceNumber++)
                {
                    var testDevice = new DeviceConfig()
                    {
                        DeviceId = $"{deviceNamePrefix}{deviceNumber:0000}",
                        Nickname = $"{deviceNamePrefix}{deviceNumber:0000}",
                        Status = "Enabled"
                    };
                    testDevices.Add(testDevice);

                    var task = AddDeviceAsync(testDevice);
                    task.Wait();

                    testDevice.Key = task.Result;
                }
            }
            else
            {
                foreach (var testDevice in testDevices)
                {
                    var task = AddDeviceAsync(testDevice);
                    task.Wait();

                    testDevice.Key = task.Result;
                }
            }

            if (configFilePath.UpdateIoTConfiguration(config).Item1)
            {
                foreach (var testDevice in testDevices)
                {
                    Console.WriteLine(
                        $"DeviceId: {testDevice.DeviceId} has DeviceKey: {testDevice.Key} \r\nConfig file: {configFilePath} has been updated accordingly.");
                }
            }


            Console.ReadLine();
        }
    }
}
