using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.SQS;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MonitorOffHandlerLambda
{
    // Smart Home Skill handler - discovery and 
    public class Function
    {
        private const string ServiceURL = "http://sqs.us-east-1.amazonaws.com";
        private const string QueueUrl = "https://sqs.us-east-1.amazonaws.com/603843067738/MonitorControlQueue";

        public async Task<object> FunctionHandler(SmartHomeRequestResponse input, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log(input + Environment.NewLine);
                if (input.Header.Name == "DiscoverAppliancesRequest")
                {
                    // Return a hardcoded list of devices in response to the discover request
                    // This response would not normally be hardcoded ... instead look at what devices the user really has
                    var response = new DiscoverDevicesResponse
                    {
                        Header = input.Header,
                        Payload = new DiscoverDevicesResponsePayload
                        {
                            DiscoveredAppliances = new List<DiscoveredAppliance>()
                        }
                    };
                    var monitor = new DiscoveredAppliance
                    {
                        Actions = new List<string> {"turnOn", "turnOff"},
                        ApplianceId = "MyComputerMonitor",
                        ApplianceTypes = new List<string> {"SWITCH"},
                        FriendlyDescription = "Your friendly computer monitor :-)",
                        FriendlyName = "Bobby",
                        IsReachable = true,
                        ManufacturerName = "Damian Mehers",
                        ModelName = "Hal 9000",
                        Version = "1.0"
                    };
                    response.Payload.DiscoveredAppliances.Add(monitor);
                    response.Header.Name = "DiscoverAppliancesResponse";
                    return response;

                }

                // Send the request to a computer that is reading from the queue
                var sqsClient = new AmazonSQSClient(new AmazonSQSConfig { ServiceURL = ServiceURL });
                await sqsClient.SendMessageAsync(QueueUrl, JsonConvert.SerializeObject(input));

                // The turn on and turn off requests are also hardcoded to say that they worked, whether or not they actually did.
                // In real life you'd only send the response once you'd received the response from the device.
                if (input.Header.Name == "TurnOnRequest")
                {
                    if (input.Payload.Appliance.ApplianceId == "MyComputerMonitor")
                    {
                        var response = new SmartHomeRequestResponse
                        {
                            Header = input.Header,
                            Payload = null
                        };
                        response.Header.Name = "TurnOnConfirmation";
                        return response;
                    }
                }
                if (input.Header.Name == "TurnOffRequest")
                {
                    if (input.Payload.Appliance.ApplianceId == "MyComputerMonitor")
                    {
                        var response = new SmartHomeRequestResponse
                        {
                            Header = input.Header,
                            Payload = null
                        };
                        response.Header.Name = "TurnOffConfirmation";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log(ex + Environment.NewLine);
            }
            return null;
        }
    }
    public class Header
    {

        [JsonProperty("messageId")]
        public string MessageId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("namespace")]
        public string Namespace { get; set; }

        [JsonProperty("payloadVersion")]
        public string PayloadVersion { get; set; }
    }

    public class Appliance
    {
        [JsonProperty("applianceId")]
        public string ApplianceId { get; set; }
    }

    public class Payload
    {

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("appliance")]
        public Appliance Appliance { get; set; }
    }

    public class SmartHomeRequestResponse
    {

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }


    
    public class DiscoveredAppliance
    {

        [JsonProperty("actions")]
        public IList<string> Actions { get; set; }

        [JsonProperty("applianceTypes")]
        public IList<string> ApplianceTypes { get; set; }

        [JsonProperty("applianceId")]
        public string ApplianceId { get; set; }

        [JsonProperty("friendlyDescription")]
        public string FriendlyDescription { get; set; }

        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonProperty("isReachable")]
        public bool IsReachable { get; set; }

        [JsonProperty("manufacturerName")]
        public string ManufacturerName { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class DiscoverDevicesResponsePayload
    {

        [JsonProperty("discoveredAppliances")]
        public IList<DiscoveredAppliance> DiscoveredAppliances { get; set; }
    }
    

    public class ControlResponse
    {

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }


    public class DiscoverDevicesResponse
    {

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("payload")]
        public DiscoverDevicesResponsePayload Payload { get; set; }
    }

}
