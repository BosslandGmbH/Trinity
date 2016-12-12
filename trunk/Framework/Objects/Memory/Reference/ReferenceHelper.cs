//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Trinity.Framework.Objects.Memory.Reference
//{
//    public static class ReferenceHelper
//    {
//        public static string GenerateDictionary<TSource, TDestination>(
//            IEnumerable<TSource> source,
//            Func<TSource, object> keySelector, Dictionary<string,
//            Func<TSource, string>> propSelectors, 
//            string name = "")
//        {
//            var indent = "    ";
//            var sb = new StringBuilder();
//            var typeName = typeof(TDestination).Name;            
//            var propName = (string.IsNullOrEmpty(name) ? "Items" : name);
//            var keyType = keySelector(source.FirstOrDefault()).GetType();
//            sb.AppendLine($"public static Dictionary<{keyType.Name},{typeName}> {propName} {{ get; }} = new Dictionary<{keyType.Name},{typeName}>");
//            sb.AppendLine("{");
//            foreach (var entry in source)
//            {
//                sb.AppendLine($"{indent}{{{indent}{keySelector(entry)}, new {typeName}");
//                {
//                    sb.AppendLine($"{indent + indent}{{");
//                    foreach (var selectorPair in propSelectors)
//                    {
//                        sb.AppendLine($"{indent + indent + indent} {selectorPair.Key.Trim()} = {selectorPair.Value(entry).Trim()},");
//                    }
//                    sb.AppendLine($"{indent + indent}}}");
//                }
//                sb.AppendLine($"{indent}}},");
//            }
//            sb.AppendLine("};");
//            return sb.ToString();
//        }
//    }
//}


