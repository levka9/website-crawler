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

        public static IElasticClient GetNestClient(IConfigurationRoot configuration, Serilog.Core.Logger log)
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

            CreateIndices(client, defaultIndex, log);

            return client;
        }

        private static void CreateIndices(IElasticClient client, string defaultIndex, Serilog.Core.Logger log)
        {

            // Data model mapping
            try 
            {
                client.Indices.Create(defaultIndex, c => c.Map<PageDataParserModuleResponse>(m => m.AutoMap()));
            }
            catch (Exception ex) 
            {
                log.Error(ex, ex.Message);
                //throw;
            }
        }

        private static void ConfigureOptions(JsonSerializerOptions o) => o.PropertyNamingPolicy = null;
    }
}
