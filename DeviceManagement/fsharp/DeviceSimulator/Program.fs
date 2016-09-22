// Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See full license at the bottom of this file.
// Learn more about F# at http://fsharp.org

namespace Azure.IoTHub.Examples.FSharp.SimulatedDevice

open System 
open System.Linq
open System.Text
open System.IO
open System.IO.Compression
open System.Device.Location
open System.Threading
open FSharp.Configuration
open Microsoft.Azure.Devices.Client
open Newtonsoft.Json

type Config = YamlConfig<FilePath="../config/config.yaml", ReadOnly=true>

type telemetryDataPoint = {
    location : GeoCoordinate 
    deviceId : string 
    windSpeed : float 
    }

[<Measure>]
type meters 

module Main = 
    
    [<EntryPoint>]
    let main argv = 
        let config = Config()
        config.Load("../../../config/config.yaml")
        let iotHubUri   = config.AzureIoTHubConfig.IoTHubUri
        let deviceKey   = config.DeviceConfigs.First().Key
        let deviceId    = config.DeviceConfigs.First().DeviceId
    
        let deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey))
    
        let avgWindSpeed = 10.
        let rand = new Random()
    
        let windSpeedMessage location = 
                let telemetryReading = { 
                    deviceId = deviceId
                    windSpeed = (avgWindSpeed + rand.NextDouble() * 4. - 2.) 
                    location = location
                    }
                let json = JsonConvert.SerializeObject(telemetryReading)
                json

        let getRandomGeoCoordinate seed (lat : float) (long : float) (radius : float) : GeoCoordinate = 
            // check out http://gis.stackexchange.com/a/68275 for where this calc originated.
            let rand = new Random(seed)
            let u = rand.NextDouble()
            let v = rand.NextDouble()
            let w = radius / 111000. * Math.Sqrt(u)
            let t = 2. * Math.PI * v
            let x = (w * Math.Cos(t)) / Math.Cos(lat)
            let y = w * Math.Sin(t)
            GeoCoordinate(y+lat, x+long)         
           
        //http://madskristensen.net/post/Compress-and-decompress-strings-in-C

        let compress (data : string) = 
            let buffer = Encoding.UTF8.GetBytes(data)
            let ms = new MemoryStream()
            (   use zip = new GZipStream(ms, CompressionMode.Compress, true)
                zip.Write(buffer, 0, buffer.Length) )
            ms.Position <- 0L
            let compressed = Array.zeroCreate<byte> (int(ms.Length))
            ms.Read(compressed, 0, compressed.Length) |> ignore
            let gzipBuffer = Array.zeroCreate<byte> (int(compressed.Length) + 4)
            Buffer.BlockCopy(compressed, 0, gzipBuffer, 4, compressed.Length)
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzipBuffer, 0, 4)
            Convert.ToBase64String gzipBuffer

        let decompress (data : string) = 
            let gzipBuffer = Convert.FromBase64String(data)
            (   use memoryStream = new MemoryStream()
                let dataLength = BitConverter.ToInt32(gzipBuffer, 0)
                memoryStream.Write(gzipBuffer, 4, gzipBuffer.Length - 4)
                let buffer = Array.zeroCreate<byte> (int(dataLength))
                memoryStream.Position <- 0L
                (   use zip = new GZipStream(memoryStream, CompressionMode.Decompress)
                    zip.Read(buffer, 0, buffer.Length) |> ignore)
                Encoding.UTF8.GetString(buffer)
            )
            
                   
        let dataSendTask (data : string) =
            async {
                let compressedData = compress data
                let message = new Message(Encoding.UTF8.GetBytes(compressedData))
                deviceClient.SendEventAsync(message) 
                        |> Async.AwaitIAsyncResult 
                        |> Async.Ignore 
                        |> ignore
                printfn "%O > Sending message %s" (DateTime.Now.ToString()) (decompress compressedData)
                Thread.Sleep 5000
            }
        
        let nycSites = Array.init 10 (fun index -> getRandomGeoCoordinate index 40.7128 74.0059 (20000.))
        
        let dataStream = 
            Seq.initInfinite ( fun x -> 
                String.concat "|" (nycSites |> Array.map windSpeedMessage)
            )
            |> Seq.iter (fun x -> 
                dataSendTask x |> Async.RunSynchronously)
             
       
    
        printfn "%A" argv
        0 // return an integer exit code
    

//*********************************************************
//
//Azure.IotHub.Examples.FSharp, https://github.com/WilliamBerryiii/Azure.IotHub.Examples.FSharp
//
//Copyright (c) Microsoft Corporation
//All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// ""Software""), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:




// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.




// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************