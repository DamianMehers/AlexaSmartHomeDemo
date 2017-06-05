using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace MonitorOffAuthLambda
{
    // Minimal OAUTH implementation for testing skills
    public class Function
    {
        public object FunctionHandler(HttpRequest request, ILambdaContext context)
        {
            try
            {
                LambdaLogger.Log(request.HttpMethod + Environment.NewLine);
                if (request.HttpMethod == "GET")
                {
                    // Redirect with a code
                    var headers = new Dictionary<string, string>();
                    var redirectUrl = $"{request.QueryStringParameters["redirect_uri"]}?state={request.QueryStringParameters["state"]}&code=123456";
                    LambdaLogger.Log("Redirect URL:" + redirectUrl + Environment.NewLine);

                    headers.Add(HttpResponseHeader.Location.ToString(), redirectUrl);
                    var response = new LambdaResponse()
                    {
                        Body = "",
                        StatusCode = HttpStatusCode.Redirect,
                        Headers = headers
                    };
                    return response;
                }
                // Translate code into a token
                return new LambdaResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Body = JsonConvert.SerializeObject(new AccessTokenResponse {AccessToken = "why hello there", TokenType = "bearer"})
                };
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Oh no: " + ex + Environment.NewLine);
            }
            return request;
        }
    }

    public class AccessTokenResponse
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    public class LambdaResponse
    {

        [JsonProperty("statusCode")]
        public HttpStatusCode StatusCode { get; set; }
        [JsonProperty("headers")]
        public Dictionary<string, string> Headers { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }

    

   

    public class HttpRequest
    {

        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("httpMethod")]
        public string HttpMethod { get; set; }

        [JsonProperty("headers")]
        public Dictionary<string,string> Headers { get; set; }

        [JsonProperty("queryStringParameters")]
        public Dictionary<string, string> QueryStringParameters { get; set; }

        [JsonProperty("pathParameters")]
        public object PathParameters { get; set; }

        [JsonProperty("stageVariables")]
        public object StageVariables { get; set; }


        [JsonProperty("body")]
        public object Body { get; set; }

        [JsonProperty("isBase64Encoded")]
        public bool IsBase64Encoded { get; set; }
    }

}
