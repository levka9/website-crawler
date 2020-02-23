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
                readerWriterLockSlim.ExitWriteLock();
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
                readerWriterLockSlim.ExitWriteLock();
            }            
        }
    }
}
