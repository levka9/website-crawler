using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Logic.Modules.Interfaces;
using WebsiteCrawler.Model.Enums;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic.Modules
{
    public class MultiThreadWebsiteParserModule : IMultiThreadWebsiteParserModule
    {
        #region Properties
        private ILogger<MultiThreadWebsiteParserModule> _log;
        private int _maxDeep;
        private int _maxTaskQuantity;
        private EDomainLevel _domainLevel;
        private List<Task> _tasks;
        private IEnumerable<string> _domainExtentions;

        private IPageDataParserModule _pageDataParserModule;
        private IWebsiteParserModule _websiteParserModule;
        #endregion

        public MultiThreadWebsiteParserModule(IPageDataParserModule pageDataParserModule,
                                              IWebsiteParserModule websiteParserModule, 
                                              ILogger<MultiThreadWebsiteParserModule> log)
        {
            _log = log;

            _websiteParserModule = websiteParserModule;
            _pageDataParserModule = pageDataParserModule;
        }

        public async Task StartAsync(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
        {
            Init(multiThreadWebsiteParserRequest);

            var taskCounter = 0;

            while (true)
            {
                while (_tasks.Count < _maxTaskQuantity)
                {
                    var domainName = string.Empty;
                    WebSitesConcurrentQueue.WebSites.TryDequeue(out domainName);

                    if (!string.IsNullOrEmpty(domainName))
                    {
                        _tasks.Add(CreateWebsiteParser(domainName, taskCounter++));
                    }
                }

                var completedTask = await Task.WhenAny(_tasks.ToArray());
                _tasks.Remove(completedTask);

                Thread.Sleep(200);

                Console.WriteLine($"Task id: {completedTask.Id} completed");
                Console.WriteLine($"Total webSites in queue: {WebSitesConcurrentQueue.WebSites.Count}");
            }
        }

        private void Init(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
        {
            _tasks = new List<Task>();

            _maxDeep = multiThreadWebsiteParserRequest.MaxDeep;
            _domainLevel = multiThreadWebsiteParserRequest.EDomainLevel;
            _domainExtentions = multiThreadWebsiteParserRequest.DomainExtentions;
            _maxTaskQuantity = multiThreadWebsiteParserRequest.MaxTaskQuantity;

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>(multiThreadWebsiteParserRequest.WebsiteUrls);
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();
        }

        private async Task CreateWebsiteParser(string WebsiteName, int? taskCounter)
        {
            var websiteParserRequest = GetWebsiteParserRequest(WebsiteName, taskCounter);

            try
            {
                await _websiteParserModule.Parse(websiteParserRequest);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MultiThreadWebsiteParser - CreateWebsiteParser");
            }
        }

        private WebsiteParserModuleRequest GetWebsiteParserRequest(string domainName, int? taskId)
        {
            return new WebsiteParserModuleRequest()
            {
                DomainName = domainName,
                MaxDeep = _maxDeep,
                DomainLevel = _domainLevel,
                DomainExtentions = _domainExtentions,
                TaskId = taskId
            };
        }
    }
}
