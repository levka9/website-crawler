using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.BenchmarkTests
{
    public class WebPageParserTests
    {
        private const string _url = "https://www.ynet.co.il/";
        private static HttpClient _httpClient;
        private static WebClient _webClient;

        [GlobalSetup]
        public void GlobalSetup()
        { 
            _httpClient = new HttpClient();
            _webClient = new WebClient();
        }

        [Benchmark]
        public async Task HttpClientTest()
        {
            var response = await _httpClient.GetAsync(_url);
            response.EnsureSuccessStatusCode();

            var htmlPageContent = await response.Content.ReadAsStringAsync();
        }

        [Benchmark]
        public async Task WebClientTest()
        {
            var uri = new Uri(_url);
            var stringContent = await _webClient.DownloadStringTaskAsync(_url);

        }

        [Benchmark]
        public void WebRequest() 
        { 
            var request = (HttpWebRequest)System.Net.WebRequest.Create(_url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string responseContent = reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
