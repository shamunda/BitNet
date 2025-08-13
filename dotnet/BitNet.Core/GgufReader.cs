using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace BitNet.Core
{
    public class GgufReader
    {
        private const uint GGUF_MAGIC = 0x46554747; // "GGUF"

        private readonly string _path;

        public uint Version { get; private set; }
        public ulong TensorCount { get; private set; }
        public ulong KVCount { get; private set; }
        public Dictionary<string, object> Metadata { get; private set; } = new Dictionary<string, object>();
        public List<GgufTensorInfo> TensorInfos { get; private set; } = new List<GgufTensorInfo>();

        public GgufReader(string path)
        {
            _path = path;
        }

        public void Read()
        {
            using (var stream = File.OpenRead(_path))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    ReadHeader(reader);
                    ReadKeyValueMetadata(reader);
                    ReadTensorInfos(reader);
                }
            }
        }

        public byte[] ReadTensorData(GgufTensorInfo tensorInfo)
        {
            using (var stream = File.OpenRead(_path))
            {
                stream.Seek((long)tensorInfo.Offset, SeekOrigin.Begin);
                // This is a simplification. The actual size needs to be calculated based on the tensor's type and dimensions.
                // For now, I will assume the size is small enough to be read into a byte array.
                // I will need to implement the size calculation later.
                var size = 0;
                foreach (var dim in tensorInfo.Dims)
                {
                    size += (int)dim;
                }
                // Placeholder size
                size = 1024;
                return new BinaryReader(stream).ReadBytes(size);
            }
        }

        private void ReadHeader(BinaryReader reader)
        {
            var magic = reader.ReadUInt32();
            if (magic != GGUF_MAGIC)
            {
                throw new Exception("Invalid GGUF magic number");
            }

            Version = reader.ReadUInt32();
            if (Version != 3)
            {
                // Or handle other versions if necessary
                throw new Exception($"Unsupported GGUF version: {Version}");
            }

            TensorCount = reader.ReadUInt64();
            KVCount = reader.ReadUInt64();
        }

        private void ReadKeyValueMetadata(BinaryReader reader)
        {
            for (ulong i = 0; i < KVCount; i++)
            {
                var key = ReadString(reader);
                var valueType = (GGUFValueType)reader.ReadUInt32();
                var value = ReadValue(reader, valueType);
                Metadata[key] = value;
            }
        }

        private void ReadTensorInfos(BinaryReader reader)
        {
            for (ulong i = 0; i < TensorCount; i++)
            {
                var name = ReadString(reader);
                var n_dims = reader.ReadUInt32();
                var dims = new uint[n_dims];
                for (int j = 0; j < n_dims; j++)
                {
                    dims[j] = (uint)reader.ReadUInt64();
                }
                var type = (GgmlType)reader.ReadUInt32();
                var offset = reader.ReadUInt64();

                TensorInfos.Add(new GgufTensorInfo
                {
                    Name = name,
                    Dims = dims,
                    Type = type,
                    Offset = offset
                });
            }
        }

        private string ReadString(BinaryReader reader)
        {
            var length = reader.ReadUInt64();
            var bytes = reader.ReadBytes((int)length);
            return Encoding.UTF8.GetString(bytes);
        }

        private object ReadValue(BinaryReader reader, GGUFValueType type)
        {
            switch (type)
            {
                case GGUFValueType.UINT8:
                    return reader.ReadByte();
                case GGUFValueType.INT8:
                    return reader.ReadSByte();
                case GGUFValueType.UINT16:
                    return reader.ReadUInt16();
                case GGUFValueType.INT16:
                    return reader.ReadInt16();
                case GGUFValueType.UINT32:
                    return reader.ReadUInt32();
                case GGUFValueType.INT32:
                    return reader.ReadInt32();
                case GGUFValueType.FLOAT32:
                    return reader.ReadSingle();
                case GGUFValueType.BOOL:
                    return reader.ReadBoolean();
                case GGUFValueType.STRING:
                    return ReadString(reader);
                case GGUFValueType.UINT64:
                    return reader.ReadUInt64();
                case GGUFValueType.INT64:
                    return reader.ReadInt64();
                case GGUFValueType.FLOAT64:
                    return reader.ReadDouble();
                case GGUFValueType.ARRAY:
                    return ReadArray(reader);
                default:
                    throw new Exception($"Unsupported GGUF value type: {type}");
            }
        }

        private object ReadArray(BinaryReader reader)
        {
            var type = (GGUFValueType)reader.ReadUInt32();
            var count = reader.ReadUInt64();
            var array = new object[count];
            for (ulong i = 0; i < count; i++)
            {
                array[i] = ReadValue(reader, type);
            }
            return array;
        }
    }
}
