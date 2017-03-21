//using Trinity.Framework;
//using Zeta.Bot.Profile;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    /// <summary>
//    /// Trinity Combat Ignore will let users add a SNO, and optionally specify to exclude elites or trash. This will be reset on every profile load.
//    /// </summary>
//    [XmlElement("TrinityCombatIgnore")]
//    class TrinityCombatIgnoreTag : ProfileBehavior
//    {
//        private bool _isDone;
//        public override bool IsDone
//        {
//            get { return _isDone; }
//        }

//        public override void OnStart()
//        {
//            Core.Logger.Error("TrinityCombatIgnore is no longer supported.");
//            _isDone = true;
//            base.OnStart();
//        }

//        public TrinityCombatIgnoreTag() { }

//        /*
//        [XmlAttribute("actorId")]
//        [XmlAttribute("actorSNO")]
//        [XmlAttribute("actorSno")]
//        public int ActorSnoId { get; set; }

//        [XmlAttribute("name")]
//        public string Name { get; set; }

//        [XmlAttribute("exceptElites")]
//        public bool ExceptElites { get; set; }

//        [XmlAttribute("exceptTrash")]
//        public bool ExceptTrash { get; set; }

//        protected override Composite CreateBehavior()
//        {
//            return new Action(ret => AddToIgnoreList());
//        }

//        private void AddToIgnoreList()
//        {
//            if (ActorSnoId > 0)
//            {
//                if (IgnoreList.Any(u => u.ActorSnoId == ActorSnoId))
//                    IgnoreList.RemoveWhere(u => u.ActorSnoId == ActorSnoId);

//                IgnoreList.Add(new CombatIgnoreUnit
//                {
//                    ActorSnoId = ActorSnoId,
//                    Name = Name,
//                    ExceptElites = ExceptElites,
//                    ExceptTrash = ExceptTrash
//                });

//                if (!string.IsNullOrWhiteSpace(Name))
//                    Technicals.Core.Logger.Log("Added {0} ({1}) to Combat Ignore List", Name, ActorSnoId);
//                else
//                    Technicals.Core.Logger.Log("Added {0} to Combat Ignore List", ActorSnoId);

//            }
//            else
//            {
//                Technicals.Core.Logger.Log("Unable to add to Trinity Combat Ignore List", ActorSnoId);
//            }

//            _isDone = true;
//        }

//        public static HashSet<CombatIgnoreUnit> IgnoreList = new HashSet<CombatIgnoreUnit>();

//        public class CombatIgnoreUnit
//        {
//            public int ActorSnoId { get; set; }
//            public string Name { get; set; }

//            /// <summary>
//            /// When set to True, should attack elite unit
//            /// </summary>
//            public bool ExceptElites { get; set; }

//            /// <summary>
//            /// When set to True, should attack non-elite unit
//            /// </summary>
//            public bool ExceptTrash { get; set; }

//            public override string ToString()
//            {
//                return string.Format(
//                    "ActorSnoId={0} Name={1} ExceptElites={2} ExceptTrash={3}",
//                    ActorSnoId, Name, ExceptElites, ExceptTrash);
//            }

//            public bool AttackElite(TrinityCacheObject tco)
//            {
//                if (ExceptElites && tco.IsEliteRareUnique)
//                    return true;

//                return false;
//            }

//            public bool AttackTrash(TrinityCacheObject tco)
//            {
//                if (ExceptTrash && tco.IsTrashMob)
//                    return true;

//                return false;
//            }

//            public bool ShouldAttack(TrinityCacheObject tco)
//            {
//                return AttackElite(tco) || AttackTrash(tco);
//            }
//        }
//        */
//    }
//}
