using System;
using System.Diagnostics;
using System.IO;
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
        private static async Task SendDeviceToCloudMessagesAsync(DeviceClient deviceClient, string deviceId, CancellationToken ct)
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

                await Task.Delay(10000);
            }
        }


        /// <summary>
        /// Receives the cloud to device message (async).
        /// </summary>
        /// <param name="deviceClient">The device client.</param>
        /// <param name="iotStorageContainerName">Name of the iot storage container.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        private static async Task ReceiveC2DAsync(DeviceClient deviceClient, string iotStorageContainerName, CancellationToken ct)
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                if (ct.IsCancellationRequested) break;

                var receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                var messageText = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                switch (messageText.ToLowerInvariant())
                {
                    case "upload bulk data":
                        const string bulkDataUploadFilePath = @"MOCK_DATA.csv";
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Uploading MOCK_DATA");
                        Console.ResetColor();

                        var sw = Stopwatch.StartNew();

                        using (var mockData = new FileStream(bulkDataUploadFilePath, FileMode.Open))
                        {
                            await deviceClient.UploadToBlobAsync(iotStorageContainerName, mockData);
                        }

                        sw.Stop();

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("File uploaded in {0}ms", sw.ElapsedMilliseconds);
                        Console.ResetColor();
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Received message: {0}", messageText);
                        Console.ResetColor();
                        break;
                }

                // this will remove the message from the queue.  
                await deviceClient.CompleteAsync(receivedMessage);
            }
        }

        public static void Main(string[] args)
        {
            var config = @"../../../config/config.yaml".GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            Console.WriteLine("Simulated device\n");
            var deviceClient = DeviceClient.Create(
                    azureConfig.IoTHubUri, 
                    new DeviceAuthenticationWithRegistrySymmetricKey(testDevice.DeviceId, testDevice.Key));

            var cts = new CancellationTokenSource();

            // register cancellation request event.
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting ...");
            };

            // kick off cloud data send.
            var sendTask = SendDeviceToCloudMessagesAsync(deviceClient, testDevice.DeviceId, cts.Token);

            // kick off could data receive
            var receiveTask = ReceiveC2DAsync(deviceClient, azureConfig.IoTHubStorageContainer, cts.Token);

            Task.WaitAny(sendTask, receiveTask);

            Console.ReadLine();
        }
    }
}
