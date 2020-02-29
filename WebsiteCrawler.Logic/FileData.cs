using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebsiteCrawler.Logic
{
    public static class FileData
    {
        static ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        public static async Task Save<T>(string FileName, IEnumerable<T> List)
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                using (TextWriter tw = new StreamWriter(FileName))
                {
                    foreach (var item in List)
                    {
                        var objectToFile = JsonConvert.SerializeObject(item);
                        await tw.WriteLineAsync(objectToFile);
                    }
                }
            }
            finally
            {
                if (readerWriterLockSlim.IsWriteLockHeld)
                    readerWriterLockSlim.ExitWriteLock();
            }         
        }

        public static async Task SaveAsync<T>(string FileName, T Object)
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                using (TextWriter tw = new StreamWriter(FileName, true))
                {
                    var objectToFile = JsonConvert.SerializeObject(Object);
                    await tw.WriteLineAsync(objectToFile);
                }
            }
            finally
            {
                if(readerWriterLockSlim.IsWriteLockHeld)
                    readerWriterLockSlim.ExitWriteLock();
            }
        }

        public static async Task Save<T>(string FileName, T Object)
        {
            using (TextWriter tw = new StreamWriter(FileName, true))
            {
                var objectToFile = JsonConvert.SerializeObject(Object);
                await tw.WriteLineAsync(objectToFile);
            }
        }

        public static void Save(string FileName, string Content)
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                using (TextWriter tw = new StreamWriter(FileName, true))
                {
                    tw.WriteLine(Content);
                }
            }
            finally
            {
                if (readerWriterLockSlim.IsWriteLockHeld)
                    readerWriterLockSlim.ExitWriteLock();
            }            
        }
    }
}
