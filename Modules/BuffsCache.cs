using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    /// <summary>
    /// Keeps track of any buffs that are active
    /// </summary>
    public class BuffsCache : Module
    {
        public List<CachedBuff> ActiveBuffs = new List<CachedBuff>();
        public Dictionary<int, CachedBuff> BuffsById = new Dictionary<int, CachedBuff>();

        public bool HasBlessedShrine { get; private set; }
        public bool HasFrenzyShrine { get; private set; }
        public bool HasArchon { get; private set; }
        public bool HasInvulnerableShrine { get; private set; }
        public bool HasCastingShrine { get; set; }
        public bool HasConduitPylon { get; set; }
        public bool HasBastiansWillSpenderBuff { get; set; }
        public bool HasBastiansWillGeneratorBuff { get; set; }
        public bool HasConduitShrine { get; set; }
        public Element ConventionElement { get; set; }
        public int ConventionElementRemainingMs { get; set; }
        public int ConventionElementElapsedMs { get; set; }

        protected override int UpdateIntervalMs => 25;

        protected override void OnPulse()
        {
            UpdateBuffs();
        }

        public void UpdateBuffs()
        {
            if (!Core.Player.IsValid)
                return;

            Clear();

            foreach (var buff in ZetaDia.Me.GetAllBuffs())
            {
                if (!buff.IsValid)
                    return;

                var cachedBuff = new CachedBuff(buff);

                //Convention of Elements
                if (cachedBuff.Id == (int)SNOPower.P2_ItemPassive_Unique_Ring_038)
                {
                    ConventionElement = CachedBuff.GetElement(cachedBuff.BuffAttributeSlot);
                    ConventionElementElapsedMs = (int)cachedBuff.Elapsed.TotalMilliseconds;
                    ConventionElementRemainingMs = (int)cachedBuff.Remaining.TotalMilliseconds;
                }

                if (!BuffsById.ContainsKey(cachedBuff.Id))
                    BuffsById.Add(cachedBuff.Id, cachedBuff);

                ActiveBuffs.Add(cachedBuff);
            }

            HasArchon = HasBuff(SNOPower.Wizard_Archon); // .Any(p => GameData.ArchonSkillIds.Contains((int)p))

            // Bastians of Will
            HasBastiansWillSpenderBuff = HasBuff(SNOPower.ItemPassive_Unique_Ring_735_x1, 2);
            HasBastiansWillGeneratorBuff = HasBuff(SNOPower.ItemPassive_Unique_Ring_735_x1, 1);

            // Shrines
            HasBlessedShrine = HasBuff(30476); //Blessed (+25% defence)
            HasFrenzyShrine = HasBuff(30479); //Frenzy  (+25% atk speed)
            HasInvulnerableShrine = HasBuff(SNOPower.Pages_Buff_Invulnerable);
            HasCastingShrine = HasBuff(SNOPower.Pages_Buff_Infinite_Casting);
            HasConduitShrine = HasBuff(SNOPower.Pages_Buff_Electrified) || HasBuff(SNOPower.Pages_Buff_Electrified_TieredRift);

            //P4_ItemPassive_Unique_Ring_057:1:\:0 dlg.icon == walking endlessly 
            //P4_ItemPassive_Unique_Ring_057:2:\:0 dlg.icon == stopping for directions
            //P3_ItemPassive_Unique_Ring_026:1:\:1 dlg.icon == shenlongs spirit
        }

        public bool HasBuff(SNOPower power, int variantId)
        {
            return HasBuff((int)power, variantId);
        }

        public CachedBuff GetBuff(int id)
        {
            CachedBuff buff;
            return BuffsById.TryGetValue(id, out buff) ? buff : new CachedBuff();
        }

        public CachedBuff GetBuff(int id, int variantId)
        {
            return ActiveBuffs.FirstOrDefault(b => b.Id == id && b.BuffAttributeSlot == variantId);
        }

        public CachedBuff GetBuff(SNOPower id)
        {
            return GetBuff((int)id);
        }

        public bool HasBuff(int id, int variantId)
        {
            return GetBuff(id, variantId) != null;
        }

        public bool HasBuff(int id)
        {
            return BuffsById.ContainsKey(id);
        }

        public bool HasBuff(SNOPower id)
        {
            return HasBuff((int)id);
        }

        public int GetBuffStacks(int id, int variantId = -1)
        {
            if (variantId >= 0)
            {
                var buff = GetBuff(id, variantId);
                return buff?.StackCount ?? 0;
            }
            return GetBuff(id).StackCount;            
        }

        public int GetBuffStacks(SNOPower id, int variantId = -1)
        {
            return GetBuffStacks((int)id, variantId);
        }

        public void Clear()
        {
            ActiveBuffs.Clear();
            BuffsById.Clear();
        }

        public int GetBuffStacks(Skill skill)
        {
            return GetBuffStacks(skill.SNOPower);
        }

        public double GetBuffTimeRemainingMilliseconds(SNOPower power)
        {
            return Core.Cooldowns.GetBuffCooldownRemaining(power).TotalMilliseconds;
        }
    }
}
