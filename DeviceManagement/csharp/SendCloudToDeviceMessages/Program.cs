using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Azure.IoTHub.Examples.CSharp.Core;

namespace SendCloudToDeviceMessages
{
    class Program
    {
        static ServiceClient serviceClient;

        private async static Task SendCloudToDeviceMessageAsync(string deviceId, string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(deviceId, commandMessage);
        }

        private async static void ReceiveFeedbackAsync(CancellationToken token)
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d ACK from service");
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Listenter Cancelled, Exiting.");
                }
                var feedbackBatchTask = feedbackReceiver.ReceiveAsync();
                feedbackBatchTask.Wait(token);

                var result = feedbackBatchTask.Result;
                if (result == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received ACK: {0}", string.Join(", ", result.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(result);
            }
        }

        static void Main(string[] args)
        {
            var config = @"config.yaml".GetIoTConfiguration();

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(config.AzureIoTHubConfig.ConnectionString);


            Task.Run(() => ReceiveFeedbackAsync(cts.Token));

            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine("Listenter Cancelled, Exiting.");
                }

                Console.WriteLine("Type a message and press <ENTER> to send a C2D message.");
                var message = Console.ReadLine();
                SendCloudToDeviceMessageAsync(config.AzureIoTHubConfig.DeviceId, message).Wait();
            }
        }
    }
}
