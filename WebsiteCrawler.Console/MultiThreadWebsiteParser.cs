using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebsiteCrawler.Logic;

namespace WebsiteCrawler.Console
{
    public class MultiThreadWebsiteParser
    {
        List<Task> tasks;

        public MultiThreadWebsiteParser()
        {
            tasks = new List<Task>();
        }

        public void Start()
        {
            var task = new Task(async () => {
                var websiteParser = new WebsiteParser("https://www.nytimes.com", 2);
                await websiteParser.Parse();
            });

            var task2 = new Task(async () => {
                var websiteParser = new WebsiteParser("https://buywordpress.co.il", 2);
                await websiteParser.Parse();
            });

            var task3 = new Task(async () => {
                var websiteParser = new WebsiteParser("https://www.mgweb.co.il", 2);
                await websiteParser.Parse();
            });

            task.Start();
            task2.Start();
            task3.Start();

            tasks.Add(task);
            tasks.Add(task2);
            tasks.Add(task3);

            Task.WaitAll(tasks.ToArray());
        }
    }
}
