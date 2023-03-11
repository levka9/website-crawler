using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace WebsiteCrawler.Model.Responses;

public class PageDataParserResponse
{
    public string DomainName { get; set; }
        // webpage charset
    public Encoding Encoding { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Keywords { get; set; }
    public string Content { get; set; }
    public List<string> Emails { get; set; }
    public List<string> Phones { get; set; }
    public List<string> Links { get; set; }
    public string Address { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(new 
            {
                DomainName = DomainName,
                Encoding = Encoding != null ? Encoding.WebName : string.Empty,
                Title = Title,
                Description = Description,
                Keywords = Keywords,
                Content = Content,
                Emails = Emails,
                Phones = Phones,
                Links = Links,
                Address = Address
            }, 
            new JsonSerializerOptions() 
            {
                Encoder = JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All)),
                IgnoreNullValues = true 
            }
        );
    }
}
