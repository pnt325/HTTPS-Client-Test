using Microsoft.VisualBasic;
using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            HttpClientHandler handler = new HttpClientHandler();
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ClientCertificates.Add(new X509Certificate2("cacert.pem"));
            handler.ServerCertificateCustomValidationCallback = ValidateServerSertificate;
            HttpClient client = new HttpClient(handler);
            while(true)
            {
                ShowContent(client);

                Console.WriteLine("_>");
                var cmd = Console.ReadLine();
                if(cmd != "next")
                {
                    break;
                }
            }
            
        }

        private static async void ShowContent(HttpClient client)
        {
            var content = new StringContent("Hello world");
            HttpResponseMessage response = await client.PostAsync("https://192.168.0.27/test", content);
            ShowHeaders(response.Headers);

            //response.EnsureSuccessStatusCode();

            Console.WriteLine($"Get sucess message - status code {(int)response.StatusCode}{response.ReasonPhrase}");

            Console.WriteLine("Starting read data");

            string bodyMsg = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Get message: {bodyMsg}");
        }

        private static bool ValidateServerSertificate(HttpRequestMessage requested, X509Certificate2? cert, X509Chain? chain, SslPolicyErrors arg4)
        {
            Console.WriteLine($"Requested URI: {requested.RequestUri}");
            Console.WriteLine($"Effective date: {cert?.GetEffectiveDateString()}");
            Console.WriteLine($"Exp date: {cert?.GetExpirationDateString()}");
            Console.WriteLine($"Issuer: {cert?.Issuer}");
            Console.WriteLine($"Subject: {cert?.Subject}");
            return true;
        }

        private static void ShowHeaders(HttpResponseHeaders headers)
        {
            Console.WriteLine("HEADER");
            foreach(var header in headers)
            {
                foreach(var value in header.Value)
                Console.WriteLine($"{header.Key, 25}: {value}");
            }
        }
    }
}