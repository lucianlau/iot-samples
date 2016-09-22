const zlib = require('zlib');
const eventHubs = require('eventhubs-js');

//https://github.com/ytechie/eventhubs-js
//https://azure.microsoft.com/en-us/documentation/articles/functions-reference-node/#node-version-amp-package-management

module.exports = function (context, input) {

    var buffer =  Buffer.from(input, 'base64').slice(4);

    eventHubs.init({
        hubNamespace: 'postshred.servicebus.windows.net',
        hubName: 'demo',
        keyName: 'demo',
        key: 'GilIgSl7Rk8wMvuL27m9MyQjN8H5DTJV5WdsVJ+dlSM='
    });
    
    
    zlib.gunzip(buffer, (err, buffer) => {
        if (!err) {
            var decompressed = buffer.toString('utf-8');
            var messages = decompressed.split('|');
    
            messages.map(function(message){
                
                // process event hub relay here.

                context.log('Processing message:', message);
                
            });
            
        } else {
            context.log(err);// handle error
        }
    });
    context.done();
};