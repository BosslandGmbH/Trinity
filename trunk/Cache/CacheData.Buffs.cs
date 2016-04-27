using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trinity.Cache;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Utilities;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Profile.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity
{
    public partial class CacheData
    {
        /// <summary>
        /// Fast Inventory Cache, Self-Updating, Use instead of ZetaDia.Inventory
        /// </summary>
        public class BuffsCache
        {
            static BuffsCache()
            {                
                Pulsator.OnPulse += (sender, args) => Instance.UpdateBuffsCache();
            }

            public BuffsCache()
            {
                UpdateBuffsCache();                
            }

            private static BuffsCache _instance = null;
            public static BuffsCache Instance
            {
                get { return BuffsCache._instance ?? (BuffsCache._instance = new BuffsCache()); }
                set { BuffsCache._instance = value; }
            }

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

            public DateTime LastUpdated = DateTime.MinValue;

            public Dictionary<int, CachedBuff> BuffsById = new Dictionary<int, CachedBuff>();

            public List<CachedBuff> AllBuffs = new List<CachedBuff>();

            public void UpdateBuffsCache()
            {
                if (!Player.IsValid)
                    return;

                using (new PerformanceLogger("UpdateCachedBuffsData"))
                {
                    if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < 50)
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

                        AllBuffs.Add(cachedBuff);                        

                        Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                            "ActiveBuffs: Id={0} Name={1} Stacks={2} VariantId={3} VariantName={4}", cachedBuff.Id, cachedBuff.InternalName, cachedBuff.StackCount, cachedBuff.BuffAttributeSlot, cachedBuff.VariantName);
                    }

                    LastUpdated = DateTime.UtcNow;

                    Logger.Log(TrinityLogLevel.Debug, LogCategory.CacheManagement,
                        "Refreshed Inventory: ActiveBuffs={0}", ActiveBuffs.Count);
                }

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

            public int ConventionElementRemainingMs { get; set; }

            public int ConventionElementElapsedMs { get; set; }

            public bool HasBuff(SNOPower power, int variantId)
            {
                return HasBuff((int) power, variantId);
            }

            public List<CachedBuff> ActiveBuffs
            {
                get { return AllBuffs; }
            }

            public CachedBuff GetBuff(int id)
            {                
                CachedBuff buff;
                return BuffsById.TryGetValue(id, out buff) ? buff : new CachedBuff();
            }

            public CachedBuff GetBuff(int id, int variantId)
            {
                return AllBuffs.FirstOrDefault(b => b.Id == id && b.BuffAttributeSlot == variantId);;
            }

            public CachedBuff GetBuff(SNOPower id)
            {
                return GetBuff((int)id);
            }

            public bool HasBuff(int id, int variantId)
            {
                return GetBuff(id,variantId) != null;
            }

            public bool HasBuff(int id)
            {
                return BuffsById.ContainsKey(id);
            }

            public bool HasBuff(SNOPower id)
            {
                return HasBuff((int)id);
            }

            public int GetBuffStacks(int id)
            {
                return GetBuff(id).StackCount;
            }

            public int GetBuffStacks(SNOPower id)
            {
                return GetBuffStacks((int)id);
            }

            //public void Dump()
            //{
            //    using (new MemoryHelper())
            //    {
            //        foreach (var hotbarskill in ActiveBuffs)
            //        {
            //            Logger.Log("Id={0} InternalName={1} Cancellable={2} StackCount={3}",
            //                hotbarskill.Id,
            //                hotbarskill.InternalName,
            //                hotbarskill.IsCancellable,
            //                hotbarskill.StackCount
            //            );
            //        }
            //    }
            //}

            public void Clear()
            {
                AllBuffs.Clear();
                BuffsById.Clear();
            }

            ///// <summary>
            ///// Finds the code that differentiates the state of a buff icon or animation 
            ///// Root.NormalLayer.buffs_backgroundScreen.buff P2_ItemPassive_Unique_Ring_038:1:2018967705:0 dlg.icon
            ///// Root.NormalLayer.buffs_backgroundScreen.buff P2_ItemPassive_Unique_Ring_038:2:2018967705:0 dlg.icon
            ///// </summary>
            //public int GetBuffVariantId(SNOPower snoPower)
            //{
            //    var elements = GetBuffUIElements(snoPower);
            //    if (!elements.Any())
            //        return 0;

            //    foreach (var element in elements)
            //    {
            //        var buffCode = element.Name.Split(' ').ElementAtOrDefault(1);
            //        if (buffCode == null)
            //            continue;

            //        var variantIdPart = buffCode.Split(':').ElementAtOrDefault(1);
            //        if (variantIdPart == null)
            //            continue;

            //        int variantId;
            //        if(!Int32.TryParse(variantIdPart, out variantId))
            //            continue;

            //        // Skip Duplicate Elements (Bastians)
            //        if (!HasBuff((int) snoPower, variantId))
            //            return variantId;
            //    }

            //    return 0;
            //}

            //private PerFrameCachedValue<UIElement> _buffIconWrapper;
            //public UIElement BuffIconWrapper
            //{
            //    get { return _buffIconWrapper ?? (_buffIconWrapper = new PerFrameCachedValue<UIElement>(() => UIElement.FromName("Root.NormalLayer.buffs_backgroundScreen.buff_icon_wrapper"))); }                
            //}

            ///// <summary>
            ///// Finds the buff UIElement for a SnoPower
            ///// </summary>
            //public List<UIElement> GetBuffUIElements(SNOPower snoPower)
            //{
            //    //		Name	"Root.NormalLayer.buffs_backgroundScreen.buff_icon_wrapper"	string
            //    //var container = UIElement.FromName("Root.NormalLayer.buffs_backgroundScreen");
            //    var elements = UIElement.GetChildren(BuffIconWrapper);
            //    return elements.Where(element => element.Name.Contains(snoPower.ToString())).ToList();
            //}



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
}

