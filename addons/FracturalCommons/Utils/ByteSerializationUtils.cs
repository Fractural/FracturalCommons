using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Fractural.Utils
{
    public interface IBufferSerializable
    {
        void Serialize(StreamPeerBuffer buffer);
        void Deserialize(StreamPeerBuffer buffer);
    }

    public static class ByteSerializationUtils
    {
        private static StreamPeerBuffer buffer = new StreamPeerBuffer();

        #region StreamPeerBuffer
        public static StreamPeerBuffer ToBuffer(this byte[] byteArray)
        {
            var buffer = new StreamPeerBuffer();
            buffer.DataArray = byteArray;
            return buffer;
        }

        public static void PutSerializable(this StreamPeerBuffer buffer, IBufferSerializable serializable)
        {
            serializable.Serialize(buffer);
        }

        public static T GetSerializable<T>(this StreamPeerBuffer buffer) where T : IBufferSerializable, new()
        {
            T inst = new T();
            inst.Deserialize(buffer);
            return inst;
        }

        public static T[] GetArray<T>(this StreamPeerBuffer buffer) where T : IBufferSerializable, new()
        {
            T[] array = new T[buffer.Get32()];
            for (int i = 0; i < array.Length; i++)
                array[i] = buffer.GetSerializable<T>();
            return array;
        }

        public static T[] GetPrimitiveArray<T>(this StreamPeerBuffer buffer) where T : struct
        {
            T[] array = new T[buffer.Get32()];
            for (int i = 0; i < array.Length; i++)
                array[i] = buffer.GetPrimitive<T>();
            return array;
        }


        public static T GetPrimitive<T>(this StreamPeerBuffer buffer) where T : struct
        {
            if (typeof(T) == typeof(bool))
                return (T)(object)(buffer.Get8() == 1);
            if (typeof(T) == typeof(short))
                return (T)(object)buffer.Get16();
            if (typeof(T) == typeof(int))
                return (T)(object)buffer.Get32();
            if (typeof(T) == typeof(long))
                return (T)(object)buffer.Get64();
            if (typeof(T) == typeof(ushort))
                return (T)(object)buffer.GetU16();
            if (typeof(T) == typeof(uint))
                return (T)(object)buffer.GetU32();
            if (typeof(T) == typeof(ulong))
                return (T)(object)buffer.GetU64();
            throw new System.Exception($"Cannot get primitive type <{typeof(T).Name}> from StreamPeerBuffer");
        }
        #endregion

        #region byte[] Serialization
        public static byte[] Serialize(this bool num)
        {
            buffer.Clear();
            buffer.PutU8(num ? (byte)1 : (byte)0);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this int num)
        {
            buffer.Clear();
            buffer.Put32(num);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this short num)
        {
            buffer.Clear();
            buffer.Put16(num);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this long num)
        {
            buffer.Clear();
            buffer.Put64(num);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this ushort num)
        {
            buffer.Clear();
            buffer.PutU16(num);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this uint num)
        {
            buffer.Clear();
            buffer.PutU32(num);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this ulong num)
        {
            buffer.PutU64(num);
            return buffer.DataArray;
        }

        public static byte[] Serialize(this IEnumerable<IBufferSerializable> serializableArray)
        {
            var buffer = new StreamPeerBuffer();
            buffer.Put32(serializableArray.Count());
            foreach (var serializable in serializableArray)
                buffer.PutData(serializable.Serialize());
            return buffer.DataArray;
        }

        public static byte[] Serialize(this IBufferSerializable serializable)
        {
            var buffer = new StreamPeerBuffer();
            serializable.Serialize(buffer);
            return buffer.DataArray;
        }

        public static T DeserializePrimitive<T>(this byte[] byteArray) where T : struct
        {
            buffer.DataArray = byteArray;
            return buffer.GetPrimitive<T>();
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : IBufferSerializable, new()
        {
            buffer.DataArray = byteArray;
            return buffer.GetSerializable<T>();
        }

        public static T[] DeserializeArray<T>(this byte[] byteArray) where T : IBufferSerializable, new()
        {
            buffer.DataArray = byteArray;
            return buffer.GetArray<T>();
        }

        public static T[] DeserializePrimitiveArray<T>(this byte[] byteArray) where T : struct
        {
            buffer.DataArray = byteArray;
            return buffer.GetPrimitiveArray<T>();
        }
        #endregion
    }
}