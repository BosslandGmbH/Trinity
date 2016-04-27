using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Objects
{
    public class Build
    {
        public string Name { get; set; }
        public Dictionary<Skill, Rune> Skills { get; set; }
        public List<Passive> Passives { get; set; }
        public List<Item> Items { get; set; }
        public int Level { get; set; }
    }
}
