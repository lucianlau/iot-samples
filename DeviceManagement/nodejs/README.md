# iot-samples/FieldGateway/NodeJS
This folder includes Azure IoT Gateway example projects for NodeJS including a demo device simulator
and a sample gateway that implements batch messaging and compression. Note: this sample will only run 
on x86 compatible architectures.   

# Prerequisites
* x86 compatible architecture. 
* NodeJS installed

# Getting Started
* In the root of the project run `npm install`
* Open the [gateway.json](/FieldGateway/nodejs/gateway.json) file and update the connection string 
under the `iothub_writer` module to map to your IoT Hub connection string.  The connection string 
should include the registerd device id of the field gateway; i.e. `HostName=foo.bar.azure-device.net;DeviceId=foo;SharedAccessKey=bar`
* Open a terminal window in the FieldGateway/nodejs folder and run the following command: `.\bin\gateway.exe 
* Open a second terminal window in the FieldGateway/nodejs/device_simulator folder and run the following command: `node simulator.js`

The device simulator will begin pushing messages at the field gateway, which will in turn, batch 10 messages, compress and forward to your IoT Hub.
 