////!CompilerOption:AddRef:Newtonsoft.Json.dll
//using System; using Trinity.Framework; using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq; using Trinity.Framework;
//using System.Reflection;
//using System.Xml.Serialization;
//using Adventurer.Game.Quests;
//using Adventurer.Util;
//using Newtonsoft.Json;
//using Zeta.Game;
//using Zeta.Game.Internals;

//namespace Adventurer.Reference
//{
//    public static class QuestDataStorage
//    {
//        public static QuestData LoadQuestData(int questId)
//        {
//            var path = Path.Combine(DataPath, "Final");
//            Directory.CreateDirectory(path);
//            var fileName = Directory.GetFiles(path).FirstOrDefault(f => f.StartsWith(Path.Combine(path, questId + " - ")));
//            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
//            {
//                var questData = LoadQuestDataFromFile(fileName);
//                questData.IsDataComplete = true;
//                return questData;
//            }

//            path = Path.Combine(DataPath, "Completed");
//            Directory.CreateDirectory(path);
//            fileName = Directory.GetFiles(path).FirstOrDefault(f => f.StartsWith(Path.Combine(path, questId + " - ")));
//            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
//            {
//                var questData = LoadQuestDataFromFile(fileName);
//                questData.IsDataComplete = true;
//                return questData;
//            }

//            path = Path.Combine(DataPath, "Partial");
//            Directory.CreateDirectory(path);
//            var files = Directory.GetFiles(path);
//            fileName = files.FirstOrDefault(f => f.StartsWith(Path.Combine(path, questId + " - ")));
//            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
//            {
//                return LoadQuestDataFromFile(fileName);
//            }

//            return null;
//        }

//        public static Dictionary<int, QuestData> LoadFinalQuestDatas()
//        {
//            var questDatas = new Dictionary<int, QuestData>();
//            var path = Path.Combine(DataPath, "Final");
//            Directory.CreateDirectory(path);
//            var files = Directory.GetFiles(path);
//            foreach (var file in files)
//            {
//                if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
//                {
//                    QuestData questData;
//                    try
//                    {
//                        questData = LoadQuestDataFromFile(file);

//                    }
//                    catch (Exception)
//                    {
//                        Core.Logger.Error("Error loading quest data from {0}", file);
//                        throw;
//                    }
//                    questData.IsReadOnly = true;
//                    questDatas.Add(questData.QuestId, questData);
//                }
//            }

//            path = Path.Combine(DataPath, "Completed");
//            Directory.CreateDirectory(path);
//            files = Directory.GetFiles(path);
//            foreach (var file in files)
//            {
//                if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
//                {
//                    QuestData questData;
//                    try
//                    {
//                        questData = LoadQuestDataFromFile(file);

//                    }
//                    catch (Exception)
//                    {
//                        Core.Logger.Error("Error loading quest data from {0}", file);
//                        throw;
//                    }
//                    questData.IsReadOnly = true;
//                    if (!questDatas.ContainsKey(questData.QuestId))
//                    questDatas.Add(questData.QuestId, questData);
//                }
//            }

//            path = Path.Combine(DataPath, "Partial");
//            Directory.CreateDirectory(path);
//            files = Directory.GetFiles(path);
//            foreach (var file in files)
//            {
//                if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
//                {
//                    QuestData questData;
//                    try
//                    {
//                        questData = LoadQuestDataFromFile(file);

//                    }
//                    catch (Exception)
//                    {
//                        Core.Logger.Error("Error loading quest data from {0}", file);
//                        throw;
//                    }
//                    questData.IsReadOnly = true;
//                    if (!questDatas.ContainsKey(questData.QuestId))
//                        questDatas.Add(questData.QuestId, questData);
//                }
//            }
//            return questDatas;
//        }

//        private static QuestData LoadQuestDataFromFile(string filePath)
//        {
//            //try
//            //{
//            var data = File.ReadAllText(filePath);
//            return JsonConvert.DeserializeObject<QuestData>(data);
//            //}
//            //catch (Exception)
//            //{
//            //    return null;
//            //}
//        }

//        public static void SaveQuestData(QuestData questData, bool isCompletedQuest)
//        {
//            if (questData.IsReadOnly) return;
//            var filePath = GetQuestPath(questData, "Completed");
//            if (!File.Exists(filePath) && !isCompletedQuest)
//            {
//                filePath = GetQuestPath(questData, "Partial");
//            }
//            OverwriteFile(filePath, JsonConvert.SerializeObject(questData, Formatting.Indented));

//            //OverwriteFile(filePath.Replace(".txt", ".xml"), XmlSerialize(questData));
//            if (isCompletedQuest)
//            {
//                DeletePartialFile(GetQuestPath(questData, "Partial"));
//            }
//        }

//        public static void CleanUpPartials()
//        {
//            var path = Path.Combine(DataPath, "Completed");
//            Directory.CreateDirectory(path);
//            var files = Directory.GetFiles(path);
//            foreach (var file in files)
//            {
//                if (File.Exists(file.Replace("Completed", "Partial")))
//                {
//                    try
//                    {
//                        File.Delete(file.Replace("Completed", "Partial"));
//                    }
//                    catch (Exception)
//                    {
//                    }
//                }
//            }
//        }

//        private static string XmlSerialize<T>(T data)
//        {
//            var stringWriter = new StringWriter();
//            var mySerializer = new XmlSerializer(typeof(T));
//            mySerializer.Serialize(stringWriter, data);
//            return stringWriter.ToString();
//        }

//        public static void OverwriteFile(string filePath, string data)
//        {
//            File.WriteAllText(filePath, data);
//        }

//        private static void DeletePartialFile(string filePath)
//        {
//            try
//            {
//                File.Delete(filePath);
//            }
//            // ReSharper disable once EmptyGeneralCatchClause
//            catch
//            {
//            }
//        }

//        private static string GetQuestPath(QuestData questData, string folder)
//        {
//            var path = Path.Combine(DataPath, folder);
//            Directory.CreateDirectory(path);
//            path = Path.Combine(path, questData.QuestId + " - " + CleanUpFilename(questData.Name) + ".txt");
//            return path;
//        }

//        private static string CleanUpFilename(string filename)
//        {
//            var invalidChars = new string(Path.GetInvalidFileNameChars());
//            return invalidChars.Aggregate(filename, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty));
//        }

//        public static readonly string DemonBuddyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
//        public static readonly string DataPath = Path.Combine(DemonBuddyPath, "Data", "Adventurer");

//        public static void LogQuestInfo(BountyInfo bountyInfo)
//        {
//            File.AppendAllLines(Path.Combine(DataPath, "Quests.txt"), new[] { string.Format("{0}\t{1}\t{2}", (int)bountyInfo.Quest, bountyInfo.Quest, bountyInfo.Info.DisplayName.Replace("Bounty: ", string.Empty)) });
//        }
//        public static void LogFailedWaypointInfo(QuestData questData, int waypointNumber)
//        {
//            File.AppendAllLines(Path.Combine(DataPath, "FailedWaypoints.txt"), new[] { string.Format("{0}\t{1}\t{2}\t{3}", questData.QuestId, string.Join(", ", questData.LevelAreaSnoIdIds), waypointNumber, AdvDia.CurrentLevelAreaSnoId) });
//        }
//    }

//}