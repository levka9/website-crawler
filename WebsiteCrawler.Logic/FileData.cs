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

        public static async Task SaveAsync(string FileName, string content, string separatorSymbol = "")
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                using (TextWriter tw = new StreamWriter(new FileStream(FileName, FileMode.Append), Encoding.UTF8))
                {
                    await tw.WriteLineAsync($"{content}{separatorSymbol}");
                }
            }
            finally
            {
                if(readerWriterLockSlim.IsWriteLockHeld)
                    readerWriterLockSlim.ExitWriteLock();
            }
        }

        public static async Task SerializeAndSaveAsync<T>(string FileName, T Object, string separatorSymbol = "")
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                using (TextWriter tw = new StreamWriter(new FileStream(FileName, FileMode.Append), Encoding.UTF8))
                {
                    var objectToFile = JsonConvert.SerializeObject(Object);
                    await tw.WriteLineAsync($"{objectToFile}{separatorSymbol}");
                }
            }
            finally
            {
                if(readerWriterLockSlim.IsWriteLockHeld)
                    readerWriterLockSlim.ExitWriteLock();
            }
        }

        public static void Save(string FileName, string Content)
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                using (TextWriter tw = new StreamWriter(new FileStream(FileName, FileMode.Append), Encoding.UTF8))
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
