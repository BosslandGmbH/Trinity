using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public interface IMemoryWrapper
    {
        IntPtr BaseAddress { get; }
    }

    public class MemoryWrapper
    {
        public IntPtr BaseAddress { get; private set; }

        public IntPtr BaseSerializationAddress { get; private set; }

        public virtual bool IsValid => BaseAddress != IntPtr.Zero;

        public IntPtr ParentAddress { get; private set; }

        public MemoryWrapper()
        {
        }

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

        protected T ReadObject<T>(int offset, IntPtr serializeBaseAddress = default(IntPtr)) where T : MemoryWrapper, new()
        {
            if (!IsValid)
            {
                return default(T);
            }
            try
            {
                var item = Create<T>(BaseAddress + offset);
                item.ParentAddress = BaseAddress;
                if (serializeBaseAddress != default(IntPtr))
                    item.BaseSerializationAddress = serializeBaseAddress;
                return item;
            }
            catch (Exception ex)
            {
                Core.Logger.Log($"Memory ReadObject Exception. {ex.ToLogString(Environment.StackTrace)}");
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
                return Create<T>(address);
            }
            catch (Exception ex)
            {
                Core.Logger.Log($"Memory ReadAbsoluteObject Exception. {ex.ToLogString(Environment.StackTrace)}");
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
                Core.Logger.Log($"Memory ReadOffset Exception. {ex.ToLogString(Environment.StackTrace)}");
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
                Core.Logger.Log($"Memory ReadArray Exception. {ex.ToLogString(Environment.StackTrace)}");
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
                Core.Logger.Log($"Memory ReadArray Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T[]);
        }

        public List<T> ReadObjects<T>(int offset, int count, IntPtr serializeBaseAddress) where T : MemoryWrapper, new()
        {
            var size = typeof(T).SizeOf();
            if (size <= 0)
                return null;

            var results = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = Create<T>(BaseAddress + offset + i * size);
                item.BaseSerializationAddress = serializeBaseAddress;
                item.ParentAddress = BaseAddress;
                results.Add(item);
            }
            return results;
        }

        public List<T> ReadObjects<T>(int offset, int count) where T : MemoryWrapper, new()
        {
            var size = typeof(T).SizeOf();
            if (size <= 0)
                return null;

            var results = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = Create<T>(BaseAddress + offset + i * size);
                item.ParentAddress = BaseAddress;
                results.Add(item);
            }
            return results;
        }

        public List<T> ReadObjects<T>(int offset, Predicate<T> validCondition) where T : MemoryWrapper, new()
        {
            var size = typeof(T).SizeOf();
            if (size <= 0)
                return null;

            var results = new List<T>();
            var count = 0;
            while (true)
            {
                var item = Create<T>(BaseAddress + offset + count * size);
                if (item == null)
                    break;

                if (!validCondition(item))
                    continue;

                results.Add(item);
                count++;
            }
            return results;
        }

        public List<T> ReadObjects<T>(int offset, Predicate<T> stopCondition, Func<T, T> creatorFunc) where T : MemoryWrapper, new()
        {
            var size = typeof(T).SizeOf();
            if (size <= 0)
                return null;

            var results = new List<T>();
            var count = 0;
            while (true)
            {
                var baseItem = Create<T>(BaseAddress + offset + count * size);
                if (stopCondition(baseItem))
                    break;

                var derivedItem = creatorFunc(baseItem);
                results.Add(derivedItem);
                count++;
            }
            return results;
        }

        public T ReadObject<T>(int offset, Func<T, T> creatorFunc) where T : MemoryWrapper, new()
        {
            if (!IsValid)
                return null;

            var baseItem = Create<T>(BaseAddress + offset);
            return creatorFunc(baseItem);
        }

        public List<T> ReadObjects<T>(int offset, int count, Predicate<T> validCondition) where T : MemoryWrapper, new()
        {
            var size = typeof(T).SizeOf();
            if (size <= 0)
                return null;

            var results = new List<T>();
            for (var i = 0; i < count; i++)
            {
                var item = Create<T>(BaseAddress + offset + i * size);
                item.ParentAddress = BaseAddress;

                if (!validCondition(item))
                    continue;

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
                Core.Logger.Log($"Memory ReadString Exception. {ex.ToLogString(Environment.StackTrace)}");
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
                Core.Logger.Log($"Memory ReadString Exception. {ex.ToLogString(Environment.StackTrace)}");
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
                Core.Logger.Log($"Memory ReadSerializedData Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T[]);
        }

        //protected List<T> ReadSerializedObjects<T>(int offset, int dataOffset) where T : MemoryWrapper, new()
        //{
        //    var data = ReadObject<NativeSerializeData>(dataOffset);
        //    var size = typeof(T).SizeOf();
        //    var count = data.Length / size;
        //    return ReadObjects<T>(data.FirstEntry, count, size).ToList();
        //}

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
                Core.Logger.Log($"Memory Read Exception. {ex} {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T);
        }

        protected virtual void OnUpdated()
        {
        }

        public T ReadPointer<T>(IntPtr address) where T : MemoryWrapper, new()
        {
            if (!IsValid)
                return default(T);

            var ptr = Read<IntPtr>(address);
            return ReadAbsoluteObject<T>(ptr);
        }

        public T ReadPointer<T>(int offset) where T : MemoryWrapper, new()
        {
            if (!IsValid)
                return default(T);

            var ptr = ReadOffset<IntPtr>(offset);
            return ReadAbsoluteObject<T>(ptr);
        }

        public static T Create<T>(IntPtr ptr) where T : MemoryWrapper, new()
        {
            if (ptr == IntPtr.Zero)
                return default(T);

            var item = new T();
            item.Update(ptr);
            return item;
        }

        public T Cast<T>() where T : MemoryWrapper, new() => Create<T>(BaseAddress);
    }

    //public struct NativeSerializeData
    //{
    //    public int Offset;
    //    public int Length;
    //}
}