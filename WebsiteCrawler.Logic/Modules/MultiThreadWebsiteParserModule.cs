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
        private int _totalWebsiteCompleted; 
        private EDomainLevel _domainLevel;
        private WebsiteParserLimitsRequest _websiteParserLimitsRequest;
        private List<Task> _tasks;
        private IEnumerable<string> _domainExtentions;

        private IWebsiteParserModule _websiteParserModule;
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

            var taskCounter = 1;

            while (true)
            {
                while (_tasks.Count < _maxTaskQuantity)
                {
                    var domainName = string.Empty;
                    WebSitesConcurrentQueue.WebSites.TryDequeue(out domainName);

                    if (!string.IsNullOrEmpty(domainName))
                    {
                        Func<int> getTaskIdAction = null;
                        var task = new Task(async() => 
                                    { 
                                        await CreateWebsiteParser(domainName, taskCounter++,  getTaskIdAction); 
                                    }, 
                                    TaskCreationOptions.LongRunning);

                        getTaskIdAction = delegate ()
                        {
                            return task.Id;
                        };

                        task.Start();
                        _tasks.Add(task);
                    }
                }

                var completedTask = await Task.WhenAny(_tasks.ToArray());

                _tasks.Remove(completedTask);

                //Thread.Sleep(200);
                #region Write log then task complete
                Console.WriteLine($"Task id: {completedTask.Id} completed IsCompleted: {completedTask.IsCompleted}");
                Console.WriteLine($"Total completed {++_totalWebsiteCompleted} websites.");
                Console.WriteLine($"Total webSites in queue: {WebSitesConcurrentQueue.WebSites.Count}");
                #endregion
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

        private async Task CreateWebsiteParser(string websiteName, int taskCounter, Func<int> getTaskIdAction)
        {
            var websiteParserRequest = GetWebsiteParserRequest(websiteName, taskCounter, getTaskIdAction);

            try
            {
                await _websiteParserModule.ParseAsync(websiteParserRequest);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "MultiThreadWebsiteParser - CreateWebsiteParser");
            }
        }

        private WebsiteParserModuleRequest GetWebsiteParserRequest(string domainName, int taskCounter, Func<int> getTaskIdAction)
        {
            return new WebsiteParserModuleRequest()
            {
                TaskId = getTaskIdAction.Invoke(),
                DomainName = domainName,
                DomainLevel = _domainLevel,
                WebsiteParserLimitsRequest = _websiteParserLimitsRequest,
                DomainExtentions = _domainExtentions,
                TaskCounter = taskCounter
            };
        }

        public int GetTaskIdInvoke(int taskId)
        {
            return taskId;
        }
    }
}
