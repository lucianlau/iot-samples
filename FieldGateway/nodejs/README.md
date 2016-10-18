# iot-samples/FieldGateway/NodeJS
This folder includes Azure IoT Gateway example projects for NodeJS including a demo [device simulator](/FieldGateway/nodejs/device_simulator/simulator.js)and a sample Field Gateway that implements batch messaging and compression. 

Please note that the binaries in the [/bin](/FieldGateway/nodejs/bin) folder are compiled for x86 and may not run on your target architecture.  Architecture specific instructions can be found in the [BUILD.md](/FieldGateway/nodejs/BUILD.md) file.

# Prerequisites
* x86 compatible architecture. 
* NodeJS & NPM installed

# Getting Started
* In the root of the project run `npm install`
* Open the [gateway.json](/FieldGateway/nodejs/gateway.json) file and update the connection string 
under the `iothub_writer` module to map to your IoT Hub connection string.  The connection string 
should include the registerd device id of the field gateway; i.e. `HostName=foo.bar.azure-device.net;DeviceId=foo;SharedAccessKey=bar`
* Open a terminal window in the `FieldGateway/nodejs` folder and run the following command: `.\bin\gateway.exe gateway.json`
* Start the device simulator by following the [Quick Start](](/FieldGateway/nodejs/device_simulator/README.md#quick-start) section of the device_simulator README.

The device simulator will begin pushing messages at the field gateway, which will in turn, batch 10 messages, compress and forward them on to your IoT Hub.
 