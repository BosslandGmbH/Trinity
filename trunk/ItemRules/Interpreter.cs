using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Cache;
using Trinity.Helpers;
using Trinity.ItemRules.Core;
using Trinity.Reference;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Items;
using Zeta.Game.Internals.Actors;

namespace Trinity.ItemRules
{
    #region Interpreter

    /// <summary>
    /// +---------------------------------------------------------------------------+
    /// | _______ __                     ______         __                   ______ 
    /// ||_     _|  |_.-----.--------.  |   __ \.--.--.|  |.-----.-----.    |__    |
    /// | _|   |_|   _|  -__|        |  |      <|  |  ||  ||  -__|__ --|    |    __|
    /// ||_______|____|_____|__|__|__|  |___|__||_____||__||_____|_____|    |______|
    /// |+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ 
    /// +---------------------------------------------------------------------------+
    /// | - Created by darkfriend77
    /// +---------------------------------------------------------------------------+
    /// </summary>
    public class Interpreter
    {
        // enumerations
        public enum LogType
        {
            LOG,
            DEBUG,
            ERROR
        };

        public enum InterpreterAction
        {
            PICKUP,
            IGNORE,
            IDENTIFY,
            UNIDENT,
            KEEP,
            SCORE,
            TRASH,
            SALVAGE,
            SELL,
            NULL
        };

        // final variables
        readonly string version = "2.2.3.0";

        // dis files
        readonly string translationFile = "translation.dis";
        readonly string pickupFile = "pickup.dis";
        readonly string identifyFile = "identify.dis";
        readonly string salvageSellFile = "salvageSell.dis";

        // log files
        readonly string KeepLogFile = "IR2Keep.log"; // Keep & Trash
        readonly string PickLogFile = "IR2Pick.log"; // Pick & Ignore
        readonly string RestLogFile = "IR2Rest.log"; // Unident, Identify, Salvage & Sell
        readonly string BugsLogFile = "IR2Bugs.log"; // Bugs
        readonly string tranLogFile = "IR2Tran.log"; // Transation fixes

        readonly Regex macroPattern = new Regex(@"(@[A-Z]+)[ ]*:=[ ]*(.+)", RegexOptions.Compiled);
        readonly string assign = "->", SEP = ";";

        TrinityItemQuality logPickQuality, logKeepQuality;

        // objects
        ArrayList pickUpRuleSet, identifyRuleSet, keepRuleSet, salvageSellRuleSet;
        TextWriter log;
        Scanner scanner;
        Parser parser;
        //TextHighlighter highlighter;

        // dictonary for the item
        public static Dictionary<string, object> itemDic;

        // dictonary for the translation
        private Dictionary<string, string> nameToBalanceId;

        // dictonary for the use of macros
        private Dictionary<string, string> macroDic;

        // dictionary for cached rules
        Dictionary<string, string[]> cachedRules = new Dictionary<string, string[]>();

        // dictionary for cached interpreter actions
        Dictionary<string, InterpreterAction> interpreterActionCache = new Dictionary<string, InterpreterAction>();

        /// <summary>
        /// 
        /// </summary>
        public Interpreter()
        {
            // initialize parser objects
            scanner = new Scanner();
            parser = new Parser(scanner);
            //highlighter = new TextHighlighter(richTextBox, scanner, parser);

            // read configuration file and item files now
            readConfiguration();
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, " _______________________________________");
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, " ___-|: Darkfriend's Item Rules 2 :|-___");
            Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, " ___________________Rel.-v {0}_______", version);
        }

        public void reset()
        {
            if (FileManager.BattleTagName != null)
            {
                string actualKeepLog = Path.Combine(FileManager.LoggingPath, KeepLogFile);
                string archivePath = Path.Combine(FileManager.LoggingPath, "IR2Archive");
                string archiveKeepLog = Path.Combine(archivePath, "IR2KeepArchive.log");
                string archivePickLog = Path.Combine(archivePath, "IR2PickArchive.log");

                if (!File.Exists(actualKeepLog))
                    return;

                if (!Directory.Exists(archivePath))
                    Directory.CreateDirectory(archivePath);

                using (Stream input = File.OpenRead(actualKeepLog))
                using (Stream output = new FileStream(archiveKeepLog, FileMode.Append, FileAccess.Write, FileShare.None))
                    input.CopyTo(output);

                File.Delete(actualKeepLog);
            }
        }

        public bool reloadFromUI()
        {
            readConfiguration();
            return false;
        }

        /// <summary>
        /// Loads (or re-loads) the ItemRules configuration from settings and .dis entries
        /// </summary>
        public void readConfiguration()
        {
            try
            {
                reset();

                // initialize or reset ruleSet array
                pickUpRuleSet = new ArrayList();
                identifyRuleSet = new ArrayList();
                keepRuleSet = new ArrayList();
                salvageSellRuleSet = new ArrayList();

                // instantiating our macro dictonary
                macroDic = new Dictionary<string, string>();

                // use TrinityPlugin setting
                if (TrinityPlugin.Settings.Loot.ItemRules.Debug)
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ItemRules is running in debug mode!", logPickQuality);

                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ItemRules is using the {0} rule set.", TrinityPlugin.Settings.Loot.ItemRules.ItemRuleType.ToString().ToLower());
                logPickQuality = getTrinityItemQualityFromString(TrinityPlugin.Settings.Loot.ItemRules.PickupLogLevel.ToString());
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "PICKLOG = {0} ", logPickQuality);
                logKeepQuality = getTrinityItemQualityFromString(TrinityPlugin.Settings.Loot.ItemRules.KeepLogLevel.ToString());
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "KEEPLOG = {0} ", logKeepQuality);

                string rulesPath;
                if (TrinityPlugin.Settings.Loot.ItemRules.ItemRuleSetPath.ToString() != String.Empty)
                {
                    rulesPath = TrinityPlugin.Settings.Loot.ItemRules.ItemRuleSetPath.ToString();
                }
                else
                {
                    rulesPath = Path.Combine(FileManager.ItemRulePath, "Rules", TrinityPlugin.Settings.Loot.ItemRules.ItemRuleType.ToString().ToLower());
                }
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "RULEPATH = {0} ", rulesPath);
                if (!Directory.Exists(rulesPath))
                {
                    Logger.Log("ERROR: RulesPath {0} does not exist!", rulesPath);
                    return;
                }

                // fill translation dictionary
                nameToBalanceId = new Dictionary<string, string>();
                StreamReader streamReader = new StreamReader(Path.Combine(FileManager.ItemRulePath, translationFile));
                string str;
                while ((str = streamReader.ReadLine()) != null)
                {
                    string[] strArrray = str.Split(';');
                    nameToBalanceId[strArrray[1].Replace(" ", "")] = strArrray[0];
                }
                //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, "... loaded: {0} ITEMID translations", nameToBalanceId.Count);

                // parse pickup file
                if (File.Exists(Path.Combine(rulesPath, pickupFile)))
                {
                    pickUpRuleSet = readLinesToArray(new StreamReader(Path.Combine(rulesPath, pickupFile)), pickUpRuleSet);
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... loaded: {0} Pickup rules", pickUpRuleSet.Count);
                }
                else
                {
                    // create an empty pickup file
                    using (File.Create(Path.Combine(rulesPath, pickupFile))) { }
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... created empty pickup rule file", identifyRuleSet.Count);
                }

                // parse identify file
                if (File.Exists(Path.Combine(rulesPath, identifyFile)))
                {
                    identifyRuleSet = readLinesToArray(new StreamReader(Path.Combine(rulesPath, identifyFile)), identifyRuleSet);
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... loaded: {0} Identify rules", identifyRuleSet.Count);
                }
                else
                {
                    // create an empty identify file
                    using (File.Create(Path.Combine(rulesPath, identifyFile))) { }
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... created empty identify rule file", identifyRuleSet.Count);
                }

                if (File.Exists(Path.Combine(rulesPath, salvageSellFile)))
                {
                    // parse salvage file
                    salvageSellRuleSet = readLinesToArray(new StreamReader(Path.Combine(rulesPath, salvageSellFile)), salvageSellRuleSet);
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... loaded: {0} Salvage rules", salvageSellRuleSet.Count);
                }
                else
                {
                    // create an empty salvage file
                    using (File.Create(Path.Combine(rulesPath, salvageSellFile))) { }
                    Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... created empty salvage rules file", identifyRuleSet.Count);
                }

                // parse all keep files
                foreach (TrinityItemQuality itemQuality in Enum.GetValues(typeof(TrinityItemQuality)))
                {
                    string fileName = itemQuality.ToString().ToLower() + ".dis";
                    string filePath = Path.Combine(rulesPath, fileName);
                    int oldValue = keepRuleSet.Count;
                    if (File.Exists(filePath))
                    {
                        keepRuleSet = readLinesToArray(new StreamReader(filePath), keepRuleSet);
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... loaded: {0} {1} Keep rules", (keepRuleSet.Count - oldValue), itemQuality.ToString());
                    }
                }

                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "... loaded: {0} Macros", macroDic.Count);
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "ItemRules loaded a total of {0} {1} rules!", keepRuleSet.Count, TrinityPlugin.Settings.Loot.ItemRules.ItemRuleType.ToString());
            }
            catch (Exception ex)
            {
                Logger.Log("Error reading ItemRules configuration: {0}", ex.ToString());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="streamReader"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        private ArrayList readLinesToArray(StreamReader streamReader, ArrayList array)
        {
            string str = "";
            Match match;
            while ((str = streamReader.ReadLine()) != null)
            {
                str = str.Split(new string[] { "//" }, StringSplitOptions.None)[0]
                    .Replace(" ", "")
                    .Replace("\t", "");

                if (str.Length == 0)
                    continue;

                // - start macro transformation
                match = macroPattern.Match(str);

                if (match.Success)
                {
                    //DbHelper.Log(TrinityLogLevel.Normal, LogCategory.UserInformation, " macro added: {0} := {1}", match.Groups[1].Value, match.Groups[2].Value);
                    macroDic.Add(match.Groups[1].Value, match.Groups[2].Value);
                    continue;
                }
                // - stop macro transformation

                // do simple translation with name to itemid
                if (TrinityPlugin.Settings.Loot.ItemRules.UseItemIDs && str.Contains("[NAME]"))
                {
                    bool foundTranslation = false;
                    foreach (string key in nameToBalanceId.Keys.ToList())
                    {
                        key.Replace(" ", "").Replace("\t", "");
                        if (str.Contains(key))
                        {
                            str = str.Replace(key, nameToBalanceId[key]).Replace("[NAME]", "[ITEMID]");
                            foundTranslation = true;
                            break;
                        }
                    }
                    if (!foundTranslation && TrinityPlugin.Settings.Loot.ItemRules.Debug)
                        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "No translation found for rule: {0}", str);
                }

                array.Add(str);
            }
            return array;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //internal InterpreterAction checkPickUpItem(PickupItem item, ItemEvaluationType evaluationType)
        //{
        //    fillPickupDic(item);

        //    return checkItem(evaluationType);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal InterpreterAction checkItem(ACDItem item, ItemEvaluationType evaluationType)
        {
            try
            {
                fillDic(item);

                return checkItem(evaluationType);
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in checkItem: {0} item: {1}/{2} eval type: {3}", ex, item.Name, item.InternalName, evaluationType);
                switch (evaluationType)
                {
                    case ItemEvaluationType.Keep:
                        return InterpreterAction.KEEP;
                    case ItemEvaluationType.PickUp:
                        return InterpreterAction.PICKUP;
                    case ItemEvaluationType.Salvage:
                        return InterpreterAction.KEEP;
                    case ItemEvaluationType.Sell:
                        return InterpreterAction.KEEP;
                    default:
                        return InterpreterAction.NULL;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public InterpreterAction checkItem(ItemEvaluationType evaluationType)
        {

            InterpreterAction action = InterpreterAction.NULL;
            InterpreterAction defaultAction = InterpreterAction.NULL;

            string validRule = "";

            ArrayList rules = null;

            switch (evaluationType)
            {
                case ItemEvaluationType.PickUp:
                    defaultAction = InterpreterAction.PICKUP;
                    rules = pickUpRuleSet;
                    break;
                case ItemEvaluationType.Sell:
                    defaultAction = InterpreterAction.IDENTIFY;
                    rules = identifyRuleSet;
                    break;
                case ItemEvaluationType.Keep:
                    defaultAction = InterpreterAction.KEEP;
                    rules = keepRuleSet;
                    break;
                case ItemEvaluationType.Salvage:
                    defaultAction = InterpreterAction.SALVAGE;
                    rules = salvageSellRuleSet;
                    break;
                default:
                    defaultAction = InterpreterAction.NULL;
                    rules = null;
                    break;
            }

            foreach (string str in rules)
            {
                ParseErrors parseErrors = null;

                InterpreterAction tempAction = defaultAction;

                string[] strings = str.Split(new string[] { assign }, StringSplitOptions.None);
                if (strings.Count() > 1)
                    tempAction = getInterpreterAction(strings[1]);
                try
                {
                    if (evaluate(strings[0], out parseErrors))
                    {
                        validRule = str;
                        action = tempAction;
                        if (parseErrors.Count > 0)
                            logOut("Have errors with out a catch!"
                                + SEP + "last use rule: " + str
                                + SEP + getParseErrors(parseErrors)
                                + SEP + getFullItem(), InterpreterAction.NULL, LogType.ERROR);
                        break;
                    }
                }
                catch (Exception e)
                {
                    logOut(e.Message
                        + SEP + "last use rule: " + str
                        + SEP + getParseErrors(parseErrors)
                        + SEP + getFullItem()
                        + SEP + e.ToString()
                        , InterpreterAction.NULL, LogType.ERROR);
                }
                if (parseErrors != null && parseErrors.Count > 0)
                {
                    if (parseErrors.Count > 0)
                        logOut("Have errors in rule! "
                            + SEP + "last use rule: " + str
                            + SEP + getParseErrors(parseErrors)
                            + SEP + getFullItem(), InterpreterAction.NULL, LogType.ERROR);
                }
            }

            logOut(evaluationType, validRule, action);

            return action;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickUp"></param>
        /// <param name="validRule"></param>
        /// <param name="action"></param>
        private void logOut(ItemEvaluationType evaluationType, string validRule, InterpreterAction action)
        {

            string logString = getFullItem() + validRule;

            TrinityItemQuality quality = TrinityItemQuality.Unknown;

            if (itemDic.ContainsKey("[QUALITY]"))
                quality = getTrinityItemQualityFromString(itemDic["[QUALITY]"]);

            switch (action)
            {
                case InterpreterAction.PICKUP:
                    if (quality >= logPickQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.IGNORE:
                    if (quality >= logPickQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.IDENTIFY:
                    //if (quality >= logPickQuality)
                    //    logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.UNIDENT:
                    if (quality >= logPickQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.KEEP:
                    if (quality >= logKeepQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.TRASH:
                    if (quality >= logKeepQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.SCORE:
                    if (quality >= logKeepQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.SALVAGE:
                    //if (quality >= logKeepQuality)
                    //    logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.SELL:
                    //if (quality >= logKeepQuality)
                    //    logOut(logString, action, LogType.LOG);
                    break;
                case InterpreterAction.NULL:
                    if (quality >= logPickQuality)
                        logOut(logString, action, LogType.LOG);
                    break;
            }
        }

        // todo use an enumeration value
        /// <summary>
        /// 
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        private TrinityItemQuality getTrinityItemQualityFromString(object quality)
        {
            TrinityItemQuality trinityItemQuality;
            if (Enum.TryParse<TrinityItemQuality>(quality.ToString(), true, out trinityItemQuality))
                return trinityItemQuality;
            else
                return TrinityItemQuality.Common;
        }

        private List<string> getDistinctItemQualitiesList()
        {
            List<string> result = new List<string>();
            foreach (ItemQuality itemQuality in Enum.GetValues(typeof(ItemQuality)))
            {
                string quality = Regex.Replace(itemQuality.ToString(), @"[\d]", string.Empty);
                if (!result.Contains(quality))
                    result.Add(quality);
            };
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="item"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private bool evaluate(string str, out ParseErrors parseErrors)
        {
            bool result = false;
            ItemRules.Core.ParseTree tree = parser.Parse(str);
            parseErrors = tree.Errors;
            object obj = tree.Eval(null);

            if (!Boolean.TryParse(obj.ToString(), out result))
                tree.Errors.Add(new ParseError("TryParse Boolean failed!", 101, 0, 0, 0, 0));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private object evaluateExpr(string str, out ParseErrors parseErrors)
        {
            ItemRules.Core.ParseTree tree = parser.Parse(str);
            parseErrors = tree.Errors;
            return tree.Eval(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private InterpreterAction getInterpreterAction(string str)
        {
            if (str == null)
                return InterpreterAction.NULL;

            InterpreterAction action = InterpreterAction.NULL;
            if (!interpreterActionCache.TryGetValue(str, out action))
            {
                if (!Enum.TryParse<InterpreterAction>(str.Replace("[", "").Replace("]", ""), out action))
                {
                    interpreterActionCache.Add(str, action);
                    return InterpreterAction.NULL;
                }
                interpreterActionCache.Add(str, action);
            }

            return action;
            //foreach (InterpreterAction action in Enum.GetValues(typeof(InterpreterAction)))
            //    if (str.IndexOf(action.ToString()) != -1)
            //        return action;
            //return InterpreterAction.NULL;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="logType"></param>
        public void logOut(string str, InterpreterAction action, LogType logType)
        {
            // no debugging when flag set false
            if (logType == LogType.DEBUG && !TrinityPlugin.Settings.Loot.ItemRules.Debug)
                return;

            if (!TrinityPlugin.Settings.Advanced.ItemRulesLogs)
                return;

            if (!Directory.Exists(Path.GetDirectoryName(FileManager.LoggingPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FileManager.LoggingPath));

            string filePath = string.Empty;
            switch (action)
            {
                case InterpreterAction.PICKUP:
                case InterpreterAction.IGNORE:
                    filePath = Path.Combine(FileManager.LoggingPath, PickLogFile);
                    log = new StreamWriter(filePath, true);
                    break;
                case InterpreterAction.IDENTIFY:
                case InterpreterAction.UNIDENT:
                    filePath = Path.Combine(FileManager.LoggingPath, RestLogFile);
                    log = new StreamWriter(filePath, true);
                    break;
                case InterpreterAction.KEEP:
                case InterpreterAction.TRASH:
                case InterpreterAction.SCORE:
                    filePath = Path.Combine(FileManager.LoggingPath, KeepLogFile);
                    log = new StreamWriter(filePath, true);
                    break;
                case InterpreterAction.SALVAGE:
                case InterpreterAction.SELL:
                    filePath = Path.Combine(FileManager.LoggingPath, RestLogFile);
                    log = new StreamWriter(filePath, true);
                    break;
                case InterpreterAction.NULL:
                    filePath = Path.Combine(FileManager.LoggingPath, BugsLogFile);
                    log = new StreamWriter(filePath, true);
                    break;
            }

            log.WriteLine(DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + ".Hero" + SEP + logType + SEP + action + SEP + str);
            log.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private string getParseErrors(ParseErrors parseErrors)
        {
            if (parseErrors == null) return null;
            string result = "tree.Errors = " + parseErrors.Count() + SEP;
            foreach (ParseError parseError in parseErrors)
                result += "ParseError( " + parseError.Code + "): " + parseError.Message + SEP;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getFullItem()
        {
            string result = "";

            // add stats            
            foreach (string key in itemDic.Keys)
            {
                object value;
                if (itemDic.TryGetValue(key, out value))
                {
                    if (value is float && (float)value > 0)
                        result += key.ToUpper() + ":" + ((float)value).ToString("0.00").Replace(".00", "") + SEP;
                    else if (value is string && (string)value != "")
                        result += key.ToUpper() + ":" + value.ToString() + SEP;
                    else if (value is bool)
                        result += key.ToUpper() + ":" + value.ToString() + SEP;
                    else if (value is int)
                        result += key.ToUpper() + ":" + value.ToString() + SEP;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool getVariableValue(string key, out object obj)
        {

            string[] strArray = key.Split('.');

            if (Interpreter.itemDic.TryGetValue(strArray[0], out obj) && strArray.Count() > 1)
            {
                switch (strArray[1])
                {
                    case "dual":
                        if (obj is float && (float)obj > 0)
                            obj = 1f;
                        break;
                    case "max":
                        object itemType, twoHand;
                        double result;
                        if (obj is float
                            && Interpreter.itemDic.TryGetValue("[TYPE]", out itemType)
                            && Interpreter.itemDic.TryGetValue("[TWOHAND]", out twoHand)
                            && MaxStats.maxItemStats.TryGetValue(itemType.ToString() + twoHand.ToString() + strArray[0], out result)
                            && result > 0)
                            obj = (float)obj / (float)result;
                        else
                            obj = (float)0;
                        break;
                }
            }

            return (obj != null);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="level"></param>
        ///// <param name="itemQuality"></param>
        ///// <param name="itemBaseType"></param>
        ///// <param name="itemType"></param>
        ///// <param name="isOneHand"></param>
        ///// <param name="isTwoHand"></param>
        ///// <param name="gameBalanceId"></param>
        //private void fillPickupDic(PickupItem item)
        //{
        //    object result;
        //    itemDic = new Dictionary<string, object>();

        //    // add log unique key
        //    itemDic.Add("[KEY]", item.DynamicID.ToString());

        //    // - BASETYPE ---------------------------------------------------------//
        //    itemDic.Add("[BASETYPE]", item.DBBaseType.ToString());

        //    // - TYPE -------------------------------------------------------------//
        //    /// TODO remove this check if it isnt necessary anymore
        //    if (item.ItemType == ItemType.Unknown && (item.Name.Contains("Plan") || item.Name.Contains("Design")))
        //    {
        //        Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "There are still buggy itemType infos for craftingPlan around {0} has itemType = {1}", item.Name, item.ItemType);
        //        result = ItemType.CraftingPlan.ToString();
        //    }
        //    else result = item.ItemType.ToString();
        //    itemDic.Add("[TYPE]", result);

        //    // - QUALITY -------------------------------------------------------//
        //    itemDic.Add("[QUALITY]", Regex.Replace(item.Quality.ToString(), @"[\d-]", string.Empty));
        //    itemDic.Add("[D3QUALITY]", item.Quality.ToString());

        //    // - ROLL ----------------------------------------------------------//
        //    float roll;
        //    if (float.TryParse(Regex.Replace(item.Quality.ToString(), @"[^\d]", string.Empty), out roll))
        //        itemDic.Add("[ROLL]", roll);
        //    else
        //        itemDic.Add("[ROLL]", 0f);

        //    // - NAME -------------------------------------------------------------//
        //    itemDic.Add("[NAME]", item.Name.ToString().Replace(" ", ""));

        //    // - LEVEL ------------------------------------------------------------//
        //    itemDic.Add("[LEVEL]", (float)item.Level);
        //    itemDic.Add("[ONEHAND]", item.IsOneHand);
        //    itemDic.Add("[TWOHAND]", item.IsTwoHand);
        //    itemDic.Add("[UNIDENT]", (bool)true);
        //    itemDic.Add("[INTNAME]", item.InternalName);
        //    itemDic.Add("[ITEMID]", item.BalanceID.ToString());
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void fillDic(ACDItem item)
        {
            object result;
            itemDic = new Dictionary<string, object>();

            // return if no item available
            if (item == null)
            {
                logOut("received an item with a null reference!", InterpreterAction.NULL, LogType.ERROR);
                return;
            }

            // check for missing translations
            if (TrinityPlugin.Settings.Loot.ItemRules.Debug && item.ItemQualityLevel == ItemQuality.Legendary)
                checkItemForMissingTranslation(item);

            // add log unique key
            itemDic.Add("[KEY]", item.AnnId.ToString());

            // - BASETYPE ---------------------------------------------------------//
            itemDic.Add("[BASETYPE]", item.ItemBaseType.ToString());

            // - TYPE -------------------------------------------------------------//
            /// TODO remove this check if it isnt necessary anymore
            if (item.ItemType == ItemType.Unknown && item.Name.Contains("Plan"))
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "There are still buggy itemType infos for craftingPlan around {0} has itemType = {1}", item.Name, item.ItemType);
                result = ItemType.CraftingPlan.ToString();
            }
            else result = item.ItemType.ToString();
            itemDic.Add("[TYPE]", result);

            // - QUALITY -------------------------------------------------------//
            //itemDic.Add("[QUALITY]", Regex.Replace(item.ItemQualityLevel.ToString(), @"[\d-]", string.Empty));
            itemDic.Add("[QUALITY]", Regex.Replace(item.GetItemQuality().ToString(), @"[\d-]", string.Empty));
            itemDic.Add("[D3QUALITY]", item.ItemQualityLevel.ToString());

            // - ROLL ----------------------------------------------------------//
            float roll;
            if (float.TryParse(Regex.Replace(item.ItemQualityLevel.ToString(), @"[^\d]", string.Empty), out roll))
                itemDic.Add("[ROLL]", roll);
            else
                itemDic.Add("[ROLL]", 0f);

            // - NAME -------------------------------------------------------------//
            itemDic.Add("[NAME]", item.Name.ToString().Replace(" ", ""));

            // - LEVEL ------------------------------------------------------------//
            itemDic.Add("[LEVEL]", (float)item.Level);
            itemDic.Add("[ONEHAND]", item.IsOneHand);
            itemDic.Add("[TWOHAND]", item.IsTwoHand);
            itemDic.Add("[UNIDENT]", item.IsUnidentified);
            itemDic.Add("[INTNAME]", item.InternalName);
            itemDic.Add("[ITEMID]", item.GameBalanceId.ToString());
            itemDic.Add("[SNO]", item.ActorSnoId);

            // if there are no stats return
            //if (item.Stats == null) return;

            itemDic.Add("[STR]", item.Stats.Strength);
            itemDic.Add("[DEX]", item.Stats.Dexterity);
            itemDic.Add("[INT]", item.Stats.Intelligence);
            itemDic.Add("[VIT]", item.Stats.Vitality);
            itemDic.Add("[AS%]", item.Stats.AttackSpeedPercent > 0 ? item.Stats.AttackSpeedPercent : item.Stats.AttackSpeedPercentBonus);
            itemDic.Add("[MS%]", item.Stats.MovementSpeed);
            itemDic.Add("[LIFE%]", item.Stats.LifePercent);
            itemDic.Add("[LS%]", item.Stats.LifeSteal);
            itemDic.Add("[LOH]", item.Stats.LifeOnHit);
            itemDic.Add("[LOK]", item.Stats.LifeOnKill);
            itemDic.Add("[REGEN]", item.Stats.HealthPerSecond);
            itemDic.Add("[GLOBEBONUS]", item.Stats.HealthGlobeBonus);
            itemDic.Add("[DPS]", item.Stats.WeaponDamagePerSecond);
            itemDic.Add("[WEAPAS]", item.Stats.WeaponAttacksPerSecond);
            itemDic.Add("[WEAPDMGTYPE]", item.Stats.WeaponDamageType.ToString());
            itemDic.Add("[WEAPMAXDMG]", item.Stats.WeaponMaxDamage);
            itemDic.Add("[WEAPMINDMG]", item.Stats.WeaponMinDamage);
            itemDic.Add("[CRIT%]", item.Stats.CritPercent);
            itemDic.Add("[CRITDMG%]", item.Stats.CritDamagePercent);
            itemDic.Add("[BLOCK%]", item.Stats.BlockChanceBonus);
            itemDic.Add("[MINDMG]", item.Stats.MinDamage);
            itemDic.Add("[MAXDMG]", item.Stats.MaxDamage);
            itemDic.Add("[ALLRES]", item.Stats.ResistAll);
            itemDic.Add("[RESPHYSICAL]", item.Stats.ResistPhysical);
            itemDic.Add("[RESFIRE]", item.Stats.ResistFire);
            itemDic.Add("[RESLIGHTNING]", item.Stats.ResistLightning);
            itemDic.Add("[RESHOLY]", item.Stats.ResistHoly);
            itemDic.Add("[RESARCANE]", item.Stats.ResistArcane);
            itemDic.Add("[RESCOLD]", item.Stats.ResistCold);
            itemDic.Add("[RESPOISON]", item.Stats.ResistPoison);
            itemDic.Add("[ARMOR]", item.Stats.Armor);
            itemDic.Add("[ARMORBONUS]", item.Stats.ArmorBonus);
            itemDic.Add("[ARMORTOT]", item.Stats.ArmorTotal);
            itemDic.Add("[GF%]", item.Stats.GoldFind);
            itemDic.Add("[MF%]", item.Stats.MagicFind);
            itemDic.Add("[PICKRAD]", item.Stats.PickUpRadius);
            itemDic.Add("[SOCKETS]", (float)item.Stats.Sockets);
            itemDic.Add("[THORNS]", item.Stats.Thorns);
            itemDic.Add("[DMGREDPHYSICAL]", item.Stats.DamageReductionPhysicalPercent);
            itemDic.Add("[MAXARCPOWER]", item.Stats.MaxArcanePower);
            itemDic.Add("[HEALTHSPIRIT]", item.Stats.HealthPerSpiritSpent);
            itemDic.Add("[MAXSPIRIT]", item.Stats.MaxSpirit);
            itemDic.Add("[SPIRITREG]", item.Stats.SpiritRegen);
            itemDic.Add("[ARCONCRIT]", item.Stats.ArcaneOnCrit);
            itemDic.Add("[MAXFURY]", item.Stats.MaxFury);
            itemDic.Add("[MAXDISCIP]", item.Stats.MaxDiscipline);
            itemDic.Add("[HATREDREG]", item.Stats.HatredRegen);
            itemDic.Add("[MAXMANA]", item.Stats.MaxMana);
            itemDic.Add("[MANAREG]", item.Stats.ManaRegen);

            itemDic.Add("[ANCIENT]", (float)item.AncientRank);

            // - ROS & MORE STATS ADDED -------------------------------------------//
            // This include Splash Damage, Cooldown Reduction, Resource Cost
            // Reduction, +% damage to Physical damage skills, and specific Class
            // Skill Bonuses
            itemDic.Add("[SLOWPROC%]", item.Stats.WeaponOnHitSlowProcChance);
            itemDic.Add("[BLINDPROC%]", item.Stats.WeaponOnHitBlindProcChance);
            itemDic.Add("[CHILLPROC%]", item.Stats.WeaponOnHitChillProcChance);
            itemDic.Add("[FEARPROC%]", item.Stats.WeaponOnHitFearProcChance);
            itemDic.Add("[FREEZEPROC%]", item.Stats.WeaponOnHitFreezeProcChance);
            itemDic.Add("[IMMOPROC%]", item.Stats.WeaponOnHitImmobilizeProcChance);
            itemDic.Add("[KNOCKPROC%]", item.Stats.WeaponOnHitKnockbackProcChance);
            itemDic.Add("[BLEEDPROC%]", item.Stats.WeaponOnHitBleedProcChance);

            itemDic.Add("[AREADMGPROC%]", item.Stats.OnHitAreaDamageProcChance);

            itemDic.Add("[CDRED%]", item.Stats.PowerCooldownReductionPercent);
            itemDic.Add("[RESRED%]", item.Stats.ResourceCostReductionPercent);

            itemDic.Add("[FIREDMG%]", item.Stats.FireSkillDamagePercentBonus);
            itemDic.Add("[LIGHTNINGDMG%]", item.Stats.LightningSkillDamagePercentBonus);
            itemDic.Add("[COLDDMG%]", item.Stats.ColdSkillDamagePercentBonus);
            itemDic.Add("[POISONDMG%]", item.Stats.PosionSkillDamagePercentBonus);
            itemDic.Add("[ARCANEDMG%]", item.Stats.ArcaneSkillDamagePercentBonus);
            itemDic.Add("[HOLYDMG%]", item.Stats.HolySkillDamagePercentBonus);

            itemDic.Add("[PHYSDMG%]", item.Stats.PhysicalSkillDamagePercentBonus);

            itemDic.Add("[ELEMDMG%]", new float[] { item.Stats.FireSkillDamagePercentBonus,
                                                    item.Stats.LightningSkillDamagePercentBonus,
                                                    item.Stats.ColdSkillDamagePercentBonus,
                                                    item.Stats.PosionSkillDamagePercentBonus,
                                                    item.Stats.ArcaneSkillDamagePercentBonus,
                                                    item.Stats.HolySkillDamagePercentBonus,
                                                    item.Stats.PhysicalSkillDamagePercentBonus }.Max());

            itemDic.Add("[SKILLDMG%]", ItemDataUtils.GetSkillDamagePercent(item));

             float damage, healing, toughness;
             item.GetStatChanges(out damage, out healing, out toughness);
             itemDic.Add("[UPDMG]", damage);
             itemDic.Add("[UPHEAL]", healing);
             itemDic.Add("[UPTOUGH]", toughness);

            // - NEW STATS ADDED --------------------------------------------------//
            itemDic.Add("[LEVELRED]", (float)item.Stats.ItemLevelRequirementReduction);
            itemDic.Add("[TOTBLOCK%]", item.Stats.BlockChance);
            itemDic.Add("[DMGVSELITE%]", item.Stats.DamagePercentBonusVsElites);
            itemDic.Add("[DMGREDELITE%]", item.Stats.DamagePercentReductionFromElites);
            itemDic.Add("[EXPBONUS]", item.Stats.ExperienceBonus);
            itemDic.Add("[REQLEVEL]", (float)item.Stats.RequiredLevel);
            itemDic.Add("[WEAPDMG%]", item.Stats.WeaponDamagePercent);

            itemDic.Add("[MAXSTAT]", new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max());
            itemDic.Add("[MAXSTATVIT]", new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max() + item.Stats.Vitality);
            itemDic.Add("[STRVIT]", item.Stats.Strength + item.Stats.Vitality);
            itemDic.Add("[DEXVIT]", item.Stats.Dexterity + item.Stats.Vitality);
            itemDic.Add("[INTVIT]", item.Stats.Intelligence + item.Stats.Vitality);
            itemDic.Add("[MAXONERES]", new float[] { item.Stats.ResistArcane, item.Stats.ResistCold, item.Stats.ResistFire, item.Stats.ResistHoly, item.Stats.ResistLightning, item.Stats.ResistPhysical, item.Stats.ResistPoison }.Max());
            itemDic.Add("[TOTRES]", item.Stats.ResistArcane + item.Stats.ResistCold + item.Stats.ResistFire + item.Stats.ResistHoly + item.Stats.ResistLightning + item.Stats.ResistPhysical + item.Stats.ResistPoison + item.Stats.ResistAll);
            itemDic.Add("[DMGFACTOR]", item.Stats.AttackSpeedPercent + item.Stats.CritPercent * 2 + item.Stats.CritDamagePercent / 5 + (item.Stats.MinDamage + item.Stats.MaxDamage) / 20);
            itemDic.Add("[AVGDMG]", (item.Stats.MinDamage + item.Stats.MaxDamage) / 2);

            float offstats = 0;
            //if (new float[] { item.Stats.Strength, item.Stats.Intelligence, item.Stats.Dexterity }.Max() > 0)
            //    offstats += 1;
            if (item.Stats.CritPercent > 0)
                offstats += 1;
            if (item.Stats.CritDamagePercent > 0)
                offstats += 1;
            if (item.Stats.AttackSpeedPercent > 0)
                offstats += 1;
            if (item.Stats.MinDamage + item.Stats.MaxDamage > 0)
                offstats += 1;
            itemDic.Add("[OFFSTATS]", offstats);

            float defstats = 0;
            //if (item.Stats.Vitality > 0)
            defstats += 1;
            if (item.Stats.ResistAll > 0)
                defstats += 1;
            if (item.Stats.ArmorBonus > 0)
                defstats += 1;
            if (item.Stats.BlockChance > 0)
                defstats += 1;
            if (item.Stats.LifePercent > 0)
                defstats += 1;
            //if (item.Stats.HealthPerSecond > 0)
            //    defstats += 1;
            itemDic.Add("[DEFSTATS]", defstats);
            itemDic.Add("[WEIGHTS]", WeightSet.CurrentWeightSet.EvaluateItem(item));

            //itemDic.Add("[GAMEBALANCEID]", (float)item.GameBalanceId);
            //itemDic.Add("[DYNAMICID]", item.AnnId);

            // starting on macro implementation here
            foreach (string key in macroDic.Keys)
            {
                ParseErrors parseErrors = null;
                string expr = macroDic[key];
                try
                {
                    object exprValue = evaluateExpr(expr, out parseErrors);
                    itemDic.Add("[" + key + "]", exprValue);
                }
                catch (Exception e)
                {
                    logOut(e.Message
                        + SEP + "last use rule: " + expr
                        + SEP + getParseErrors(parseErrors)
                        + SEP + getFullItem()
                        + SEP + e.ToString()
                        , InterpreterAction.NULL, LogType.ERROR);
                }
            }

            // end macro implementation
        }

        private void checkItemForMissingTranslation(ACDItem item)
        {
            string balanceIDstr;
            if (!nameToBalanceId.TryGetValue(item.Name.Replace(" ", ""), out balanceIDstr) && !nameToBalanceId.ContainsValue(item.GameBalanceId.ToString()))
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Translation: Missing: " + item.GameBalanceId.ToString() + ";" + item.Name + " (ID is missing report)");
                // not found missing name
                StreamWriter transFix = new StreamWriter(Path.Combine(FileManager.LoggingPath, tranLogFile), true);
                transFix.WriteLine("Missing: " + item.GameBalanceId.ToString() + ";" + item.Name);
                transFix.Close();
            }
            else if (balanceIDstr != item.GameBalanceId.ToString())
            {
                Logger.Log(TrinityLogLevel.Info, LogCategory.UserInformation, "Translation: Wrong(" + balanceIDstr + "): " + item.GameBalanceId.ToString() + ";" + item.Name);
                // wrong reference
                StreamWriter transFix = new StreamWriter(Path.Combine(FileManager.LoggingPath, tranLogFile), true);
                transFix.WriteLine("Wrong(" + balanceIDstr + "): " + item.GameBalanceId.ToString() + ";" + item.Name);
                transFix.Close();
            }
        }
    }

    #endregion Interpreter
}
