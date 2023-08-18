using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Data.Models
{
    public class ElasticsearchRequest
    {
        public int From { get; set; }
        public int Size { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
