using CommandLineParser.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.IoTHub.Examples.CSharp.CreateDeviceIdentity
{
    class CommandLineArguments
    {
        [ValueArgument(typeof(int), 'n', "number-of-devices", Description = "Number of devices to create", DefaultValue = -1)]
        public int NumberOfDeviceToCreate;

        [ValueArgument(typeof(string), 'p', "device-name-prefix", Description = "Prefix of device name", DefaultValue = "dev")]
        public string DeviceNamePrefix;

    }
}
