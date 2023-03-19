﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteCrawler.Console.TempTests
{
    public class TestMultithreadTasks
    {
        private static int Counter = 0;
        private List<Task> tasks;

        public async Task Start()
        {
            tasks = new List<Task>();

            while (true)
            {
                for (int i = 0; i < 5 && tasks.Count < 5; i++)
                {
                    Func<int> getTaskId = null;
                    Task worker = new Task(async () =>
                    {
                        DoWork(Counter++, getTaskId);
                    }, TaskCreationOptions.LongRunning);

                    getTaskId = delegate ()
                    {
                        return worker.Id;
                    };

                    worker.Start();
                    tasks.Add(worker);
                }

                var completedTask = await Task.WhenAny(tasks.ToArray());
                tasks.Remove(completedTask);
                System.Console.Write("Task id is {0} removed\r\n", completedTask.Id);
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
