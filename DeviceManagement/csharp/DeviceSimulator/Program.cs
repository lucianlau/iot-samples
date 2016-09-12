using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

using Azure.IoTHub.Examples.CSharp.Core;

namespace DeviceSimulator
{
    class Program
    {
        static DeviceClient deviceClient;

        private static async void SendDeviceToCloudMessagesAsync(string deviceId)
        {
            double avgWindSpeed = 10; // m/s
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Task.Delay(1000).Wait();
            }
        }

        private static async void ReceiveC2dAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        static void Main(string[] args)
        {
            var config = @"../../../config.yaml".GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            Console.WriteLine("Simulated device\n");
            deviceClient = DeviceClient.Create(azureConfig.IoTHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(testDevice.DeviceId, testDevice.DeviceKey));

            SendDeviceToCloudMessagesAsync(testDevice.DeviceId);
            ReceiveC2dAsync();
            Console.ReadLine();
        }
    }
}
