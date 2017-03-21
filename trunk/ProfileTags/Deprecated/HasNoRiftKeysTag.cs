//using System;
//using System.Linq;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.Components.QuestTools.ProfileTags.Depreciated
//{
//    [XmlElement("HasNoRiftKeys")]
//    public class HasNoRiftKeysTag : BaseComplexProfileBehavior
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

//        private Func<ACDItem, bool> IsRiftKeyFunc
//        {
//            get { return i => i.ItemType == ItemType.KeystoneFragment; }
//        }

//        public override bool GetConditionExec()
//        {
//            bool backpack = InventoryManager.Backpack.Any(IsRiftKeyFunc);
//            bool stash = InventoryManager.StashItems.Any(IsRiftKeyFunc);

//            return !backpack && !stash;
//        }
//    }
//}
