using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Trinity.Technicals;
using Zeta.Game.Internals;

namespace Trinity.Helpers
{
    public static class UIElementExtensions
    {
        public static UIElement FindDecedentWithText(this UIElement element, string text)
        {
            if (element == null || !element.IsValid || string.IsNullOrEmpty(text))
                return null;

            if (element.HasText && element.Text.Contains(text))
                return element;

            foreach (var child in UIElement.GetChildren(element))
            {
                var childResult = FindDecedentWithText(child, text);
                if (childResult != null)
                    return childResult;
            }

            return null;
        }

        public static List<UIElement> FindDecedentsWithText(this UIElement element, string text = "")
        {
            if (element == null || !element.IsValid || string.IsNullOrEmpty(text))
                return null;

            var elements = new List<UIElement>();

            if (element.HasText && (element.Text.Contains(text) || string.IsNullOrEmpty(text)))
                elements.Add(element);

            foreach (var child in UIElement.GetChildren(element))
            {
                var childResults = FindDecedentsWithText(child, text);
                if (childResults != null && childResults.Any())
                    elements.AddRange(childResults);
            }

            return elements;
        }

        public static List<UIElement> FindDecendentsWithName(this UIElement element, string text)
        {
            if (element == null || !element.IsValid || string.IsNullOrEmpty(text))
                return null;

            var elements = new List<UIElement>();

            var currentName = element.Name.Split('.').LastOrDefault();
            if (currentName != null && currentName.Contains(text))
            {
                elements.Add(element);
            }   

            foreach (var child in UIElement.GetChildren(element))
            {
                var childResullts = FindDecendentsWithName(child, text);
                if (childResullts != null && childResullts.Any())
                    elements.AddRange(childResullts);
            }

            return elements;        
        }

        public static void DebugTestClickChildren(this UIElement element)
        {
            if (element == null || !element.IsValid)
                return;

            foreach (var child in UIElement.GetChildren(element))
            {
                var result = child.SafeClick();
                Logger.Log("Click {0} on {1} ({2})", result, child.Name, child.Hash);                
                Thread.Sleep(50);
            }
        }

        public static UIElement FindAncestorWithName(this UIElement element, string name, bool allowPartialMatch = false, bool caseSensitive = false)
        {
            if (element == null || !element.IsValid || string.IsNullOrEmpty(name))
                return null;

            var parts = element.Name.Split('.').ToList();

            if (!caseSensitive)
                name = name.ToLowerInvariant();

            for (int i = parts.Count()-1; i >= 0; i--)
            {
                var part = caseSensitive ? parts[i] : parts[i].ToLowerInvariant();

                if (allowPartialMatch)
                {
                    if (part.Contains(name))
                        break;
                }
                else
                {
                    if (part == name)
                        break;                    
                }

                parts.RemoveAt(i);
            }

            var path = string.Join(".",parts);

            return UIElement.FromName(path);
        }

        public static UIElement GetParent(this UIElement element)
        {
            var parts = element.Name.Split('.').ToList();
            parts.RemoveAt(parts.Count-1);
            var path = string.Join(".", parts);
            return UIElement.FromName(path);
        }

        public static List<UIElement> GetSiblings(this UIElement element, bool includeSelf = false)
        {
            var nodes = UIElement.GetChildren(element.GetParent());
            return nodes.Where(e => e.Hash != element.Hash).ToList();
        }

        public static List<UIElement> GetChildren(this UIElement element)
        {
            return UIElement.GetChildren(element).ToList();
        }

        public static void Log(this UIElement element)
        {
            if (element == null)
                return;

            Logger.Log("Element Name={1}, IsVisible={0}, Text={2}, Hash={3}, IsEnabled={4}, HasText={5}",
                element.IsVisible, element.Name, element.Text, element.Hash, element.IsEnabled, element.HasText);
        }

        public static bool SafeClick(this UIElement element)
        {
            if (element == null || element.IsValid)
                return false;

            return element.Click();
        }

    }
}
