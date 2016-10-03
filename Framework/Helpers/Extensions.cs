using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Items;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Class Extensions.
    /// </summary>
    public static class Extensions
    {


        public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dic, Func<TValue, bool> predicate)
        {
            var keys = dic.Keys.Where(k => predicate(dic[k])).ToList();
            foreach (var key in keys)
            {
                dic.Remove(key);
            }
        }

        public static void AddRangeOverride<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic[x.Key] = x.Value);
        }

        public static void AddRangeNewOnly<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => { if (!dic.ContainsKey(x.Key)) dic.Add(x.Key, x.Value); });
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
        }

        public static bool ContainsKeys<TKey, TValue>(this Dictionary<TKey, TValue> dic, IEnumerable<TKey> keys)
        {
            bool result = false;
            keys.ForEachOrBreak((x) => { result = dic.ContainsKey(x); return result; });
            return result;
        }

        //public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        //{
        //    foreach (var item in source)
        //        action(item);
        //}

        public static void ForEachOrBreak<T>(this IEnumerable<T> source, Func<T, bool> func)
        {
            foreach (var item in source)
            {
                bool result = func(item);
                if (result) break;
            }
        }

        public static Queue<T> AddRange<T>(this Queue<T> queue, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                queue.Enqueue(item);
            }
            return queue;
        }

        public static Queue<T> InsertItem<T>(this Queue<T> queue, T item)
        {
            var transfer = queue.ToArray();
            queue.Clear();
            queue.Enqueue(item);
            queue.AddRange(transfer);
            return queue;
        }




        public static void Sort<TSource, TKey>(this ObservableCollection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
            {
                source.Add(sortedItem);
            }
        }


        /// <summary>
        /// Fetches value from Dictionary or adds and returns a default value.
        /// </summary>
        internal static TV GetOrCreateValue<TK, TV>(this Dictionary<TK, TV> dictionary, TK key, TV newValue = default(TV))
        {
            if (key == null)
                throw new ArgumentNullException("key");

            TV foundValue;
            if (dictionary.TryGetValue(key, out foundValue))
                return foundValue;

            if (newValue == null)
                newValue = (TV)Activator.CreateInstance(typeof(TV));

            dictionary.Add(key, newValue);
            return newValue;
        }

        /// <summary>
        /// Allows a nullable backed property and use _field.GetValueOrDefaultAttribute() for [DefaultValue(1)] attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <returns>T.</returns>
        public static T GetValueOrDefaultAttribute<T>(this T? obj, [CallerMemberName] string name = "", Type type = null) where T : struct, IComparable
        {
            if (obj.HasValue)
                return obj.Value;

            if (type == null)
            {
                var frame = new StackFrame(1);
                var method = frame.GetMethod();
                if (method.DeclaringType != null)
                {
                    type = method.DeclaringType;
                }
                else
                {
                    return default(T);
                }
            }

            var properties = TypeDescriptor.GetProperties(type)[name];
            if (properties != null)
            {
                var myAttribute = properties.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                if (myAttribute != null)
                {
                    return (T)Convert.ChangeType(myAttribute.Value, typeof(T));
                }
            }
            return default(T);
        }

        /// <summary>
        /// Gets a dictionary value or the default
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>TValue.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }

        /// <summary>
        /// Get an attribute, exceptions get swallowed and default returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actor">The actor.</param>
        /// <param name="type">The type.</param>
        /// <returns>T.</returns>
        public static T GetAttributeOrDefault<T>(this TrinityActor actor, ActorAttributeType type) where T : struct
        {
            try
            {
                actor.Attributes.GetAttribute<T>(type);
            }
            catch (Exception ex)
            {
                Logger.LogDebug(LogCategory.CacheManagement, "Exception on {0} accessing {1} attribute: {2}", actor.InternalName, type, ex);
            }
            return default(T);
        }

        /// <summary>
        /// Get an attribute, exceptions get swallowed and default returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actorACD">The actor acd.</param>
        /// <param name="type">The type.</param>
        /// <returns>T.</returns>
        public static T GetAttributeOrDefault<T>(this ACD actorACD, ActorAttributeType type) where T : struct
        {
            try
            {
                actorACD.GetAttribute<T>(type);
            }
            catch (Exception ex)
            {
                Logger.LogDebug(LogCategory.CacheManagement, "Exception on {0} accessing {1} attribute: {2}", actorACD.Name, type, ex);
            }
            return default(T);
        }

        /// <summary>
        /// Gets the trinity item quality.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>TrinityItemQuality.</returns>
        public static TrinityItemQuality GetTrinityItemQuality(this ACDItem item)
        {
            if (item == null)
                return TrinityItemQuality.None;
            if (!item.IsValid)
                return TrinityItemQuality.None;

            var itemQuality = item.GetItemQuality();

            switch (itemQuality)
            {
                case ItemQuality.Invalid:
                    return TrinityItemQuality.None;
                case ItemQuality.Inferior:
                case ItemQuality.Normal:
                case ItemQuality.Superior:
                    return TrinityItemQuality.Common;
                case ItemQuality.Magic1:
                case ItemQuality.Magic2:
                case ItemQuality.Magic3:
                    return TrinityItemQuality.Magic;
                case ItemQuality.Rare4:
                case ItemQuality.Rare5:
                case ItemQuality.Rare6:
                    return TrinityItemQuality.Rare;
                case ItemQuality.Legendary:
                case ItemQuality.Special:
                default:
                    return TrinityItemQuality.Legendary;

            }
        }

        /// <summary>
        /// To the enum value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the t enum.</typeparam>
        /// <param name="e">The e.</param>
        /// <returns>EnumValue&lt;TEnum&gt;.</returns>
        /// <exception cref="System.ArgumentException">T must be an enumerated type</exception>
        public static EnumValue<TEnum> ToEnumValue<TEnum>(this TEnum e) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return new EnumValue<TEnum>(e);
        }

        /// <summary>
        /// The _item quality regex
        /// </summary>
        private static Regex _itemQualityRegex = new Regex("{c:[a-zA-Z0-9]{8}}", RegexOptions.Compiled);

        /// <summary>
        /// Items the link color quality.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>ItemQuality.</returns>
        public static ItemQuality GetItemQuality(this ACDItem item)
        {
            if (item == null)
                return ItemQuality.Invalid;
            if (!item.IsValid)
                return ItemQuality.Invalid;

            /*
            {c:ff00ff00} = Set
            {c:ffff8000} = Legendary
            {c:ffffff00} = Rare
            {c:ff6969ff} = Magic
             */

            string itemLink = item.ItemLink;

            string linkColor = _itemQualityRegex.Match(itemLink).Value;

            ItemQuality qualityResult;

            switch (linkColor)
            {
                case "{c:ff00ff00}": // Green
                    qualityResult = ItemQuality.Legendary;
                    break;
                case "{c:ffff8000}": // Orange
                    qualityResult = ItemQuality.Legendary;
                    break;
                case "{c:ffffff00}": // Yellow
                    qualityResult = ItemQuality.Rare4;
                    break;
                case "{c:ff6969ff}": // Magic Blue 
                    qualityResult = ItemQuality.Magic1;
                    break;
                case "{c:ffffffff}": // White
                    qualityResult = ItemQuality.Normal;
                    break;
                // got this off a "lore book" - not sure what it actually equates to
                case "{c:ff99bbff}": // Gem Blue
                    qualityResult = ItemQuality.Normal;
                    break;
                case "{c:ffc236ff}": // Purple
                    qualityResult = ItemQuality.Special;
                    break;
                case "{c:ff888888}": // Gray
                    qualityResult = ItemQuality.Inferior;
                    break;
                case "{c:ff6cd8bb}": // Unique
                    qualityResult = ItemQuality.Rare4;
                    break;
                case "":
                    qualityResult = item.ItemQualityLevel;
                    break;
                default:
                    Logger.Log("Invalid Item Link color={0} internalName={1} name={2} gameBalanceId={3}", linkColor, item.InternalName, item.Name, item.GameBalanceId);
                    qualityResult = item.ItemQualityLevel;
                    break;
            }

            return qualityResult;
        }

        /// <summary>
        /// Determines whether [is set item] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [is set item] [the specified item]; otherwise, <c>false</c>.</returns>
        public static bool IsSetItem(this ACDItem item)
        {
            if (item == null)
                return false;
            if (!item.IsValid)
                return false;

            string itemLink = item.ItemLink;

            string linkColor = _itemQualityRegex.Match(itemLink).Value;

            if (linkColor == "{c:ff00ff00}")
                return true;

            return false;
        }

        /// <summary>
        /// Items the name of the set.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.String.</returns>
        public static string ItemSetName(this ACDItem item)
        {
            if (!item.IsSetItem())
                return null;

            var set = Sets.Where(s => s.ItemIds.Contains(item.ActorSnoId)).FirstOrDefault();
            if (set != null)
                return set.Name;

            return null;
        }

        /// <summary>
        /// Gets the gem quality level.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public static int GetGemQualityLevel(this ACDItem item)
        {
            if (item == null)
                return 0;
            if (!item.IsValid)
                return 0;

            // Imperial Gem hax
            if (item.InternalName.EndsWith("_16"))
                return 68;

            return item.Level;
        }
        /// <summary>
        /// Gets the size of the nav cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns>System.Double.</returns>
        public static double GetNavCellSize(this NavCell cell)
        {
            var diff = cell.Max.ToVector2() - cell.Min.ToVector2();
            return diff.X * diff.Y;
        }

        /// <summary>
        /// Returns if a DiaObject is not null, is valid, and it's ACD is not null, and is valid
        /// </summary>
        /// <param name="diaObject">The dia object.</param>
        /// <returns><c>true</c> if [is fully valid] [the specified dia object]; otherwise, <c>false</c>.</returns>
        public static bool IsFullyValid(this DiaObject diaObject)
        {
            return diaObject != null && diaObject.IsValid && diaObject.CommonData != null && diaObject.CommonData.IsValid;
        }

        /// <summary>
        /// Removed duplicates from a list based on specified property .DistinctBy(o =&gt; o.property)
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns>IEnumerable&lt;TSource&gt;.</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// Splits a StringLikeThisWithCapitalLetters into words with spaces between.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="preserveAcronyms">if set to <c>true</c> [preserve acronyms].</param>
        /// <returns>System.String.</returns>
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }


        public static TrinityItemType GetTrinityItemType(this ACDItem item)
        {
            return TypeConversions.DetermineItemType(item.InternalName, item.ItemType);
        }

        public static ItemSelectionType GetItemSelectionType(this ACDItem item)
        {
            ItemSelectionType result;
            return Enum.TryParse(item.GetTrinityItemType().ToString(), out result) ? result : ItemSelectionType.Unknown;
        }

        public static ItemSelectionType ToItemSelectionType(this TrinityItemType trinityItemType)
        {
            ItemSelectionType result;
            return Enum.TryParse(trinityItemType.ToString(), out result) ? result : ItemSelectionType.Unknown;
        }

        /// <summary>
        /// Gets the item stack quantity.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public static int GetItemStackQuantity(this ACDItem item)
        {
            return (item.GetAttribute<int>(ActorAttributeType.ItemStackQuantityHi) << 32) | item.GetAttribute<int>(ActorAttributeType.ItemStackQuantityLo);
        }

        internal static object ReflectInstanceField(this object instance, string fieldName)
        {
            var type = instance.GetType();

            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            FieldInfo field = type.GetField(fieldName, bindFlags);
            if (field != null)
                return field.GetValue(instance);

            return null;
        }
    }
}


