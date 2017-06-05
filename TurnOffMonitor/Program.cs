using System;
using System.Runtime.InteropServices;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace TurnOffMonitor
{

    class Program
    {
        private const string ServiceURL = "http://sqs.us-east-1.amazonaws.com";
        private const string QueueUrl = "https://sqs.us-east-1.amazonaws.com/603843067738/MonitorControlQueue";


        private static int WM_SYSCOMMAND = 0x0112;
        private static uint SC_MONITORPOWER = 0xF170;
        public static void Main(string[] args)
        {

            var sqsClient = new AmazonSQSClient(new AmazonSQSConfig { ServiceURL = ServiceURL });
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = QueueUrl,
                WaitTimeSeconds = 20
            };
            for (;;)
            {
                Console.WriteLine("Waiting ...");
                var receiveMessageResponse = sqsClient.ReceiveMessage(receiveMessageRequest);
                foreach (var message in receiveMessageResponse.Messages)
                {
                    Console.WriteLine("Received: " + message.Body);
                    var request = JsonConvert.DeserializeObject<SmartHomeRequestResponse>(message.Body);
                    if (request.Header.Name == "TurnOffRequest")
                    {
                        SendMessage(GetConsoleWindow(), WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)2);
                    }
                    sqsClient.DeleteMessage(new DeleteMessageRequest {QueueUrl = QueueUrl, ReceiptHandle = message.ReceiptHandle});
                }
            }
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
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

    public class SmartHomeRequestResponse
    {

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }
    }
}
