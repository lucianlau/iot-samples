using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Azure.IoTHub.Examples.CSharp.Core;
using Microsoft.Azure.Devices.Common;

namespace SendCloudToDeviceMessages
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        private static ServiceClient _serviceClient;

        /// <summary>
        /// Sends a cloud to device message (async).
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private static async Task SendCloudToDeviceMessageAsync(string deviceId, string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message)) {Ack = DeliveryAcknowledgement.Full};
            await _serviceClient.SendAsync(deviceId, commandMessage);
        }

        /// <summary>
        /// Receives the message ACK from the device (async).
        /// </summary>
        /// <param name="token">The token.</param>
        private static async Task ReceiveFeedbackAsync(CancellationToken token)
        {
            var feedbackReceiver = _serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nWaiting for c2d ACK from service");
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Listenter Cancelled, Exiting ...");
                    break;
                }

                var result = await feedbackReceiver.ReceiveAsync(TimeSpan.FromSeconds(1));
                if (result == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received ACK: {0}", string.Join(", ", result.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(result);
            }
        }

        private static async Task MessageSendTaskAsync(string deviceId, CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Listenter Cancelled, Exiting ...");
                    break;
                }

                Console.WriteLine("Type a message and press <ENTER> to send a C2D message.");
                // niave impl - press ctrl-c twice or close window to exit
                var message = Console.ReadLine();

                if (!message.IsNullOrWhiteSpace())
                    await SendCloudToDeviceMessageAsync(deviceId, message);
            }
        }

        static void Main(string[] args)
        {
            var config = @"../../../config/config.yaml".GetIoTConfiguration();

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            Console.WriteLine("Send Cloud-to-Device message\n");
            _serviceClient = ServiceClient.CreateFromConnectionString(config.AzureIoTHubConfig.ConnectionString);


            var ackTask = ReceiveFeedbackAsync(cts.Token);
            var sendTask = MessageSendTaskAsync(config.DeviceConfigs.First().DeviceId, cts.Token);

            Task.WaitAll(ackTask, sendTask);
            
        }
    }
}
