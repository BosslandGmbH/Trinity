//using System;
//using System.Linq;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("HasNormalRiftKeys")]
//    public class HasNormalRiftKeysTag : BaseComplexProfileBehavior
//    {
//        protected override Composite CreateBehavior()
//        {
//            return
//             new Decorator(ret => !IsDone,
//                 new PrioritySelector(
//                     base.GetNodes().Select(b => b.Behavior).ToArray()
//                 )
//             );
//        }

//        private Func<ACDItem, bool> IsNormalRiftKeyFunc
//        {
//            get { return i => i.ItemType == ItemType.KeystoneFragment && i.TieredLootRunKeyLevel == -1; }
//        }

//        public override bool GetConditionExec()
//        {
//            bool backpack = InventoryManager.Backpack.Any(IsNormalRiftKeyFunc);
//            bool stash = InventoryManager.StashItems.Any(IsNormalRiftKeyFunc);

//            return backpack || stash;
//        }
//    }
//}
