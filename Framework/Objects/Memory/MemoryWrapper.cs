using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Buddy.Auth.Math;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Items;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class MemoryWrapper
    {
        public IntPtr BaseAddress { get; private set; }

        public SerializeData BaseSeralizationInfo { get; private set; }

        public IntPtr BaseSerializationAddress { get; private set; }

        public bool IsValid => BaseAddress != IntPtr.Zero;

        public MemoryWrapper() { }

        public MemoryWrapper(IntPtr ptr)
        {
            Update(ptr);
        }

        public void Update()
        {
            OnUpdated();
        }

        public void Update(IntPtr ptr)
        {
            BaseAddress = ptr;
            OnUpdated();
        }

        protected T ReadObject<T>(int offset) where T : MemoryWrapper, new()
        {
            if (!IsValid)
            {
                return default(T);
            }
            try
            {
                var type = typeof(T);
                if (type == typeof(MemoryWrapper) || type.IsSubclassOf(typeof(MemoryWrapper)))
                {
                    return Create<T>(BaseAddress + offset);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadObject Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T);
        }

        protected T ReadAbsoluteObject<T>(IntPtr address) where T : MemoryWrapper, new()
        {
            if (!IsValid)
            {
                return default(T);
            }
            try
            {
                var type = typeof(T);
                if (type == typeof(MemoryWrapper) || type.IsSubclassOf(typeof(MemoryWrapper)))
                {
                    return Create<T>(address);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadAbsoluteObject Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T);
        }

        public virtual T ReadOffset<T>(int offset) where T : struct
        {
            if (!IsValid)
            {
                return default(T);
            }
            try
            {
                return ZetaDia.Memory.Read<T>(BaseAddress + offset);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadOffset Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T);
        }
        protected List<T> ReadArray<T>(int offset, int count) where T : struct
        {
            if (!IsValid)
            {
                return new List<T>();
            }
            try
            {
                return ZetaDia.Memory.ReadArray<T>(BaseAddress + offset, count).ToList();
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadArray Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return new List<T>();
        }

        public static T[] ReadArray<T>(IntPtr address, int count) where T : struct
        {
            try
            {
                return ZetaDia.Memory.ReadArray<T>(address, count);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadArray Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T[]);
        }

        public List<T> ReadObjects<T>(int offset, int count) where T : MemoryWrapper, new()
        {
            var size = typeof (T).SizeOf();
            if (size <= 0)
            {
                Logger.LogError("ReadObjects<T>(int offset, int count) requires a SizeOf field");
                return null;
            }     
            var results = new List<T>();       
            for (var i = 0; i < count; i++)
            {
                results.Add(Create<T>(BaseAddress + offset + i * size));
            }
            return results;
        }

        public List<T> ReadObjects<T>(int offset, Predicate<T> invalidItemCondition) where T : MemoryWrapper, new()
        {
            var size = typeof (T).SizeOf();
            if (size <= 0)
            {
                Logger.LogError("ReadObjects<T>(int offset, int count) requires a SizeOf field");
                return null;
            }     
            var results = new List<T>();       
            for (var i = 0; i < 255; i++)
            {
                var item = Create<T>(BaseAddress + offset + i*size);
                if (invalidItemCondition(item))
                    break;
                results.Add(item);
            }
            return results;
        }

        public IEnumerable<T> ReadObjects<T>(int offset, int count, int blockSize) where T : MemoryWrapper, new()
        {
            for (var i = 0; i < count; i++)
            {
                yield return Create<T>(BaseAddress + offset + i * blockSize);
            }
        }

        public static IEnumerable<T> ReadObjects<T>(IntPtr address, int count, int blockSize) where T : MemoryWrapper, new()
        {
            for (var i = 0; i < count; i++)
            {
                yield return Create<T>(address + i * blockSize);
            }
        }

        public static IEnumerable<T> ReadArray<T>(IntPtr address, int count, int blockSize) where T : struct
        {
            for (var i = 0; i < count; i++)
            {
                yield return ZetaDia.Memory.Read<T>(address + i * blockSize);
            }
        }

        protected string ReadSerializedString(int offset, int serializeDataOffset)
        {
            var serializeData = ReadOffset<SerializeData>(serializeDataOffset);
            return ReadSerializedString(offset, serializeData);
        }

        protected string ReadSerializedString(int offset, SerializeData serializeData)
        {
            if (!IsValid)
            {
                return string.Empty;
            }
            try
            {
                return Encoding.UTF8.GetString(ZetaDia.Memory.ReadBytes(BaseSerializationAddress + serializeData.Offset, serializeData.Length)).Trim('\0');
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadString Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return string.Empty;
        }

        protected string ReadStringPointer(int offset)
        {
            if (!IsValid)
            {
                return string.Empty;
            }
            try
            {
                return ZetaDia.Memory.ReadStringUTF8(ZetaDia.Memory.Read<IntPtr>(BaseAddress + offset));
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadString Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return string.Empty;
        }

        protected string ReadString(int offset, int length = 128)
        {
            if (!IsValid)
            {
                return string.Empty;
            }
            try
            {
                return ZetaDia.Memory.ReadStringUTF8(BaseAddress + offset);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadString Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return string.Empty;
        }

        public T[] ReadSerializedData<T>(int offset, int size) where T : struct
        {
            if (!IsValid)
            {
                return default(T[]);
            }
            try
            {
                if (offset == 0 || size == 0)
                    return new T[0];

                var num = Marshal.SizeOf(typeof(T));
                var count = size / num;

                return ZetaDia.Memory.ReadArray<T>(BaseAddress + offset, count);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadSerializedData Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T[]);
        }

        protected List<T> ReadSerializedObjects<T>(int offset, int serializeDataOffset) where T : MemoryWrapper, new()
        {
            var serializeData = ReadOffset<SerializeData>(serializeDataOffset);
            return ReadSerializedObjects<T>(offset, serializeData);
        }

        protected List<T> ReadSerializedObjects<T>(int offset, SerializeData serializeData) where T : MemoryWrapper, new()
        {
            if (!IsValid)
            {
                return null;
            }
            try
            {
                var type = typeof(T);
                if (type == typeof(MemoryWrapper) || type.IsSubclassOf(typeof(MemoryWrapper)))
                {
                    var size = type.SizeOf();
                    var count = serializeData.Length / size;
                    var container = new List<T>();
                    for (int i = 0; i < count; i++)
                    {
                        var item = Create<T>(BaseAddress + serializeData.Offset + i * size);
                        item.SetSerializationInfo(BaseAddress, serializeData);
                        container.Add(item);
                    }
                    return container;
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadSerializedObjects Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(List<T>);
        }

        protected T Read<T>(IntPtr address) where T : struct
        {
            if (address == IntPtr.Zero || (int)address < 10000)
            {
                return default(T);
            }
            try
            {
                return ZetaDia.Memory.Read<T>(address);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory Read Exception. {ex} {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T);
        }

        protected virtual void OnUpdated()
        {

        }

        public T ReadPointer<T>(int offset) where T : MemoryWrapper, new()
        {
            var ptr = ReadOffset<IntPtr>(offset);
            return ReadAbsoluteObject<T>(ptr);
        }

        public void SetSerializationInfo(IntPtr address, SerializeData info)
        {
            BaseSerializationAddress = address;
            BaseSeralizationInfo = info;
        }

        public static T Create<T>(IntPtr ptr) where T : MemoryWrapper, new()
        {
            if (ptr == IntPtr.Zero)
                return default(T);

            var item = new T();
            item.Update(ptr);
            return item;
        }
    }

    public struct SerializeData
    {
        public int Offset;
        public int Length;
    }
}






