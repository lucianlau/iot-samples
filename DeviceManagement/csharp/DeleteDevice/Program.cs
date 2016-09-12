using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Azure.IoTHub.Examples.CSharp.Core;

namespace Azure.IotHub.Examples.Csharp.DeleteDevice
{
    class Program
    {
        static RegistryManager registryManager;

        private static async Task<Device> AddDeviceAsync(string deviceId)
        {
            Device device;
            try
            {
                
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
                Console.WriteLine($"Registered (Id:PrimaryKey): {device.Id}:{device.Authentication.SymmetricKey.PrimaryKey}.");
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
                Console.WriteLine($"Device (Id:PrimaryKey): {device.Id}:{device.Authentication.SymmetricKey.PrimaryKey} already registered.");
            }
            return device;
        }

        public static async Task GetDevices()
        {
            Console.WriteLine("Getting Configured Device List ...");
            var devices = await registryManager.GetDevicesAsync(1000); 

            Console.WriteLine("Configured Device List: (ID:PrimaryKey)");
            devices.ToList().ForEach(x => Console.WriteLine($"\t {x.Id} : {x.Authentication.SymmetricKey.PrimaryKey}"));
        }

        public static async Task RemoveDevice(Device device)
        {
            await registryManager.RemoveDeviceAsync(device);
        }



        static void Main(string[] args)
        {
            var config = @"../../../config.yaml".GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            registryManager = RegistryManager.CreateFromConnectionString(azureConfig.ConnectionString);

            // check that the device is configured, add if not
            var t1 = AddDeviceAsync(testDevice.DeviceId);
            t1.Wait();
            var device = t1.Result;

            //display list of devices.
            GetDevices().Wait();

            // Remove a device.
            Console.WriteLine($"Removing (Id:PrimaryKey): {testDevice.DeviceId}:{testDevice.DeviceKey}.");
            RemoveDevice(device).Wait();
            Console.WriteLine($"Removed (Id:PrimaryKey): {testDevice.DeviceId}:{testDevice.DeviceKey}.");

            // show device removed.
            GetDevices().Wait();

            // add device back to not interrupt 
            Console.WriteLine("Adding device back into registry.");
            t1 = AddDeviceAsync(testDevice.DeviceId);
            t1.Wait();

            Console.ReadLine();
        }
    }
}
