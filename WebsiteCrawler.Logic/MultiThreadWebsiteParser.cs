using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic
{
    public class MultiThreadWebsiteParser
    {
        #region Properties
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const int MAX_TASK_QUANTITY = 6;

        int maxDeep;
        List<Task> tasks;
        IEnumerable<string> domainExtentions;
        #endregion

        public MultiThreadWebsiteParser(MultiThreadWebsiteParserRequest MultiThreadWebsiteParserRequest)
        {
            tasks = new List<Task>();

            maxDeep = MultiThreadWebsiteParserRequest.MaxDeep;
            domainExtentions = MultiThreadWebsiteParserRequest.DomainExtentions;

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>(MultiThreadWebsiteParserRequest.WebsiteUrls);
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();
        }

        public async Task Start()
        {
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

                System.Console.WriteLine($"Task id: {completedTask.Id} completed");
                System.Console.WriteLine($"Total webSites in queue: {WebSitesConcurrentQueue.WebSites.Count}");
            }
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

        private WebsiteParserRequest GetWebsiteParserRequest(string DomainName, int? TaskId)
        {
            return new WebsiteParserRequest()
            {
                DomainName = DomainName,
                MaxDeep = maxDeep,
                DomainExtentions = domainExtentions,
                TaskId = TaskId
            };
        }
    }
}
