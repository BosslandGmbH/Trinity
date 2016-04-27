using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Trinity.Technicals;

namespace Trinity.Config
{
    //http://stackoverflow.com/questions/11092274/datacontractserializer-change-namespace-and-deserialize-file-bound-to-old-name
    public class SettingsMigrator : XmlReader
    {
        public static string GilesTrinityNamespace { get { return "http://schemas.datacontract.org/2004/07/GilesTrinity.Settings"; } }

        private static HashSet<string> _typeNames;
        public static HashSet<string> TypeNames
        {
            get
            {
                if (_typeNames == null)
                {
                    _typeNames = new HashSet<string>((from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                                      from t in assembly.GetTypes()
                                                      select t.Name).Distinct());
                }
                return _typeNames;
            }
        }

        XmlReader inner;
        public SettingsMigrator(XmlReader inner)
        {
            this.inner = inner;
        }

        public override void ReadEndElement()
        {
            inner.ReadEndElement();
        }

        public override int AttributeCount
        {
            get { return inner.AttributeCount; }
        }

        public override string BaseURI
        {
            get { return inner.BaseURI; }
        }

        public override void Close()
        {
            inner.Close();
        }

        public override int Depth
        {
            get { return inner.Depth; }
        }

        public override bool EOF
        {
            get { return inner.EOF; }
        }

        public override string GetAttribute(int i)
        {
            return inner.GetAttribute(i);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return inner.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(string name)
        {
            return inner.GetAttribute(name);
        }

        public override bool IsEmptyElement
        {
            get { return inner.IsEmptyElement; }
        }

        public override string LocalName
        {
            get
            {
                var local = inner.LocalName;
                return local;
            }
        }

        public override string LookupNamespace(string prefix)
        {
            return inner.LookupNamespace(prefix);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return inner.MoveToAttribute(name, ns);
        }

        public override bool MoveToAttribute(string name)
        {
            return inner.MoveToAttribute(name);
        }

        public override bool MoveToElement()
        {
            var move = inner.MoveToElement();

            //if (inner.HasAttributes)
            //{
            //    var attrType = inner.GetAttribute("type", NamespaceURI);
            //    if (!string.IsNullOrEmpty(attrType))
            //    {
            //        //Logger.LogVerbose("Type Attr Found {0}", attrType);
            //       // inner.MoveToElement();
            //        //inner.Skip();
            //        //inner.ReadEndElement();
            //        //return string.Empty;
            //        return true;
            //    }
            //}

            return move;
        }

        public override bool MoveToFirstAttribute()
        {
            return inner.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            return inner.MoveToNextAttribute();
        }

        public override XmlNameTable NameTable
        {
            get { return inner.NameTable; }
        }

        public override string NamespaceURI
        {
            get
            {
                if (inner.NamespaceURI.StartsWith(GilesTrinityNamespace))
                {
                    return "";
                }
                return inner.NamespaceURI;
            }
        }

        public override XmlNodeType NodeType
        {
            get { return inner.NodeType; }
        }

        public override string Prefix
        {
            get { return inner.Prefix; }
        }

        //public override bool Read()
        //{
        //    var read = inner.Read();
        //    //Logger.Log("Read: {0}", read);
        //    return read;
        //}

        public override bool Read()
        {
            bool result;
            result = inner.Read();

            switch (inner.NodeType)
            {
                case XmlNodeType.Element:

                    if (inner.HasAttributes)
                    {
                        var attrTypeValue = inner.GetAttribute("i:type");
                        if (!string.IsNullOrEmpty(attrTypeValue) && !TypeNames.Contains(attrTypeValue))
                        {
                            Logger.Log("Found unknown class XML specified in settings: {0}. Skipping.", attrTypeValue);
                            inner.Skip();
                        }
                    }

                    break;

                default:
                    break;
            }
            return result;
        }

        public override bool ReadAttributeValue()
        {
            var attrVal = inner.ReadAttributeValue();
            //Logger.Log("ReadAttributeValue: {0}", attrVal);
            return attrVal;
        }

        public override ReadState ReadState
        {
            get { return inner.ReadState; }
        }

        public override void ResolveEntity()
        {
            inner.ResolveEntity();
        }

        public override string Value
        {
            get { return inner.Value; }
        }
    }
}
