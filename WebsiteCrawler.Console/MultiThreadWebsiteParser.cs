using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;

namespace WebsiteCrawler.Console
{
    public class MultiThreadWebsiteParser
    {
        const int MAX_TASK_QUANTITY = 10;

        int deep;
        List<Task> tasks;
        Queue<string> webSites;
        int totalTasks;

        public MultiThreadWebsiteParser(Queue<string> WebSites, int Deep)
        {
            tasks = new List<Task>();

            deep = Deep;
            webSites = WebSites;

            totalTasks = 0;
        }

        public void Start()
        {

            while (true)
            {
                while (webSites.Count > 0)
                {
                    var websiteName = webSites.Dequeue();

                    var task = new Task(async () =>
                    {
                        var websiteParser = new WebsiteParser(websiteName, deep);
                        await websiteParser.Parse();
                    });

                    task.Start();
                    tasks.Add(task);
                }                

                Task.WaitAny(tasks.ToArray());
            }                
        }
    }
}
