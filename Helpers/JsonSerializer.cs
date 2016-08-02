using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Trinity.Helpers
{
    public static class JsonSerializer
    {
        public static string Serialize<T>(T value) where T : class
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, value);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch
            {
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
            catch
            {
                return null;
            }
        }
    }
}
