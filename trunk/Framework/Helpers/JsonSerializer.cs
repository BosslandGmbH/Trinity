using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Trinity.Framework.Helpers
{
    public static class JsonSerializer
    {
        public static string Serialize<T>(T value, bool ignoreTypes = true) where T : class
        {
            try
            {                
                var settings = new DataContractJsonSerializerSettings();
                if (ignoreTypes)
                {
                    settings.EmitTypeInformation = EmitTypeInformation.Never;
                }
                var serializer = new DataContractJsonSerializer(typeof(T), settings);
                string output;
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, value);
                    output = Encoding.UTF8.GetString(stream.ToArray());
                }
                //output = FormatJson(output);
                return output;
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"JsonSerializer.Serialize Excpetion. Type={typeof(T)} {ex}");
                return null;
            }
        }

        public static T Deserialize<T>(string json) where T : class
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    return serializer.ReadObject(stream) as T;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"JsonSerializer.Serialize Excpetion. Type={typeof(T)} {ex}");
                return null;
            }
        }

        public static void Deserialize<T>(string json, T instance) where T : class, new()
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    var newObj = serializer.ReadObject(stream) as T;
                    PropertyCopy.Copy(newObj, instance);
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"JsonSerializer.Serialize Excpetion. Type={typeof(T)} {ex}");
            }
        }

        static string FormatJson(string json)
        {
            int indent = 0, quote = 0;
            var result =
                from ch in json
                let quotes = ch == '"' ? quote++ : quote
                let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat("\t", indent)) : null
                let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat("\t", ++indent)) : ch.ToString()
                let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + string.Concat(Enumerable.Repeat("\t", --indent)) + ch : ch.ToString()
                select lineBreak ?? (openChar.Length > 1 ? openChar : closeChar);
            return string.Concat(result);
        }

    }
}
