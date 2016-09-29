using System;
using Trinity.Framework.Helpers;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Objects
{
    public class CachedBuff
    {
        public CachedBuff() { }
        private readonly Buff _buff;

        public CachedBuff(Buff buff)
        {
            if (buff == null || !buff.IsValid)
                return;

            try
            {
                BuffAttributeSlot = ZetaDia.Memory.Read<int>(buff.BaseAddress + 4);
            }
            catch (Exception ex)
            {
                Logger.LogVerbose("Exception finding buff VariantId {0}", ex);
            }

            _buff = buff;
            Id = buff.SNOId;
            SNOPower = (SNOPower) Id;
            FrameDuration = buff.FrameDuration;                        
            InternalName = buff.InternalName;            
            VariantName = GetBuffVariantName(Id, BuffAttributeSlot);
            IsCancellable = buff.IsCancelable;
            StackCount = buff.StackCount;
            GetElement(BuffAttributeSlot);
        }

        public SNOPower SNOPower { get; set; }        
        public string InternalName { get; set; }
        public bool IsCancellable { get; set; }
        public int StackCount { get; set; }
        public int Id { get; set; }
        public int BuffAttributeSlot { get; set; }
        public string VariantName { get; set; }
        public int FrameDuration { get; set; }

        public TimeSpan Elapsed
        {
            get { return Core.Cooldowns.GetBuffCooldownElapsed(SNOPower); }
        }

        public TimeSpan Remaining
        {
            get { return Core.Cooldowns.GetBuffCooldownRemaining(SNOPower); }
        }

        public void Cancel()
        {
            if (IsCancellable && _buff.IsValid)
                _buff.Cancel();
        }

        public override string ToString()
        {
            return ToStringReflector.GetObjectString(this);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        /// <summary>
        /// Converts a variantId into an element.
        /// </summary>
        public static Element GetElement(int id)
        {
            if (id > 7) id -= 7;
            if (id < 1) id += 7;

            switch (id)
            {
                case 1:
                    return Element.Arcane;
                case 2:
                    return Element.Cold;
                case 3:
                    return Element.Fire;
                case 4:
                    return Element.Holy;
                case 5:
                    return Element.Lightning;
                case 6:
                    return Element.Physical;
                case 7:
                    return Element.Poison;
                default:
                    return Element.Unknown;
            }
        }

        /// <summary>
        /// Converts an element into a variantId.
        /// </summary>
        public static int GetElementId(Element element)
        {
            switch (element)
            {
                case Element.Arcane:
                    return 1;
                case Element.Cold:
                    return 2;
                case Element.Fire:
                    return 3;
                case Element.Holy:
                    return 4;
                case Element.Lightning:
                    return 5;
                case Element.Physical:
                    return 6;
                case Element.Poison:
                    return 7;
                default:
                    return 0;
            }
        }

        public string GetBuffVariantName(int id, int variantId)
        {
            switch (id)
            {
                case (int)SNOPower.ItemPassive_Unique_Ring_735_x1: // Bastians of Will
                    return ((ResourceEffectType)variantId).ToString();

                case (int)SNOPower.P2_ItemPassive_Unique_Ring_038: // Convention of Elements
                    return (GetElement(variantId).ToString());
            }
            return string.Empty;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

    }
}
