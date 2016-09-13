using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.IoTHub.Examples.CSharp.Core;
using Microsoft.Azure.Devices;

namespace Jobs
{
    /// <summary>
    /// Sample program that simulates an IoT device for use with Azure IoT Hub.
    /// </summary>
    public class Program5
    {
        /// <summary>
        /// Sends the device to cloud messages (async).
        /// </summary>
        /// <param name="registryManager">The registry manager.</param>
        /// <param name="ct">The cancellation token</param>
        /// <returns></returns>
        private static async Task GetCurrentJobListFromHubAsync(RegistryManager registryManager, CancellationToken ct)
        {
            var jobs = (await registryManager.GetJobsAsync(ct)).ToList();
            if (jobs.Count > 0)
                jobs.ToList().ForEach(job =>
                   Console.WriteLine("Current Hub Job: {0} status is: {1}", job.JobId, job.Status));
            else
                Console.WriteLine("Job list is empty");
        }

        public static void Main(string[] args)
        {
            var config = @"../../../config/config.yaml".GetIoTConfiguration();
            var testDevice = config.DeviceConfigs.First();
            var azureConfig = config.AzureIoTHubConfig;

            Console.WriteLine("Retriving list of current job(s):");
            var registryManager = RegistryManager.CreateFromConnectionString(azureConfig.ConnectionString);

            var cts = new CancellationTokenSource();

            GetCurrentJobListFromHubAsync(registryManager, cts.Token).Wait(cts.Token);;

            Console.ReadLine();
        }
    }
}
