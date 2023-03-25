using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebsiteCrawler.Model.JsonConverters
{
    public class EncodingConverter : JsonConverter<Encoding>
    {
        public override Encoding? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Encoding.GetEncoding(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options)
        {
            var encoding = (Encoding)value;
            var jsonValue = encoding?.WebName;

            writer.WriteStringValue(jsonValue);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Encoding);
        }

    }
}
