using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals;

namespace Trinity.Framework.Helpers
{
    public static class MemoryHelper
    {
        private static readonly Type ZetaDiaType = typeof(ZetaDia);

        public static int GameBalanceItemHash(string input)
        {
            input = input.ToLowerInvariant();
            uint hash = 0;
            for (int i = 0; i < input.Length; i++)
                hash = (hash << 5) + hash + input[i];
            return unchecked((int)hash);
        }

        public static int GameBalanceNormalHash(string input)
        {
            int hash = 0;
            for (int i = 0; i < input.Length; ++i)
                hash = (hash << 5) + hash + input[i];
            return hash;
        }


        /// <summary>
        /// Strip zero value byte off byte array
        /// </summary>
        public static byte[] GetMinByteArray(byte[] bytes)
        {
            var i = bytes.Length - 1;
            while (bytes[i] == 0) --i;
            var minArr = new byte[i + 1];
            Array.Copy(bytes, minArr, i + 1);
            return minArr;
        }

        public static int GetOffetOfValue<T>(IntPtr baseAddress, T valueToFind)
        {
            for (int i = 0; i < 2000; i++)
            {
                try
                {
                    //var val = ZetaDia.Memory.Read<T>(baseAddress + i);
                    var val = Reader.Read<T>(baseAddress + i);
                    if (val.Equals(valueToFind))
                    {
                        return i;
                    }
                }
                catch (Exception)
                {
                    //Logger.Log($"Exception at {i}");
                }
            }
            return -1;
        }

        /// <summary>
        /// Memory reading adapter for GreyMagic.
        /// Slower but useful when T is a reflected value and so cannot satisfy the GreyMagic struct constraint.
        /// </summary>
        public static class Reader
        {
            abstract class GenericRead<T>
            {
                public abstract T DoStuff(IntPtr ptr);
            }

            class ValueTypeReadHelper<T> : GenericRead<T> where T : struct
            {
                public override T DoStuff(IntPtr ptr)
                {
                    return ValueRead<T>(ptr);
                }
            }
            static T ValueRead<T>(IntPtr ptr) where T : struct
            {
                return ZetaDia.Memory.Read<T>(ptr);
            }

            class RefTypeReadHelper<T> : GenericRead<T> where T : class
            {
                public override T DoStuff(IntPtr ptr)
                {
                    return RefRead<T>(ptr);
                }
            }

            static T RefRead<T>(IntPtr ptr)
            {
                return default(T);
            }

            public static T Read<T>(IntPtr ptr)
            {
                var helperType = typeof(T).IsValueType ? typeof(ValueTypeReadHelper<>) : typeof(RefTypeReadHelper<>);
                helperType = helperType.MakeGenericType(typeof(T));
                var helper = (GenericRead<T>)Activator.CreateInstance(helperType);
                return helper.DoStuff(ptr);
            }
        }

        static readonly Dictionary<Type, Func<int, IntPtr>> GetRecordPtrMethods = new Dictionary<Type, Func<int, IntPtr>>();

        /// <summary>
        /// Call private method GetRecordPtr on SNOTable instance
        /// GetRecordPtr() finds a record pointer in a table for given value
        /// e.g. var testPtr = ZetaDia.SNO[ClientSNOTable.ActorInfo].GetRecordPtr(ZetaDia.Me.ActorSnoId);
        /// </summary>
        public static IntPtr GetRecordPtr(this SNOTable table, int id)
        {
            var type = typeof(SNOTable);
            Func<int, IntPtr> expr;

            if (!GetRecordPtrMethods.TryGetValue(type, out expr))
            {
                // Get all delcared private methods
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                // GetRecordPtr is obfusticated with no name so find a method with the right pattern of args
                var method = methods.FirstOrDefault(m => m.ReturnType == typeof(IntPtr));

                if (method == null)
                    throw new NullReferenceException("GetRecordPtr MethodInfo cannot be null");

                // Define that expression will take an Int argument
                var parameterExpr = Expression.Parameter(typeof(int), "input");

                // Define instance that MethodInfo will be executed against.
                var instanceExpr = Expression.Constant(table);

                // Formalize instance, method and arguments.
                var methodCallExpr = Expression.Call(instanceExpr, method, parameterExpr);

                expr = Expression.Lambda<Func<int, IntPtr>>(methodCallExpr, parameterExpr).Compile();

                GetRecordPtrMethods.Add(type, expr);
            }

            return expr != null ? expr(id) : new IntPtr(-1);
        }

        static readonly Dictionary<Type, Action<IntPtr>> PurgeSNORecordPtrMethods = new Dictionary<Type, Action<IntPtr>>();

        /// <summary>
        /// Call private method GetRecordPtr on SNOTable instance.
        /// Apparently bad things ensue if you don't purge the record after using it
        /// </summary>
        public static void PurgeRecordPtr(this SNOTable table, IntPtr ptr)
        {
            var type = typeof(SNOTable);
            Action<IntPtr> expr;

            if (!PurgeSNORecordPtrMethods.TryGetValue(type, out expr))
            {
                // Get all delcared private methods
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                // PurgeSNORecord is obfusticated with no name so find a method with the right pattern of args
                var method = methods.FirstOrDefault(m => m.ReturnType == typeof(void));

                if (method == null)
                    throw new NullReferenceException("GetRecordPtr MethodInfo cannot be null");

                // Define that expression will take an Int argument
                var parameterExpr = Expression.Parameter(typeof(IntPtr), "input");

                // Define instance that MethodInfo will be executed against.
                var instanceExpr = Expression.Constant(table);

                // Formalize instance, method and arguments.
                var methodCallExpr = Expression.Call(instanceExpr, method, parameterExpr);

                expr = Expression.Lambda<Action<IntPtr>>(methodCallExpr, parameterExpr).Compile();

                PurgeSNORecordPtrMethods.Add(type, expr);
            }

            if (expr != null)
                expr(ptr);
        }


    }
}

