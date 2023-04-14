using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebsiteCrawler.Model.Base;
using System.Text.Json.Serialization;
using WebsiteCrawler.Model.JsonConverters;
using Nest;

namespace WebsiteCrawler.Model.Responses;

[ElasticsearchType(RelationName = "page_data")]
public class PageDataParserModuleResponse : BaseResponse
{
    [Text(Name = "domain_name")]
    public string? DomainName { get; set; }

    // webpage charset
    [JsonConverter(typeof(EncodingConverter))]
    [Ignore]
    public Encoding? Encoding { get; set; }
    [Text(Name = "encoding")]
    public string EncodingText 
    { 
        get 
        {
            return Encoding.WebName;
        } 
    }
    [Text(Name = "title")]
    public string? Title { get; set; }
    [Text(Name = "description")]
    public string? Description { get; set; }
    [Text(Name = "adderess")]
    public string? Address { get; set; }
    [Text(Name = "keywords")]
    public List<string> Keywords { get; set; }
    [Text(Name = "content")]
    public string Content { get; set; }
    [Text(Name = "contact_page_url")]
    public string ContactPageUrl { get; set; }
    [Boolean(Name = "is_contact_page_parsed")]
    public bool IsContactPageParsed { get; set; }
    public List<string> Emails { get; set; }
    public List<string> Phones { get; set; }
    public List<string> Links { get; set; }

    public PageDataParserModuleResponse()
    {
        base.Id = Guid.NewGuid();
    }

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
                Address = Address,
                IsContactPageParsed = IsContactPageParsed
        }, 
            new JsonSerializerOptions() 
            {
                Encoder = JavaScriptEncoder.Create(new TextEncoderSettings(UnicodeRanges.All)),
                IgnoreNullValues = true 
            }
        );
    }
}
