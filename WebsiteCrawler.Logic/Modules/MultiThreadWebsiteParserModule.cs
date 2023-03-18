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
        private int _maxTaskQuantity;
        private EDomainLevel _domainLevel;
        private WebsiteParserLimitsRequest _websiteParserLimitsRequest;
        private List<Task> _tasks;
        private IEnumerable<string> _domainExtentions;

        private IWebsiteParserModule _websiteParserModule;
        private Action<int> _getTaskIdAction;
        #endregion

        public MultiThreadWebsiteParserModule(IWebsiteParserModule websiteParserModule, 
                                              ILogger<MultiThreadWebsiteParserModule> log)
        {
            _log = log;
            _websiteParserModule = websiteParserModule;
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
                        var task = new Task(delegate { CreateWebsiteParser(domainName, taskCounter++); });
                        _tasks.Add(task);
                    }
                }

                var completedTask = await Task.WhenAny(_tasks.ToArray());

                _tasks.Remove(completedTask);
                Console.WriteLine($"Task id: {completedTask.Id} completed IsCompleted: {completedTask.IsCompleted}");

                Thread.Sleep(200);
                Console.WriteLine($"Total webSites in queue: {WebSitesConcurrentQueue.WebSites.Count}");
            }
        }

        private void Init(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
        {
            _tasks = new List<Task>();

            _domainLevel = multiThreadWebsiteParserRequest.EDomainLevel;
            _domainExtentions = multiThreadWebsiteParserRequest.DomainExtentions;
            _maxTaskQuantity = multiThreadWebsiteParserRequest.MaxTaskQuantity;
            _websiteParserLimitsRequest = new WebsiteParserLimitsRequest();
            _websiteParserLimitsRequest.MaxDeep = multiThreadWebsiteParserRequest.WebsiteParserLimits.MaxDeep;
            _websiteParserLimitsRequest.MaxInternalLinks = multiThreadWebsiteParserRequest.WebsiteParserLimits.MaxInternalLinks;

            WebSitesConcurrentQueue.WebSites = new ConcurrentQueue<string>(multiThreadWebsiteParserRequest.WebsiteUrls);
            WebSitesConcurrentQueue.AllWebSites = new ConcurrentQueue<string>();
        }

        private async Task CreateWebsiteParser(string websiteName, int taskCounter)
        {
            var websiteParserRequest = GetWebsiteParserRequest(websiteName, taskCounter);

            try
            {
                websiteParserRequest.TaskId = Task.CurrentId;
                await _websiteParserModule.ParseAsync(websiteParserRequest);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MultiThreadWebsiteParser - CreateWebsiteParser");
            }
        }

        private WebsiteParserModuleRequest GetWebsiteParserRequest(string domainName, int taskCounter)
        {
            return new WebsiteParserModuleRequest()
            {
                DomainName = domainName,
                DomainLevel = _domainLevel,
                WebsiteParserLimitsRequest = _websiteParserLimitsRequest,
                DomainExtentions = _domainExtentions,
                TaskId = Task.CurrentId,
                TaskCounter = taskCounter
            };
        }

        private int GetTaskIdAction(int taskId)
        {
            return taskId;
        }
    }
}
