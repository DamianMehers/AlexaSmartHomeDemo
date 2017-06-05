# C# Alexa Smart Home demo
This is the code associated with a live-coding session I recorded when I created an [Amazon Echo Smart Home skill](https://developer.amazon.com/public/solutions/alexa/alexa-skills-kit/overviews/understanding-the-smart-home-skill-api) from scratch, in C#, making use of:
* [AWS Lambdas (C#)](http://docs.aws.amazon.com/lambda/latest/dg/dotnet-programming-model-handler-types.html) to have OAUTH and the smart home skill discovery and control requests
* [AWS API Gateway](https://aws.amazon.com/api-gateway/) to expose the lambda handling oauth to the web
* [AWS Simple Queue Services](https://aws.amazon.com/sqs/) to send commands from the lambda handling smart home requests to a PC where the requests are actually handled
* [The Alexa Smart Home Skill console](https://developer.amazon.com/edw/home.html#/skills) to define the skill
* A console app to receive SQS requests and turn off the computer monitor in response.

The result is a smart home skill that lets you turn off your computer monitor using your Amazon Echo.

Once the live-coding session is [here](https://youtu.be/ajzGjIXPg54)

<img src="images/overview.png">
