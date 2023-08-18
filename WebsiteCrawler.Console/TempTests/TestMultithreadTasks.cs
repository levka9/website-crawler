using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteCrawler.Console.Configuration;

namespace WebsiteCrawler.Console.TempTests
{
    public class TestMultithreadTasks
    {
        private static int _currentTaskQuantity = 0;
        private int _maxTaskQuantity;

        public TestMultithreadTasks(IConfigurationRoot? configuration)
        {
            _maxTaskQuantity = 2; // configuration.GetValue<int>(AppSettingsParameters.PARSER_MULTITHREAD_PARSER_MAX_TASK_QUANTITY);
        }

        public async Task Start()
        {
            var tasks = new List<Task>();

            while (true)
            {
                for (int i = 0; i < _maxTaskQuantity && tasks.Count < _maxTaskQuantity; i++)
                {
                    Func<int> getTaskId = null;
                    Task task = new Task(async () =>
                    {
                        DoWork(_currentTaskQuantity++, getTaskId);
                    }, TaskCreationOptions.LongRunning);

                    getTaskId = delegate ()
                    {
                        return task.Id;
                    };

                    task.Start();
                    tasks.Add(task);
                }

                var completedTask = await Task.WhenAny(tasks.ToArray());
                tasks.Remove(completedTask);
                System.Console.Write("Task id {0} completed his work.\r\n", completedTask.Id);
                Thread.Sleep(2000);
            }
        }

        private static void DoWork(int Counter, Func<int> getTaskId)
        {
            var counter = Counter;
            int taskId = getTaskId.Invoke();

            System.Console.WriteLine($"Task id: {taskId}");

            while (counter == 1 || counter == 2 || counter == 3)
            {
                System.Console.Write("Counter value is {0}\r\n", counter);

                Thread.Sleep(2000);
            }

            System.Console.Write("Counter value is {0} finished\r\n", counter);
        }
    }
}
