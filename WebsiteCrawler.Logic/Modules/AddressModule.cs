using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCrawler.Logic.Modules
{
    public class AddressModule
    {
        private List<string> _addressContain;

        public AddressModule()
        {
            _addressContain = new List<string> 
            {
                "רח'",
                "כתובת",
                "עיר",
                "מיקוד"
            };
        }

        public string Find(string content)
        {
            // get address between address tags

            // find address by regex mesk


            return "";
        }
    }
}
