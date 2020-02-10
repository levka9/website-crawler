using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Logic
{
    public static class FileData
    {
        public static async Task Save<T>(string FileName, IEnumerable<T> List)
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
    }
}
