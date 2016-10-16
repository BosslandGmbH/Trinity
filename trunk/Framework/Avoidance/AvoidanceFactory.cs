using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Avoidance
{
    /// <summary>
    /// Creates Avoidance objects from TrinityActors
    /// </summary>
    public class AvoidanceFactory
    {
        internal static List<AvoidanceDefinition> AvoidanceData = new List<AvoidanceDefinition>();

        internal static ILookup<SNOAnim, AvoidancePart> LookupPartByAnimation { get; set; }

        public static readonly Dictionary<int, AvoidancePart> AvoidanceDataDictionary = new Dictionary<int, AvoidancePart>();

        public static Dictionary<AvoidanceType, AvoidanceDefinition> AvoidanceDataByType { get; set; }

        public static bool TryCreateAvoidance(List<TrinityActor> actors, TrinityActor actor, out Structures.Avoidance avoidance)
        {
            avoidance = null;

            var data = GetAvoidanceData(actor);
            if (data == null)
                return false;

            avoidance = new Structures.Avoidance
            {
                Definition = data,
                CreationTime = DateTime.UtcNow,
                StartPosition = actor.Position,
                Settings = Core.Avoidance.Settings.GetDefinitionSettings(data.Type),
                Actors = new List<TrinityActor> { actor },
                IsImmune = Core.Player.ElementImmunity.Contains(data.Element)
            };

            return true;
        }

        static AvoidanceFactory()
        {
            CreateData();
            CreateUtils();
        }

        private static void CreateData()
        {
            AvoidanceData = AvoidanceDefinitions.Items;
            AvoidanceDataByType = AvoidanceData.ToDictionary(k => k.Type, v => v);
        }

        private static void CreateUtils()
        {
            var allParts = new List<AvoidancePart>();
                                
            foreach (var avoidanceDatum in AvoidanceDefinitions.Items)
            {
                foreach (var part in avoidanceDatum.Parts)
                {
                    part.Parent = avoidanceDatum;
                    allParts.Add(part);                    
                    try
                    {
                        if (part.ActorSnoId > 0)
                        {
                            AvoidanceDataDictionary.Add(part.ActorSnoId, part);
                        }
                    }
                    catch(Exception ex)                   
                    {
                        Logger.LogError("Failed to add AvoidanceData for {0} > {1}. Probably a duplicate ActorSnoId ({2})", avoidanceDatum.Name, part.Name, part.ActorSnoId);
                    }
                }
            }

            LookupPartByAnimation = allParts.Where(o => o.Animation != default(SNOAnim)).ToLookup(k => k.Animation, v => v);
        }

        public static AvoidanceDefinition GetAvoidanceData(AvoidanceType type)
        {
            return AvoidanceDataByType.ContainsKey(type) ? AvoidanceDataByType[type] : null;
        }

        public static AvoidanceDefinition GetAvoidanceData(int id)
        {
            return GetAvoidanceData((AvoidanceType)id);
        }

        public static AvoidanceDefinition GetAvoidanceData(TrinityActor actor)
        {
            if (actor == null)
                return null;

            AvoidanceDefinition data;

            if (TryFindPartByActorId(actor, out data))
                return data;

            if (TryFindPartByAffix(actor, out data))
                return data;

            if (TryFindPartByAnimation(actor, out data))
                return data;

            return null;
        }

        private static bool TryFindPartByActorId(TrinityActor actor, out AvoidanceDefinition data)
        {
            data = null;

            if (actor == null || actor.ActorSnoId <= 0)
                return false;

            var part = GetAvoidancePart(actor.ActorSnoId);
            if (part != null && (part.Filter == null || part.Filter(actor)))
            {            
                data = part.Parent;
                return true;                
            }
            return false;
        }

        private static bool TryFindPartByAffix(TrinityActor actor, out AvoidanceDefinition data)
        {
            data = null;

            if (actor.MonsterAffixes.HasFlag(MonsterAffixes.None))
                return false;

            var part = GetAvoidancePart(actor.MonsterAffixes);
            if (part != null)
            {
                data = part.Parent;
                return true;
            }        
            return false;
        }

        private static bool TryFindPartByAnimation(TrinityActor actor, out AvoidanceDefinition data)
        {
            data = null;

            if (actor.Animation == default(SNOAnim))
                return false;

            var part = GetAvoidancePart(actor.Animation);
            if (part != null)
            {
                data = part.Parent;
                return true;
            }
            return false;
        }

        public static AvoidancePart GetAvoidancePart(int actorId)
        {
            return AvoidanceDataDictionary.ContainsKey(actorId) ? AvoidanceDataDictionary[actorId] : null;
        }

        public static AvoidancePart GetAvoidancePart(MonsterAffixes affixes)
        {
            return AvoidanceDataDictionary.Values.FirstOrDefault(a => affixes.HasAny(a.Affix));
        }

        public static AvoidancePart GetAvoidancePart(SNOAnim actorAnimation)
        {
            return LookupPartByAnimation.Contains(actorAnimation) ? LookupPartByAnimation[actorAnimation].FirstOrDefault() : null;
        }
    }
}


