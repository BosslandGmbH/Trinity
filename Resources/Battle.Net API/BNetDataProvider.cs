using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using Trinity.Helpers;
using Trinity.Technicals;

namespace Trinity.Resources.BNetAPI
{
    class BNetDataProvider
    {
        static BNetDataProvider()
        {
            _serializer = new JavaScriptSerializer();
            _serializer.RegisterConverters(new List<JavaScriptConverter> { new RawAttributeDictionaryResolver() });            
        }

        private static JavaScriptSerializer _serializer;

        internal static ApiItem GetItem(string slug)
        {
            ApiItem item = null;
            var url = "https://us.battle.net/api/d3/data/item/" + slug;

            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    Logger.Log("Requesting Data from: {0}", url);
                    var json = webClient.DownloadString(url);
                    item = _serializer.Deserialize<ApiItem>(json);
                }
            }
            catch (Exception ex)
            {
                DumpException(ex);
            }

            return item;
        }

        public class RawAttributeDictionaryResolver : JavaScriptConverter
        {

            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                return dictionary.ToDictionary(item => item.Key, item =>
                {
                    var d = (item.Value as Dictionary<string, object>);
                    var range = new ApiRange
                    {
                        Min = Convert.ToDouble(d["min"]),
                        Max = Convert.ToDouble(d["max"])
                    };
                    return range;
                });
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                throw new Exception("Deserialize Only");
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get
                {
                    return new List<Type>
                    {
                        typeof(Dictionary<string,ApiRange>)
                    };
                }
            }
        } 

        public static void DumpException(Exception ex)
        {
            Console.WriteLine("--------- Outer Exception Data ---------");
            WriteExceptionInfo(ex);
            ex = ex.InnerException;
            if (null != ex)
            {
                Console.WriteLine("--------- Inner Exception Data ---------");
                WriteExceptionInfo(ex.InnerException);
                ex = ex.InnerException;
            }
        }

        public static void WriteExceptionInfo(Exception ex)
        {
            Console.WriteLine("Message: {0}", ex.Message);
            Console.WriteLine("Exception Type: {0}", ex.GetType().FullName);
            Console.WriteLine("Source: {0}", ex.Source);
            Console.WriteLine("StrackTrace: {0}", ex.StackTrace);
            Console.WriteLine("TargetSite: {0}", ex.TargetSite);
        }
    }



}

