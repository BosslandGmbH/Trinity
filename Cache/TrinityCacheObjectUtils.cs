using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache
{
    /// <summary>
    /// Where methods go to die.
    /// </summary>
    public static class TrinityCacheObjectUtils
    {
        public static bool HasDebuff(this TrinityCacheObject obj, SNOPower debuffSNO)
        {
            if (obj?.CommonData == null || !obj.IsValid)
                return false;

            //todo this is needlessly slow checking every possible attribute, trace the spells being used and record only the required slots

            try
            {
                //These are the debuffs we've seen so far
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffect & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff5VisualEffectC & 0xFFF)) == 1)
                    return true;

                //These are here just in case
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (obj.CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectD & 0xFFF)) == 1)
                    return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

    }
}
