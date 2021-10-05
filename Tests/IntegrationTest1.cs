using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace Tests
{
    public class HelloFactory
    {
        public static IEnumerable<object[]> Names()
        {
            return new List<object[]>{
              new object[] { new HelloBody("Erwin") },
              new object[] { new HelloBody("Frank") },
              new object[] { new HelloBody("Mark") }
            };
        }
    }


    public class IntegrationTest1
    {
        private HttpClient client { get; }
        public IntegrationTest1()
        {
            string hostname = Environment.GetEnvironmentVariable("functionHostName");
            string apikey = Environment.GetEnvironmentVariable("functionApiKey");

            if (hostname == null)
                hostname  = $"http://localhost:{7071}";
            client = new HttpClient();
            client.BaseAddress = new Uri(hostname);
            
            if(apikey != null)
                client.DefaultRequestHeaders.Add("api-key", apikey);
        }

        [Fact]
        public void TestGet()
        {
            HttpResponseMessage response = client.GetAsync("api/Hello").Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string message = response.Content.ReadAsStringAsync().Result;

            Assert.Equal("Welcome to Azure Functions!", message);
        }

        [Fact]
        public void TestPostSuccess()
        {
            string testName = "Mark";
            HelloBody hello = new HelloBody(testName);
            HttpContent data = new StringContent(JsonConvert.SerializeObject(hello), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync("api/Hello", data).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string message = response.Content.ReadAsStringAsync().Result;

            Assert.Equal($"Welcome {testName}!", message);
        }

        [Theory]
        [MemberData(nameof(HelloFactory.Names), MemberType = typeof(HelloFactory))]
        public void TestPostTheorySuccess(HelloBody hello)
        {
            HttpContent data = new StringContent(JsonConvert.SerializeObject(hello), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync("api/Hello", data).Result;

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string message = response.Content.ReadAsStringAsync().Result;

            Assert.Equal($"Welcome {hello.Name}!", message);
        }

        [Fact]
        public void TestPostFailure()
        {
            HttpResponseMessage response = client.PostAsync("api/Hello", null).Result;

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
