//using System;
//using System.Linq;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("HasTrialRiftKeys")]
//    public class HasTrialRiftKeysTag : BaseComplexProfileBehavior
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

//        private Func<ACDItem, bool> IsTrialRiftKeyFunc
//        {
//            get { return i => i.ItemType == ItemType.KeystoneFragment && i.TieredLootRunKeyLevel == 0; }
//        }

//        public override bool GetConditionExec()
//        {
//            bool backpack = InventoryManager.Backpack.Any(IsTrialRiftKeyFunc);
//            bool stash = InventoryManager.StashItems.Any(IsTrialRiftKeyFunc);

//            return backpack || stash;
//        }
//    }
//}
