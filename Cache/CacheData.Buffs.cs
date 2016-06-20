using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Objects;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public class BuffsCache
    {
        public List<CachedBuff> ActiveBuffs = new List<CachedBuff>();
        public Dictionary<int, CachedBuff> BuffsById = new Dictionary<int, CachedBuff>();
        public DateTime LastUpdated = DateTime.MinValue;

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

        public void Update()
        {
            UpdateBuffsCache();
        }

        public void UpdateBuffsCache()
        {
            if (!CacheData.Player.IsValid)
                return;

            using (new PerformanceLogger("UpdateCachedBuffsData"))
            {
                if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < 100)
                    return;

                Clear();

                foreach (var buff in ZetaDia.Me.GetAllBuffs())
                {
                    if (!buff.IsValid)
                        return;

                    var cachedBuff = new CachedBuff(buff);

                    //Convention of Elements
                    if (cachedBuff.Id == (int) SNOPower.P2_ItemPassive_Unique_Ring_038)
                    {
                        ConventionElement = CachedBuff.GetElement(cachedBuff.BuffAttributeSlot);
                        ConventionElementElapsedMs = (int) cachedBuff.Elapsed.TotalMilliseconds;
                        ConventionElementRemainingMs = (int) cachedBuff.Remaining.TotalMilliseconds;
                    }

                    if (!BuffsById.ContainsKey(cachedBuff.Id))
                        BuffsById.Add(cachedBuff.Id, cachedBuff);

                    ActiveBuffs.Add(cachedBuff);

                    Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                        "ActiveBuffs: Id={0} Name={1} Stacks={2} VariantId={3} VariantName={4}", cachedBuff.Id, cachedBuff.InternalName, cachedBuff.StackCount, cachedBuff.BuffAttributeSlot, cachedBuff.VariantName);
                }

                LastUpdated = DateTime.UtcNow;

                //Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                //    "Refreshed Buffs: ActiveBuffs={0}", ActiveBuffs.Count);
            }

            HasArchon = HasBuff(SNOPower.Wizard_Archon); // .Any(p => DataDictionary.ArchonSkillIds.Contains((int)p))

            // Bastians of Will
            HasBastiansWillSpenderBuff = HasBuff(SNOPower.ItemPassive_Unique_Ring_735_x1, 2);
            HasBastiansWillGeneratorBuff = HasBuff(SNOPower.ItemPassive_Unique_Ring_735_x1, 1);

            // Shrines
            HasBlessedShrine = HasBuff(30476); //Blessed (+25% defence)
            HasFrenzyShrine = HasBuff(30479); //Frenzy  (+25% atk speed)
            HasInvulnerableShrine = HasBuff(SNOPower.Pages_Buff_Invulnerable);
            HasCastingShrine = HasBuff(SNOPower.Pages_Buff_Infinite_Casting);
            HasConduitShrine = HasBuff(SNOPower.Pages_Buff_Electrified) || HasBuff(SNOPower.Pages_Buff_Electrified_TieredRift);
        }

        public bool HasBuff(SNOPower power, int variantId)
        {
            return HasBuff((int) power, variantId);
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
            return GetBuff((int) id);
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
            return HasBuff((int) id);
        }

        public int GetBuffStacks(int id)
        {
            return GetBuff(id).StackCount;
        }

        public int GetBuffStacks(SNOPower id)
        {
            return GetBuffStacks((int) id);
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