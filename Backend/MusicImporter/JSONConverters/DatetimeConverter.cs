using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusicImporter.JSONConverters
{
    internal class DatetimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.GetString() == null)
            {
                throw new Exception("No string found to convert to JSON");
            }

            if (string.IsNullOrEmpty(reader.GetString()))
            {
                return DateTime.Now;
            }

            try
            {
                return DateTime.Parse(reader.GetString());
            }
            catch (Exception)
            {
                return new DateTime(Convert.ToInt32(reader.GetString()), 1, 1);
            }
            
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
