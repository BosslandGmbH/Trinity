using System;
using System.Globalization;
using System.Linq;
using Zeta.Game.Internals.Actors;

namespace Trinity.Cache
{
    public class CacheUIObject : IEquatable<CacheUIObject>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((CacheUIObject) obj);
        }

        public int Distance { get; set; }
        public int Radius { get; set; }
        public int RadiusDistance { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string InCache { get; set; }
        public string IgnoreReason { get; set; }
        public string Weight { get; set; }
        public string IsBoss { get; set; }
        public string IsElite { get; set; }
        public string IsQuestMonster { get; set; }
        public string IsMinimapActive { get; set; }
        public string MarkerHash { get; set; }
        public string MinimapTexture { get; set; }
        public string WeightInfo { get; set; }
        public int RActorGUID { get; set; }
        public int ActorSNO { get; set; }

        public CacheUIObject(DiaObject obj)
        {
            try
            {
                RActorGUID = obj.RActorId;
                ACDGuid = obj.ACDId;
                ActorSNO = obj.ActorSnoId;
                Distance = (int)obj.Distance;
                Radius = (int)obj.CollisionSphere.Radius;
                RadiusDistance = Distance - Radius;
                Name = CleanName(obj.Name);

                var marker = CacheData.CachedMarkers.FirstOrDefault(m => m.Position.Distance(obj.Position) < 1f);
                if (marker != null)
                {
                    MarkerHash = IntToString(marker.NameHash);
                    MinimapTexture = IntToString(marker.MinimapTextureSnoId);
                }

                try 
                {
                    if (obj is DiaUnit)
                    {
                        IsQuestMonster = BoolToString((obj as DiaUnit).IsQuestMonster);
                        IsMinimapActive = BoolToString(obj.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive) > 0);
                    }
                }
                catch {}

                var cacheObject = TrinityPlugin.ObjectCache.FirstOrDefault(o => o.RActorGuid == RActorGUID);
                if (cacheObject == null)
                {
                    Type = obj.ActorType.ToString();
                    InCache = "";
                    return;
                }
                InCache = "True";
                Type = cacheObject.Type.ToString();
                Weight = IntToString((int)cacheObject.Weight);
                IsBoss = BoolToString(cacheObject.IsBoss);
                IsElite = BoolToString(cacheObject.IsEliteRareUnique);
                WeightInfo = cacheObject.WeightInfo;

                string ignoreReason;
                CacheData.IgnoreReasons.TryGetValue(RActorGUID, out ignoreReason);
                IgnoreReason = ignoreReason;

            }
            catch (Exception ex)
            {
                WeightInfo = ex.Message;
            }
        }

        private string BoolToString(bool val)
        {
            if (val)
                return "True";
            return "";
        }

        private string IntToString(int val)
        {
            if (val != 0)
                return val.ToString(CultureInfo.InvariantCulture);
            return "";
        }

        private string CleanName(string name)
        {
            return name.Split('-')[0];
        }

        public bool Equals(CacheUIObject other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Distance == other.Distance && Radius == other.Radius && RadiusDistance == other.RadiusDistance && string.Equals(Name, other.Name) && string.Equals(Type, other.Type) && string.Equals(InCache, other.InCache) && string.Equals(IgnoreReason, other.IgnoreReason) && string.Equals(Weight, other.Weight) && string.Equals(IsBoss, other.IsBoss) && string.Equals(IsElite, other.IsElite) && string.Equals(IsQuestMonster, other.IsQuestMonster) && string.Equals(IsMinimapActive, other.IsMinimapActive) && string.Equals(MarkerHash, other.MarkerHash) && string.Equals(MinimapTexture, other.MinimapTexture) && string.Equals(WeightInfo, other.WeightInfo) && RActorGUID == other.RActorGUID && ActorSNO == other.ActorSNO && ACDGuid == other.ACDGuid;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Distance;
                hashCode = (hashCode*397) ^ Radius;
                hashCode = (hashCode*397) ^ RadiusDistance;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (InCache != null ? InCache.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IgnoreReason != null ? IgnoreReason.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Weight != null ? Weight.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IsBoss != null ? IsBoss.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IsElite != null ? IsElite.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IsQuestMonster != null ? IsQuestMonster.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IsMinimapActive != null ? IsMinimapActive.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MarkerHash != null ? MarkerHash.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MinimapTexture != null ? MinimapTexture.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (WeightInfo != null ? WeightInfo.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ RActorGUID;
                hashCode = (hashCode*397) ^ ActorSNO;
                hashCode = (hashCode*397) ^ ACDGuid;
                return hashCode;
            }
        }
        public static bool operator ==(CacheUIObject a, CacheUIObject b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.Equals(b);
        }
        public static bool operator !=(CacheUIObject a, CacheUIObject b)
        {
            return !(a == b);
        }

        public int ACDGuid { get; set; }
    }
}
