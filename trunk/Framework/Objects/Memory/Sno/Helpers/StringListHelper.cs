using System.Collections.Generic;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Sno.Types;

namespace Trinity.Framework.Objects.Memory.Sno.Helpers
{
    public class StringListHelper
    {
        private readonly Dictionary<int, SnoDictionary<string>> _cachedStringLists = new Dictionary<int, SnoDictionary<string>>();

        public SnoDictionary<string> GetStringList(SnoStringListType type)
        {
            return GetStringList((int)type);
        }

        public SnoDictionary<string> GetStringList(int gameBalanceId)
        {
            SnoDictionary<string> dict;

            if (_cachedStringLists.TryGetValue(gameBalanceId, out dict))
            {
                return dict;
            }

            var entityId = SnoManager.Core.GetEntityId(SnoType.StringList, gameBalanceId);
            if (entityId == 0)
            {
                return new SnoDictionary<string>();
            }

            var stringList = SnoManager.Groups.StringList.Container[entityId].Value;

            //dict = ToSnoDictionary(stringList);
            dict = stringList.ToSnoDictionary();
            _cachedStringLists.Add(gameBalanceId, dict);

            return dict;
        }

        public SnoDictionary<string> ToSnoDictionary(NativeStringList stringlist)
        {
            var dictionary = new SnoDictionary<string>();
            foreach (var entry in stringlist._1_0x10_VariableArray)
            {
                dictionary.Add(entry._1_0x0_SerializedString, entry._3_0x10_SerializedString);
            }
            return dictionary;
        }

        public string GetStringListValue(SnoStringListType type, int stringListId)
        {
            return GetStringListValue((int)type, stringListId);
        }

        public string GetStringListValue(int listGameBalanceId, int stringListId)
        {
            var dict = GetStringList(listGameBalanceId);
            string value;
            if (dict.TryGetValue(stringListId, out value))
            {
                return value;
            }
            return string.Empty;
        }

        public string GetStringListValueByIndex(int listGameBalanceId, int index)
        {
            var dict = GetStringList(listGameBalanceId);
            if (index >= 0 && index < dict.Count)
            {
                return dict[index];
            }
            return string.Empty;
        }
    }
}