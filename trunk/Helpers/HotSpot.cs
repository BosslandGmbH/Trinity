using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Zeta.Common;

namespace Trinity
{
    [Serializable]
    public class HotSpot : IEquatable<HotSpot>
    {
        internal const float MaxPositionDistance = 5f;
        internal const float MaxPositionDistanceSqr = 25f;
        internal const float DefaultDuration = 2000;
        public Vector3 Location { get; set; }
        public DateTime ExpirationTime { get; set; }
        public int WorldId { get; set; }
        public HotSpot()
        {

        }
        public HotSpot(Vector3 location, DateTime expirationTime, int worldId)
        {
            Location = location;
            ExpirationTime = expirationTime;
            WorldId = worldId;
        }
        public HotSpot(Vector3 location, int worldId)
        {
            Location = location;
            WorldId = worldId;
            ExpirationTime = DateTime.UtcNow.AddMilliseconds(DefaultDuration);
        }
        public override int GetHashCode()
        {
            return ((int)Location.X ^ (int)Location.Y ^ WorldId);
        }
        public bool Equals(HotSpot other)
        {
            return this.Location.Distance2DSqr(other.Location) <= MaxPositionDistanceSqr && this.WorldId == other.WorldId;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public static bool operator ==(HotSpot left, HotSpot right)
        {
            return left.Location.Distance2DSqr(right.Location) <= MaxPositionDistanceSqr && left.WorldId == right.WorldId;
        }
        public static bool operator !=(HotSpot left, HotSpot right)
        {
            return left.Location.Distance2DSqr(right.Location) > MaxPositionDistanceSqr && left.WorldId != right.WorldId;
        }

        public override string ToString()
        {
            return string.Format("Location: {0} WorldId: {1} Expires: {2}", Location, WorldId, ExpirationTime.Subtract(DateTime.UtcNow));
        }

        public static string Serialize(HotSpot hotSpot)
        {
            XmlSerializer xs = new XmlSerializer(typeof(HotSpot));
            XmlWriterSettings xws = new XmlWriterSettings()
            {
                Indent = false,
                OmitXmlDeclaration = true,
                NewLineHandling = NewLineHandling.None,
            };
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xws))
                {
                    XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                    xmlSerializerNamespaces.Add("", "");
                    xs.Serialize(xmlWriter, hotSpot, xmlSerializerNamespaces);
                }
                return stringWriter.ToString();
            }
        }

        public static HotSpot Deserialize(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return default(HotSpot);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(HotSpot));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader stringReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                {
                    return (HotSpot)serializer.Deserialize(xmlReader);
                }
            }
        }

        public static string CurrentTargetHotSpot
        {
            get
            {
                try
                {
                    if (Trinity.CurrentTarget == null)
                        return null;
                    else if (Trinity.CurrentTarget.Type != TrinityObjectType.Unit)
                        return null;
                    else
                    {
                        return HotSpot.Serialize(new HotSpot(Trinity.CurrentTarget.Position, Trinity.Player.WorldID));
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool IsValid
        {
            get { return this.Location != Vector3.Zero && this.WorldId != 0; }
        }
    }
}
