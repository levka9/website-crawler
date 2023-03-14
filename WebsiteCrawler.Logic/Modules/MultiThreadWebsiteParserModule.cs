using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Interfaces;
using WebsiteCrawler.Model.Enums;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic.Modules
{
    public class MultiThreadWebsiteParserModule : IMultiThreadWebsiteParserModule
    {
        #region Properties
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const int MAX_TASK_QUANTITY = 6;

        int maxDeep;
        EDomainLevel domainLevel;
        List<Task> tasks;
        IEnumerable<string> domainExtentions;
        #endregion

        public async Task StartAsync(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
        {
            Init(multiThreadWebsiteParserRequest);

            var taskCounter = 0;

            while (true)
            {
                while (tasks.Count < MAX_TASK_QUANTITY)
                {
                    var domainName = string.Empty;
                    WebSitesConcurrentQueue.WebSites.TryDequeue(out domainName);

                    if (!string.IsNullOrEmpty(domainName))
                    {
                        tasks.Add(CreateWebsiteParser(domainName, taskCounter++));
                    }
                }

                var completedTask = await Task.WhenAny(tasks.ToArray());
                tasks.Remove(completedTask);

                Thread.Sleep(200);

                Console.WriteLine($"Task id: {completedTask.Id} completed");
                Console.WriteLine($"Total webSites in queue: {WebSitesConcurrentQueue.WebSites.Count}");
            }
        }

        private void Init(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
        {
            tasks = new List<Task>();

            maxDeep = multiThreadWebsiteParserRequest.MaxDeep;
            domainLevel = multiThreadWebsiteParserRequest.EDomainLevel;
            domainExtentions = multiThreadWebsiteParserRequest.DomainExtentions;

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>(multiThreadWebsiteParserRequest.WebsiteUrls);
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();
        }

        private async Task CreateWebsiteParser(string WebsiteName, int? taskCounter)
        {
            var websiteParserRequest = GetWebsiteParserRequest(WebsiteName, taskCounter);

            var websiteParser = new WebsiteParser(websiteParserRequest);

            try
            {
                await websiteParser.Parse();
            }
            catch (Exception ex)
            {
                log.Error("MultiThreadWebsiteParser - CreateWebsiteParser", ex);
            }
        }

        private WebsiteParserRequest GetWebsiteParserRequest(string domainName, int? taskId)
        {
            return new WebsiteParserRequest()
            {
                DomainName = domainName,
                MaxDeep = maxDeep,
                DomainLevel = domainLevel,
                DomainExtentions = domainExtentions,
                TaskId = taskId
            };
        }
    }
}
