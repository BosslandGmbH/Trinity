using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Framework.Objects.Memory.Sno;
using Trinity.Framework.Objects.Memory.Symbols;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Objects.Memory.Debug
{
    public enum DataTypeGroup
    {
        None = 0,
        Null,
        Simple,
        Object,
        Sno,
        VariableArray,
        FixedArray,
        SerializedString,
        GameBalanceId,
        String,
        Enum,
        TagMap,
        SerializeData
    }

    public class ClassMapper
    {
        private static readonly HashSet<ValueTypeDescriptor> AlreadyMapped = new HashSet<ValueTypeDescriptor>();
        private static readonly Queue<ValueTypeDescriptor> ToBeMapped = new Queue<ValueTypeDescriptor>();

        public static void MapRecursively(ValueTypeDescriptor valueType)
        {
            ToBeMapped.Clear();
            ToBeMapped.Enqueue(valueType);
            while (ToBeMapped.Any())
            {
                valueType = ToBeMapped.Dequeue();
                if (valueType == null)
                    break;

                Map(valueType);
            }
        }

        private static readonly HashSet<string> OffsetEnumObjects = new HashSet<string>()
        {
            "GameBalance",
        };

        public const string Indent = "    ";

        public static string Map(ValueTypeDescriptor valueType)
        {
            var result = new StringBuilder();

            if (OffsetEnumObjects.Contains(valueType.Name))
            {
                var offsetEnum = new OffsetEnum(valueType, DataTypeGroup.Object);
                result.Append(Environment.NewLine);
                result.Append(offsetEnum);
                result.Append(Environment.NewLine);
            }

            result.Append(Environment.NewLine);
            result.Append($"[CompilerGenerated] {Environment.NewLine}");
            result.Append($"public class Native{valueType.Name} : MemoryWrapper {Environment.NewLine}");
            result.Append($"{{{Environment.NewLine}");
            var index = 0;
            var endMarker = valueType.FieldDescriptors.LastOrDefault(f => f.Type.Name == "DT_NULL" && f.BaseType.Name == "DT_NULL");
            if (endMarker != null)
            {
                result.Append($"{Indent}public const int SizeOf = {endMarker.Offset}; // {GetHexOffset(endMarker.Offset)}{Environment.NewLine}");
            }

            foreach (var field in valueType.FieldDescriptors.OrderBy(f => f.Offset))
            {
                if (!AlreadyMapped.Contains(field.Type) && !ToBeMapped.Contains(field.Type) && !field.Type.Name.StartsWith("DT_"))
                    ToBeMapped.Enqueue(field.Type);

                if (!AlreadyMapped.Contains(field.BaseType) && !ToBeMapped.Contains(field.BaseType) && !field.BaseType.Name.StartsWith("DT_"))
                    ToBeMapped.Enqueue(field.BaseType);

                index++;
                var group = GetTypeGroup(field.Type.Name);
                var comment = GetComments(group, field);
                var offsetHex = GetHexOffset(field.Offset);
                var typeName = GetTypeName(group, field.Type.Name, field);
                var baseTypeGroup = GetTypeGroup(field.BaseType.Name);
                var baseTypeName = GetTypeName(baseTypeGroup, field.BaseType.Name, field);

                if(group == DataTypeGroup.VariableArray && baseTypeGroup == DataTypeGroup.Simple)
                {
                    group = DataTypeGroup.SerializedString;
                }

                switch (group)
                {
                    case DataTypeGroup.Simple:
                        var name = baseTypeGroup == DataTypeGroup.Object ? baseTypeName : typeName;
                        result.AppendLine($"{Indent}public {typeName} _{index}_{offsetHex}_{name} => ReadOffset<{typeName}>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.Enum:
                        result.AppendLine($"{Indent}public {typeName} _{index}_{offsetHex}_{group} => ReadOffset<{typeName}>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.Sno:
                        var snoId = (SnoType)field.GroupId;
                        result.AppendLine($"{Indent}public int _{index}_{offsetHex}_{snoId}_{group} => ReadOffset<int>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.GameBalanceId:
                        var gbId = (SnoGameBalanceType)field.GroupId;
                        result.AppendLine($"{Indent}public int _{index}_{offsetHex}_{gbId}_{group} => ReadOffset<int>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.FixedArray:
                        var readType = GetSimplePropertyTypeName(field.BaseType.Name) != null ? "ReadArray" : "ReadObjects";
                        result.AppendLine($"{Indent}public List<{baseTypeName}> _{index}_{offsetHex}_{group} => {readType}<{baseTypeName}>({offsetHex}, {field.FixedArrayLength}); {comment}");
                        break;
                    case DataTypeGroup.VariableArray:
                        var serializeDataOffset = GetHexOffset(field.Offset + field.VariableArraySerializeOffsetDiff);
                        result.AppendLine($"{Indent}public List<{baseTypeName}> _{index}_{offsetHex}_{group} => ReadSerializedObjects<{baseTypeName}>({offsetHex}, {serializeDataOffset}); {comment}");
                        break;
                    case DataTypeGroup.Object:
                        result.AppendLine($"{Indent}public {typeName} _{index}_{offsetHex}_{group} => ReadObject<{typeName}>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.SerializeData:
                        result.AppendLine($"{Indent}public NativeSerializeData _{index}_{offsetHex}_SerializeData => ReadObject<NativeSerializeData>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.SerializedString:
                        var serializeStringDataOffset = GetHexOffset(field.Offset + field.VariableArraySerializeOffsetDiff);
                        result.AppendLine($"{Indent}public string _{index}_{offsetHex}_SerializedString => ReadSerializedString({offsetHex}, {serializeStringDataOffset}); {comment}");
                        break;
                    case DataTypeGroup.TagMap:
                        result.AppendLine($"//{Indent}public TagMap _{index}_{offsetHex}_TagMap => ReadOffset<TagMap>({offsetHex}); {comment}");
                        break;
                    case DataTypeGroup.String:
                        result.AppendLine($"{Indent}public string _{index}_{offsetHex}_String => ReadString({offsetHex}); {comment}");
                        break;
                    default:
                        if (!field.Equals(endMarker))
                        {
                            result.AppendLine($"// Unknown index={index} @Offset{offsetHex} Type={field.Type.Name} BaseType={field.BaseType.Name} {comment}");
                        }
                        break;

                }
            }

            result.Append($"}}{Environment.NewLine}");
            Logger.LogRaw(result.ToString());

            if (!AlreadyMapped.Contains(valueType))
                AlreadyMapped.Add(valueType);

            return result.ToString();
        }

        public class OffsetEnum
        {
            public string Name;

            public IndexedList<EnumValue> Values = new IndexedList<EnumValue>();

            public class EnumValue
            {
                public string Name;
                public string Value;
                public FieldDescriptor Field;

                public override string ToString() => $"   {Name} = {Value},";
            }

            public EnumValue this[FieldDescriptor field] 
                => Values.FirstOrDefault(v => Equals(v.Field, field));

            public OffsetEnum(ValueTypeDescriptor valueType, DataTypeGroup groupRestriction)
            {
                Name = $"Native{valueType.Name}Offsets";
                Values.Clear();
                Values.Add(new EnumValue { Name = "None", Value = "0" });
                foreach (var field in valueType.FieldDescriptors.OrderBy(f => f.Offset))
                {
                    var group = GetTypeGroup(field.Type.Name);
                    if (groupRestriction != DataTypeGroup.None && group != groupRestriction)
                        continue;
                                        
                    Values.Add(new EnumValue
                    {
                        Field = field,
                        Name = GetTypeName(group, field.Type.Name, field),
                        Value = GetHexOffset(field.Offset)
                    });
                }
            }

            private string NewLine => Environment.NewLine;

            public override string ToString() 
                => $"public enum {Name} {{ {NewLine} {string.Join(NewLine, Values)} {NewLine} }}";
        }

        private static string GetHexOffset(int intOffset)
        {
            return "0x" + intOffset.ToString("X");
        }

        private static string GetComments(DataTypeGroup group, FieldDescriptor field)
        {
            var comments = new List<string>();
            var size = GetSizeInBytes(field, @group);

            comments.Add(!string.IsNullOrEmpty(field._x4C_Text) ? $"Text={field._x4C_Text}" : string.Empty);
            comments.Add(field.VariableArraySerializeOffsetDiff != 0 ? $" VarArrSerializeOffsetDiff={field.VariableArraySerializeOffsetDiff}" : string.Empty);
            comments.Add(field.FixedArraySerializeOffsetDiff != 0 ? $" FixArrSerializeOffsetDiff={field.FixedArraySerializeOffsetDiff}" : string.Empty);
            comments.Add(field.Flags != FieldDescriptorFlags.None ? $" Flags={field.Flags}" : string.Empty);
            //comments.Add(size > 0 ? $" Size={size}" : string.Empty);
            comments.Add(field.SymbolTable > 0 ? $" SymbolTable=@{field.SymbolTable}" : string.Empty);
            comments.Add((field.Min != 0 ? $"Min={field.Min}" : string.Empty) + (field.Max != 0 ? $"Max={field.Max}" : string.Empty));
            return comments.Any(comment => !string.IsNullOrEmpty(comment)) ? comments.Aggregate("// ", (c, str) => c + " " + str) : string.Empty;
        }

        private static int GetSizeInBytes(FieldDescriptor field, DataTypeGroup group)
        {
            if (group == DataTypeGroup.Enum)
                return 4;

            if (field.UsedBits > 0)
                return field.UsedBits / 8;

            return -1;
        }

        public static DataTypeGroup GetTypeGroup(string d3Type)
        {
            switch (d3Type)
            {
                case "DT_VECTOR3D":
                case "DT_IVECTOR2D":
                case "DT_INT":
                case "DT_FLOAT":
                case "DT_BYTE":
                case "DT_TIME":
                    return DataTypeGroup.Simple;
                case "DT_VARIABLEARRAY":
                    return DataTypeGroup.VariableArray;
                case "DT_FIXEDARRAY":                    
                    return DataTypeGroup.FixedArray;
                case "DT_SNO":
                    return DataTypeGroup.Sno;
                case "DT_ENUM":
                    return DataTypeGroup.Enum;
                case "DT_TAGMAP":
                    return DataTypeGroup.TagMap;
                case "DT_CHARARRAY":
                    return DataTypeGroup.String;
                case "DT_GBID":
                    return DataTypeGroup.GameBalanceId;
                case "DT_CSTRING":
                    return DataTypeGroup.SerializedString;
                case "DT_NULL":
                    return DataTypeGroup.Null;
            }

            if (d3Type.Contains("SerializeData"))
                return DataTypeGroup.SerializeData;

            if (!d3Type.StartsWith("DT"))
                return DataTypeGroup.Object;

            return DataTypeGroup.None;
        }

        public static string GetTypeName(DataTypeGroup group, string d3Name, FieldDescriptor field)
        {
            switch (group)
            {
                case DataTypeGroup.Null:
                    return "null";
                case DataTypeGroup.Simple:
                    return GetSimplePropertyTypeName(d3Name);
                case DataTypeGroup.Object:
                    return "Native" + d3Name;
                case DataTypeGroup.Sno:
                    return "Native" + ((SnoType)field.GroupId);
                case DataTypeGroup.FixedArray:
                    return GetTypeName(GetTypeGroup(field.BaseType.Name), field.BaseType.Name, field);
                case DataTypeGroup.VariableArray:
                    return GetTypeName(GetTypeGroup(field.BaseType.Name), field.BaseType.Name, field);
                case DataTypeGroup.Enum:
                    return SymbolManager.Tables.ContainsKey(field.SymbolTable) ? SymbolManager.Tables[field.SymbolTable].Name : "int";
                case DataTypeGroup.GameBalanceId:
                    return "int";
            }
            return $"TypeNameNotHandled({d3Name}:{field.BaseType.Name})";
        }

        public static string GetSimplePropertyTypeName(string d3Name)
        {
            switch (d3Name)
            {
                case "DT_VECTOR3D":
                    return "Vector3";
                case "DT_IVECTOR2D":
                    return "Vector2";
                case "DT_INT":
                    return "int";
                case "DT_FLOAT":
                    return "float";
                case "DT_BYTE":
                    return "byte";
                case "DT_TIME":
                    return "int";
            }
            return null;
        }
    }
}