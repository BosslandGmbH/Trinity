using System;
namespace Trinity.Framework.Objects.Memory
{
    public class AttributeDescripter : MemoryWrapper
    {
        public int Modifier;
        public int Key;
        public int Id;
        public int DefaultValue;
        public AttributeParameterType ParameterType;
        public Type DataType;
        public string Name;
        public string FormulaA;
        public string FormulaB;
        public bool IsInteger;

        protected override void OnUpdated()
        {
            Id = ReadOffset<int>(0x00);
            Modifier = -1;
            DefaultValue = ReadOffset<int>(0x04);
            ParameterType = ReadOffset<AttributeParameterType>(0x08);
            IsInteger = ReadOffset<int>(0x10) == 1;
            DataType = IsInteger ? typeof(int) : typeof(float);
            Key = (Modifier << 12) + (Id & 0xFFF);
            FormulaA = ReadStringPointer(0x14);
            FormulaB = ReadStringPointer(0x18);
            Name = ReadStringPointer(0x1C).Replace("_", string.Empty);
        }
    }

    public enum AttributeParameterType : int
    {
        None = -1,
        RequirementType = 1, // ?? ModifierType=1 Modifier=57
        DamageType = 0, // SymbolTable @ 0x01802898
        PowerSnoId = 4,
        PowerSnoId2 = 11,
        ResourceType = 10, // SymbolTable @ 0x01802FB8
        TreasureClassSnoId = 14,
        ActorSnoId = 15,
        ConversationSnoId = 16,
        Set = 17, // ?? related to set item powers
        ActorType = 18, // SymbolTable @ 0x01802778
        InventoryLocation = 19, // SymbolTable @ 0x01803BF0
    }

    public enum RequirementType
    {
        None = -1,
        EquipItem = 57,
    }
}