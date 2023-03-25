using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebsiteCrawler.Console.Configuration
{
    public static class ElasticsearchClientConfig
    {
        public static ElasticsearchClient GetClient(IConfigurationRoot configuration)
        {
            var url = configuration.GetValue<string>("Elasticsearch:Url");
            var port = configuration.GetValue<int>("Elasticsearch:Port");
            var defuaultIndex = configuration.GetValue<string>("Elasticsearch:DefaultIndex");
            var username = configuration.GetValue<string>("Elasticsearch:Username");
            var password = configuration.GetValue<string>("Elasticsearch:Password");

            var singleNode = new SingleNodePool(new Uri(url));

            var settings = new ElasticsearchClientSettings(singleNode, 
                                                           sourceSerializer: (defaultSerializer, settings) => 
                                                           new DefaultSourceSerializer(settings, ConfigureOptions))
                                    .DefaultIndex(defuaultIndex)
                                    .ThrowExceptions()
                                    .DisableDirectStreaming()
                                    .PrettyJson();
                                    //.CertificateFingerprint("<FINGERPRINT>")
                                    //.Authentication(new BasicAuthentication(username, password));
            var client = new ElasticsearchClient(settings);

            return client;
        }

        private static void ConfigureOptions(JsonSerializerOptions o) => o.PropertyNamingPolicy = null;
    }
}
