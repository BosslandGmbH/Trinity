using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using System.Collections.Generic;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;

namespace Trinity.Framework.Helpers
{
    public class SnoHelper
    {
        private static readonly Dictionary<int, SNORecordActor> ActorInfoCache = new Dictionary<int, SNORecordActor>();

        public static SNORecordActor GetActorInfo(int actorSnoId)
        {
            Core.Logger.Verbose(LogCategory.CrashDebug, "Requesting SnoRecord Actor");

            if (ActorInfoCache.ContainsKey(actorSnoId))
                return ActorInfoCache[actorSnoId];

            var entry = ZetaDia.SNO[SNOGroup.Actor].GetRecord<SNORecordActor>(actorSnoId);
            ActorInfoCache[actorSnoId] = entry;
            return entry;
        }

        private static readonly Dictionary<int, SNORecordMonster> MonsterInfoCache = new Dictionary<int, SNORecordMonster>();

        public static SNORecordMonster GetMonsterInfo(int monsterSnoId)
        {
            Core.Logger.Verbose(LogCategory.CrashDebug, "Requesting SnoRecord Monster");

            if (MonsterInfoCache.ContainsKey(monsterSnoId))
                return MonsterInfoCache[monsterSnoId];

            var entry = ZetaDia.SNO[SNOGroup.Monster].GetRecord<SNORecordMonster>(monsterSnoId);
            MonsterInfoCache[monsterSnoId] = entry;
            return entry;
        }

        private static readonly Dictionary<int, SNORecordScene> SceneInfoCache = new Dictionary<int, SNORecordScene>();

        public static SNORecordScene GetSceneInfo(int sceneSnoId)
        {
            Core.Logger.Verbose(LogCategory.CrashDebug, "Requesting SnoRecord Scene");

            if (SceneInfoCache.ContainsKey(sceneSnoId))
                return SceneInfoCache[sceneSnoId];

            var entry = ZetaDia.SNO[SNOGroup.Scene].GetRecord<SNORecordScene>(sceneSnoId);
            SceneInfoCache[sceneSnoId] = entry;
            return entry;
        }

        private static readonly Dictionary<int, SnoDictionary<string>> StringListCache = new Dictionary<int, SnoDictionary<string>>();

        public static SnoDictionary<string> GetStringList(int stringListSnoId)
        {
            Core.Logger.Verbose(LogCategory.CrashDebug, "Requesting SnoRecord StringList");

            if (StringListCache.ContainsKey(stringListSnoId))
                return StringListCache[stringListSnoId];

            var entry = ZetaDia.SNO[SNOGroup.StringList].GetRecord<SNORecord>(stringListSnoId);
            var stringList = MemoryWrapper.Create<SnoStringList>(entry.BaseAddress);
            var stringListDictionary = stringList.ToSnoDictionary();

            StringListCache[stringListSnoId] = stringListDictionary;
            return stringListDictionary;
        }

        public static string GetStringListEntry(SnoStringListType type, int stringListId)
        {
            return GetStringList((int)type)?[stringListId];
        }

    }
}
