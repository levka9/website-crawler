using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic
{
    public class MultiThreadWebsiteParserTest
    {
        #region Properties
        const int MAX_TASK_QUANTITY = 10;

        int maxDeep;
        List<Task> tasks;
        IEnumerable<string> domainExtentions;
        #endregion

        public MultiThreadWebsiteParserTest(MultiThreadWebsiteParserRequest MultiThreadWebsiteParserRequest)
        {
            tasks = new List<Task>();

            maxDeep = MultiThreadWebsiteParserRequest.MaxDeep;
            domainExtentions = MultiThreadWebsiteParserRequest.DomainExtentions;

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>(MultiThreadWebsiteParserRequest.WebsiteUrls);
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();
        }

        public async Task Start()
        {

            while (true)
            {
                while (tasks.Count < MAX_TASK_QUANTITY)
                {
                    var websiteName = string.Empty;
                    WebSitesConcurrentQueue.WebSites.TryDequeue(out websiteName);

                    if (!string.IsNullOrEmpty(websiteName))
                    {                            
                        var task = new Task(x =>
                        {
                            var websiteParserRequest = GetWebsiteParserRequest(websiteName, Task.CurrentId);
                            
                            var websiteParser = new WebsiteParserTest(websiteParserRequest);
                            websiteParser.Parse();
                        }, TaskCreationOptions.LongRunning);

                        task.Start();                        
                        tasks.Add(task);
                    }                    
                }

                //Task.WaitAll(tasks.ToArray());
                //Thread.Sleep(500);
                var completedTask = await Task.WhenAny(tasks.ToArray());                
                tasks.Remove(completedTask);
                System.Console.WriteLine($"Task id: {completedTask.Id} completed");
                Thread.Sleep(2000);
            }                
        }

        private WebsiteParserRequest GetWebsiteParserRequest(string WebsiteName, int? TaskId)
        {
            return new WebsiteParserRequest()
            {
                WebsiteUrl = WebsiteName,
                MaxDeep = maxDeep,
                DomainExtentions = domainExtentions,
                TaskId = TaskId
            };
        }
    }
}
