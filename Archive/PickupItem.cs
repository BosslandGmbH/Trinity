//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;

//namespace Trinity.Cache
//{
//    internal class PickupItem
//    {
//        public string Name { get; set; }
//        public string InternalName { get; set; }
//        public int Level { get; set; }
//        public ItemQuality Quality { get; set; }
//        public int BalanceID { get; set; }
//        public DBItemBaseType DBBaseType { get; set; }
//        public ItemType DBItemType { get; set; }
//        public TrinityItemBaseType TBaseType { get; set; }
//        public TrinityItemType TType { get; set; }
//        public bool IsOneHand { get; set; }
//        public bool IsTwoHand { get; set; }
//        public FollowerType ItemFollowerType { get; set; }
//        public int DynamicID { get; set; }
//        public Vector3 Position { get; set; }
//        public int ActorSNO { get; set; }
//        public int AcdId { get; set; }
//        public int RActorGUID { get; set; }
//        public ACDItem ACDItem { get { return ZetaDia.Actors.GetACDItemById(AcdId); } }
//        public bool IsUpgrade { get; set; }
//        public float UpgradeDamage { get; set; }
//        public float UpgradeToughness { get; set; }
//        public float UpgradeHealing { get; set; }
//        public int WorldId { get; set; }

//        public PickupItem() { }

//        public PickupItem(ACDItem item, TrinityItemBaseType trinityItemBaseType, TrinityItemType trinityItemType)
//        {
//            Name = item.Name;
//            InternalName = TrinityPlugin.NameNumberTrimRegex.Replace(item.InternalName, ""); ;
//            Level = item.Level;
//            Quality = item.ItemQualityLevel;
//            BalanceID = item.GameBalanceId;
//            DBBaseType = item.DBItemBaseType;
//            DBItemType = item.ItemType;
//            TBaseType = trinityItemBaseType;
//            TType = trinityItemType;
//            IsOneHand = item.IsOneHand;
//            IsTwoHand = item.IsTwoHand;
//            ItemFollowerType = item.FollowerSpecialType;
//            DynamicID = item.AnnId;
//            ActorSNO = item.ActorSnoId;
//            AcdId = item.AcdId;
//            WorldId = TrinityPlugin.Player.WorldSnoId;
//        }

//        public PickupItem(string name, string internalName, int level, ItemQuality quality, int balanceId, DBItemBaseType dbItemBaseType, 
//            ItemType dbItemType, bool isOneHand, bool isTwoHand, FollowerType followerType, int AcdId, int dynamicID = 0)
//        {
//            Name = name;
//            InternalName = TrinityPlugin.NameNumberTrimRegex.Replace(internalName, "");
//            Level = level;
//            Quality = quality;
//            BalanceID = balanceId;
//            DBBaseType = dbItemBaseType;
//            DBItemType = dbItemType;
//            IsOneHand = isOneHand;
//            IsTwoHand = isTwoHand;
//            ItemFollowerType = followerType;
//            AcdId = AcdId;
//            DynamicID = dynamicID;
//            WorldId = TrinityPlugin.Player.WorldSnoId;
//        }

//        public virtual bool Equals(PickupItem other)
//        {
//            return DynamicID == other.DynamicID || GetHashCode() == other.GetHashCode();
//        }

//        public override int GetHashCode()
//        {            
//            return
//                Position.GetHashCode() ^
//                ActorSNO.GetHashCode() ^
//                InternalName.GetHashCode() ^
//                WorldId.GetHashCode() ^
//                Quality.GetHashCode() ^
//                Level.GetHashCode();

        
//        }
//    }
//}
