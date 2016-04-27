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
using Trinity.Objects.Native;
using Trinity.Technicals;
using Zeta.Game;

namespace Trinity.Framework.Objects.Memory
{
    public class MemoryWrapper
    {
        public IntPtr BaseAddress { get; private set; }

        public SerializationInfo BaseSeralizationInfo { get; private set; }

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
        protected T[] ReadArray<T>(int offset, int count) where T : struct
        {
            if (!IsValid)
            {
                return default(T[]);
            }
            try
            {
                return ZetaDia.Memory.ReadArray<T>(BaseAddress + offset, count);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory ReadArray Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T[]);
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

        protected string ReadString(int offset, SerializationInfo serializationInfo)
        {
            if (!IsValid)
            {
                return string.Empty;
            }
            try
            {
                return Encoding.UTF8.GetString(ZetaDia.Memory.ReadBytes(BaseSerializationAddress + serializationInfo.Offset, serializationInfo.Length)).Trim('\0');
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

        protected string ReadString(int offset, int length)
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

        protected List<T> ReadSerializedObjects<T>(int offset, SerializationInfo serializationInfo) where T : MemoryWrapper, new()
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
                    var count = serializationInfo.Length / size;
                    var container = new List<T>();
                    for (int i = 0; i < count; i++)
                    {
                        var item = Create<T>(BaseAddress + serializationInfo.Offset + i * size);
                        item.SetSerializationInfo(BaseAddress, serializationInfo);
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
            if (address == IntPtr.Zero)
            {
                return default(T);
            }
            try
            {
                return ZetaDia.Memory.Read<T>(address);
            }
            catch (Exception ex)
            {
                Logger.Log($"Memory Read Exception. {ex.ToLogString(Environment.StackTrace)}");
            }
            return default(T);
        }

        protected virtual void OnUpdated()
        {

        }

        //public static T Create<T>(IntPtr ptr) where T : new()
        //{
        //    return Create<T>(ptr, default(SerializationInfo));
        //}

        public T ReadPointer<T>(int offset) where T : MemoryWrapper, new()
        {
            var ptr = ReadOffset<IntPtr>(offset);
            return ReadAbsoluteObject<T>(ptr);
        }

        public void SetSerializationInfo(IntPtr address, SerializationInfo info)
        {
            BaseSerializationAddress = address;
            BaseSeralizationInfo = info;
        }

        public static T Create<T>(IntPtr ptr) where T : new()
        {
            if (ptr == IntPtr.Zero)
                return default(T);

            var item = new T();
            var memoryWrapper = item as MemoryWrapper;
            if (memoryWrapper == null)
            {
                return default(T);
            }
            memoryWrapper.Update(ptr);
            return item;
        }
    }

    public struct SerializationInfo
    {
        public int Offset;
        public int Length;
    }
}






