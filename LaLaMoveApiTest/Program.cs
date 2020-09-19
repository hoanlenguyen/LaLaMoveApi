using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LaLaMoveApiTest
{
    class Program
    {
        const string BASE_URL = "https://sandbox-rest.lalamove.com";
        const string API_KEY = "02bd00b1a3304b8a9175cbc4b7cc7f5d";
        const string SECRET = "MC0CAQACBQC+5UT/AgMBAAECBQCvL77RAgMAyAMCAwD0VQIDAK5dAgIsOQIC";
        //const string PATH = "/v2/quotations";
        const string SERVICE_ID = "LALA2H";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //using var stream = new StreamReader("D:/C#/Test/LaLaMoveApiTest/BodyContent.json");
            //var json = stream.ReadToEnd();
            //var obj = JsonConvert.DeserializeObject(json);
            //var body = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //Console.WriteLine(body);

            var points = new OrderPoint[]
            {
                new OrderPoint
                {
                    Location=new Location{Latitude="10.777888",Longitude="106.684554"},
                    Addresses=new Address
                    {
                        VNAddress =new LanguageString
                        {
                            DisplayString="35 Nguyễn Thông, phường 7, quận 3, TPHCM"
                        }
                    }
                    
                },
                new OrderPoint
                {
                    Location = new Location{Latitude = "10.841889",Longitude = "106.809325"},
                    Addresses = new Address{VNAddress = 
                    new LanguageString{
                            DisplayString = "DH FPT Q9, TP.HCM"
                        }
                    }

                }
            };
            var contacts = new Contact[]
            {
                new Contact{Name = "Nguyễn Văn A",Phone = "0987655133"},
                new Contact{Name = "Nguyễn Văn B",Phone = "0987654123"}
            };
            var nextDate = DateTime.Now.AddDays(1);

            var estimate = await CalculateOrderAsync(nextDate, points, contacts);
            var order = await CreateOrderAsync(nextDate, 84000, points, contacts);
            Console.WriteLine(estimate);
            Console.WriteLine(order);
        }

        static string CreateRequestBody(DateTime? orderTime, int?cost, OrderPoint[] points, Contact[] contacts)
        {
            var toAddress = new List<object> { new { toStop = 1, toContact = contacts[1] } };
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("sms", true);
            dictionary.Add("serviceType", SERVICE_ID);
            dictionary.Add("stops", points);
            dictionary.Add("requesterContact", contacts[0]);
            dictionary.Add("deliveries", toAddress);

            if (orderTime != null)
                dictionary.Add("scheduleAt", orderTime.Value.ToString("s") + "Z");

            if (cost != null)
            {
                var fee = new { amount = cost.ToString(), currency = "VND" };
                dictionary.Add("quotedTotalFee", fee);
            }

            return JsonConvert.SerializeObject(dictionary);
        }

        static string GenerateToken(string body, HttpMethod httpMethod, string path)
        {
            var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var method = httpMethod.Method.ToUpper();
            var rawSignature = $"{time}\r\n{method}\r\n{path}\r\n\r\n{body}";
            byte[] keyByte = Encoding.UTF8.GetBytes(SECRET);
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawSignature);
            byte[] hashmessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);
            var signature = string.Concat(Array.ConvertAll(hashmessage, x => x.ToString("x2")));
            var token = $"{API_KEY}:{time}:{signature}";

            return token;
        }

        static HttpRequestMessage CreateRequestMessage(HttpMethod method, string path, string token, string body)
        {
            var url = $"{BASE_URL}{path}";
            //var requestId = Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.TryAddWithoutValidation("Authorization", $"hmac {token}");
            request.Headers.TryAddWithoutValidation("X-LLM-Country", "VN");
            request.Headers.TryAddWithoutValidation("X-Request-ID", /*requestId*/ "e05383df-d42f-4bb0-b5f3-e56cfe7f717c");
            request.Content = new StringContent(body);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            return request;
        }

        static async Task<string> CalculateOrderAsync(DateTime? orderTime, OrderPoint[] points, Contact[] contacts)
        {
            var path = "/v2/quotations";
            var body = CreateRequestBody(orderTime, null, points, contacts);
            var token = GenerateToken(body, HttpMethod.Post, path);
            using var client = new HttpClient();
            var request = CreateRequestMessage(HttpMethod.Post, path, token, body);
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        static async Task<string> CreateOrderAsync(DateTime? orderTime, int? fee, OrderPoint[] points, Contact[] contacts)
        {
            var path = "/v2/orders";
            var body = CreateRequestBody(orderTime, fee, points, contacts);
            //Console.WriteLine(body);
            var token = GenerateToken(body, HttpMethod.Post, path);
            using var client = new HttpClient();
            var request = CreateRequestMessage(HttpMethod.Post, path, token, body);            
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

    }
}