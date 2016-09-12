using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

using Azure.IoTHub.Examples.CSharp.Core;

namespace DeviceSimulator
{
    /// <summary>
    /// Sample program that simulates an IoT device for use with Azure IoT Hub.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Sends the device to cloud messages (async).
        /// </summary>
        /// <param name="deviceClient">The device client.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="ct">The cancellation token</param>
        private static async void SendDeviceToCloudMessagesAsync(DeviceClient deviceClient, string deviceId, CancellationToken ct)
        {
            const double avgWindSpeed = 10; // m/s
            var rand = new Random();

            while (true)
            {
                if (ct.IsCancellationRequested) break;

                var currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;

                var telemetryDataPoint = new
                {
                    deviceId = deviceId,
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Receives the cloud to device message (async).
        /// </summary>
        private static async void ReceiveC2DAsync(DeviceClient deviceClient, CancellationToken ct)
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                if (ct.IsCancellationRequested) break;

                var receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        public static void Main(string[] args)
        {
            var config = @"../../../config/config.yaml".GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            Console.WriteLine("Simulated device\n");
            var deviceClient = DeviceClient.Create(azureConfig.IoTHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(testDevice.DeviceId, testDevice.DeviceKey));

            var cts = new CancellationTokenSource();

            // register cancellation request event.
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting ...");
            };

            // kick off cloud data send.
            SendDeviceToCloudMessagesAsync(deviceClient, testDevice.DeviceId, cts.Token);

            // kick off could data receive
            ReceiveC2DAsync(deviceClient, cts.Token);

            Console.ReadLine();
        }
    }
}
