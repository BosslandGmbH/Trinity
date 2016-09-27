//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Demonbuddy.Routines.Generic;
//using Trinity.Framework.Objects.Memory.Misc;
//using Trinity.Helpers;
//using Trinity.Reference;
//using Zeta.Game;

//namespace Trinity.Objects
//{
//    public class SkillViewModel : NotifyBase
//    {
//        public SkillViewModel()
//        {
//            Items = new FullyObservableCollection<SkillSettings>();            
//        }

//        private ActorClass _actorClass;
//        private string _name;
//        private string _description;
//        private string _author;
//        private string _version;
//        private FullyObservableCollection<SkillSettings> _items;

//        public ActorClass ActorClass
//        {
//            get { return _actorClass; }
//            set { SetField(ref _actorClass, value); }
//        }

//        public string Name
//        {
//            get { return _name; }
//            set { SetField(ref _name, value); }
//        }

//        public string Description
//        {
//            get { return _description; }
//            set { SetField(ref _description, value); }
//        }

//        public string Author
//        {
//            get { return _author; }
//            set { SetField(ref _author, value); }
//        }

//        public string Version
//        {
//            get { return _version; }
//            set { SetField(ref _version, value); }
//        }

//        public override void LoadDefaults()
//        {
//            var items = new FullyObservableCollection<SkillSettings>();
//            foreach (var skill in SkillUtils.ByActorClass(ActorClass))
//            {
//                items.Add(skill.GetDefaultSetting());
//            }
//            Items = items;
//        }

//        public FullyObservableCollection<SkillSettings> Items
//        {
//            get { return _items; }
//            set { SetField(ref _items, value); }
//        }        
//    }


//}




