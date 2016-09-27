using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Trinity.Framework.Helpers;
using Trinity.Technicals;

namespace Trinity.Helpers
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
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, value);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch(Exception ex)
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

        public static T Deserialize<T>(string json, T instance) where T : class, new()
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    var newObj = serializer.ReadObject(stream) as T;
                    PropertyCopy.Copy(newObj, instance);
                    return instance;
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug($"JsonSerializer.Serialize Excpetion. Type={typeof(T)} {ex}");
                return null;
            }
        }


        //public static T DeserializeToType<T>(string json, object instance) where T : class, new()
        //{
        //    try
        //    {
        //        using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
        //        {
        //            var serializer = new DataContractJsonSerializer(typeof(T));
        //            var newObj = serializer.ReadObject(stream) as T;
        //            PropertyCopy.Copy(newObj, instance);
        //            return instance as T;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        //public static void DeserializeDervivedTo(string json, object instance)
        //{
        //    try
        //    {
        //        using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
        //        {
        //            var serializer = new DataContractJsonSerializer(instance.GetType());
        //            var newObj = serializer.ReadObject(stream);
        //            PropertyCopy.CopyDerived(newObj, instance);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

    }
}
