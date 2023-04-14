using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Transport;
using Elasticsearch.Net;
using log4net.Repository.Hierarchy;
using Microsoft.Extensions.Configuration;
using Nest;
using Nest.JsonNetSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebsiteCrawler.Model.Responses;

namespace WebsiteCrawler.Console.Configuration
{
    public static class ElasticsearchClientConfig
    {

        public static IElasticClient GetNestClient(IConfigurationRoot configuration)
        {
            var url = configuration.GetValue<string>("Elasticsearch:Url");
            var port = configuration.GetValue<int>("Elasticsearch:Port");
            var defaultIndex = configuration.GetValue<string>("Elasticsearch:DefaultIndex");
            var username = configuration.GetValue<string>("Elasticsearch:Username");
            var password = configuration.GetValue<string>("Elasticsearch:Password");
            var timeoutInSeconds = configuration.GetValue<int>("Elasticsearch:TimeoutInSeconds");

            var pool = new SingleNodeConnectionPool(new Uri($"{url}:{port}"));

            var connectionSettings = new ConnectionSettings(pool,
                                                            sourceSerializer: JsonNetSerializer.Default)
                                        .DefaultIndex(defaultIndex)
                                        .ThrowExceptions()
                                        .PrettyJson()
                                        .RequestTimeout(TimeSpan.FromSeconds(timeoutInSeconds));

            //.CertificateFingerprint("<FINGERPRINT>")
            //.Authentication(new BasicAuthentication(username, password));

            var client = new ElasticClient(connectionSettings);

            CreateIndicesAndMapEntities(client, defaultIndex);

            return client;
        }

        /// <summary>
        /// Create new index and map entities
        /// </summary>
        /// <param name="client"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="log"></param>
        private static void CreateIndicesAndMapEntities(IElasticClient client, string defaultIndex)
        {

            // Data model mapping
            var indicesRequest = new IndexExistsRequest(defaultIndex);

            if (!client.Indices.Exists(indicesRequest).Exists)
            {
                client.Indices.Create(defaultIndex, c => c.Map<PageDataParserModuleResponse>(m => m.AutoMap()));
            }
        }

        private static void ConfigureOptions(JsonSerializerOptions o) => o.PropertyNamingPolicy = null;
    }
}
