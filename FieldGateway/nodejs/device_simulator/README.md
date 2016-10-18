# iot-samples/FieldGateway/NodeJs

This device simulator can be used to explore the NodeJs Field Gateway workflow example.  The simulator emits http requests that are received by the gateway's [receiver.js](/FieldGateway/nodejs/js/receiver.js) module, imitating a protocol translation step often found in industrial gateway devices and applications.       

# Prerequisites
* x86 compatible architecture. 
* NodeJS & NPM installed

## Quick Start
* Open a terminal window in the `device_simulator` folder and run `npm install`.  
* Open the `simulator.js` file - set the `deviceUrl` and `port` properties.
* Opening a new terminal window on the target device.
* Run the command `node simulator.js`. 

## License
This project is licensed under the [MIT License](/LICENSE.txt)
