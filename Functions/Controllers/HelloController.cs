using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

namespace Functions
{
    // based on work by Erwin de Vries and https://devkimchi.com/2021/08/13/azure-functions-openapi-on-net5/

    public class HelloController
    {
        ILogger logger { get; }

        public HelloController(ILogger<HelloController> logger)
        {
            this.logger = logger;
        }

        [Function("HelloGet")]
        [OpenApiOperation(operationId: "greeting", 
            tags: new[] { "greeting" }, 
            Summary = "Greetings method", 
            Description = "This shows a welcome message.", 
            Visibility = OpenApiVisibilityType.Important)]
        public HttpResponseData HelloGet([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Hello")] HttpRequestData req,
            FunctionContext executionContext)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        [Function("HelloPost")]
        public HttpResponseData HelloPost([HttpTrigger(AuthorizationLevel.Function, "post", Route = "Hello")] 
        HttpRequestData req,
            FunctionContext executionContext)
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            try
            {
                HelloBody data = JsonConvert.DeserializeObject<HelloBody>(requestBody);

                if (data == null)
                    return req.CreateResponse(HttpStatusCode.BadRequest);

                string name = data.Name;

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

                response.WriteString($"Welcome {name}!");

                return response;

            } 
            catch(JsonException e)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }       
        }
    }
}
