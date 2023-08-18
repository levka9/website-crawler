using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Data.Elasticsearch;
using WebsiteCrawler.Logic.Modules.Interfaces;
using WebsiteCrawler.Model.Enums;
using WebsiteCrawler.Models.Requests;

namespace WebsiteCrawler.Logic.Modules
{
    public class MultiThreadWebsiteParserModule : IMultiThreadWebsiteParserModule
    {
        #region Properties
        private readonly object _lockTaskCounter = new object();
        private int _maxTaskQuantity;
        private int _totalWebsiteCompleted;
        private int _taskCounter;

        private ILoggerFactory _loggerFactory;
        private ILogger<MultiThreadWebsiteParserModule> _log;
        private EDomainLevel _domainLevel;
        private WebsiteParserLimitsRequest _websiteParserLimitsRequest;
        private IPageDataParserRepository _pageDataParserRepository;
        private List<Task> _tasks;
        private IEnumerable<string> _domainExtentions;
        #endregion

        public MultiThreadWebsiteParserModule(ILoggerFactory loggerFactory,
                                              IPageDataParserRepository pageDataParserRepository)
        {
            _loggerFactory = loggerFactory;
            _log = loggerFactory.CreateLogger<MultiThreadWebsiteParserModule>();
            _pageDataParserRepository = pageDataParserRepository;
        }

        public async Task StartAsync(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
        {
            await Init(multiThreadWebsiteParserRequest);

            _taskCounter = 1;

            await CreateTasks();
        }

        private async Task CreateTasks()
        {
            while (true)
            {
                while (_tasks.Count < _maxTaskQuantity)
                {
                    var domainName = string.Empty;
                    WebSitesConcurrentQueue.WebSites.TryDequeue(out domainName);

                    if (IsDomainNameValidAndNew(domainName))
                    {
                        Func<int> getTaskIdAction = null;
                        var task = new Task(async () =>
                        {
                            IncreaseTaskCounter();

                            await CreateWebsiteParser(domainName, _taskCounter, getTaskIdAction);
                        },
                                    TaskCreationOptions.LongRunning);

                        getTaskIdAction = delegate ()
                        {
                            return task.Id;
                        };

                        task.Start();
                        _tasks.Add(task);
                    }

                    Thread.Sleep(1000);
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

        private bool IsDomainNameValidAndNew(string domainName)
        {
            return !string.IsNullOrEmpty(domainName) &&
                   WebSitesConcurrentQueue.AllWebSites.FirstOrDefault(x => x == domainName) == null;
        }

        private async Task Init(MultiThreadWebsiteParserRequest multiThreadWebsiteParserRequest)
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

            await SetAlreadyParsedWebsites();
        }

        private async Task SetAlreadyParsedWebsites()
        {
            var websites = (await _pageDataParserRepository.GetAllAsync(1, int.MaxValue))
                                                           .Select(x => x.DomainName);

            foreach (var website in websites)
            {
                WebSitesConcurrentQueue.AllWebSites.Enqueue(website);
            }
        }

        private async Task CreateWebsiteParser(string websiteName, int taskCounter, Func<int> getTaskIdAction)
        {
            var websiteParserRequest = GetWebsiteParserRequest(websiteName, taskCounter, getTaskIdAction);

            try
            {
                var websiteParserModule = new WebsiteParserModule(_loggerFactory, _pageDataParserRepository);
                await websiteParserModule.ParseAsync(websiteParserRequest);
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

        private void IncreaseTaskCounter()
        {
            lock(_lockTaskCounter)
            {
                _taskCounter++;
            }            
        }

        public int GetTaskIdInvoke(int taskId)
        {
            return taskId;
        }
    }
}
