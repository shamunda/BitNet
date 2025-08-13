using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BitNet.Core
{
    public class GgufReader
    {
        public static Model ReadModel(string path)
        {
            var model = new Model();
            using (var reader = new BinaryReader(File.OpenRead(path)))
            {
                // Read header
                model.Header = new Header
                {
                    Magic = reader.ReadUInt32(),
                    Version = reader.ReadUInt32(),
                    TensorCount = reader.ReadUInt64(),
                    MetadataKVCount = reader.ReadUInt64()
                };

                if (model.Header.Magic != 1734831462) // "GGUF"
                {
                    throw new Exception("Invalid GGUF magic number");
                }

                // Read metadata
                model.Metadata = new Dictionary<string, object>();
                for (ulong i = 0; i < model.Header.MetadataKVCount; i++)
                {
                    var key = ReadGgufString(reader);
                    var valueType = (GgufValueType)reader.ReadUInt32();
                    object value = ReadValue(reader, valueType);
                    model.Metadata[key] = value;
                }

                // Read tensor info
                model.Tensors = new List<TensorInfo>();
                for (ulong i = 0; i < model.Header.TensorCount; i++)
                {
                    var tensorInfo = new TensorInfo
                    {
                        Name = ReadGgufString(reader),
                        NDimensions = reader.ReadUInt32(),
                    };
                    tensorInfo.Dimensions = new ulong[tensorInfo.NDimensions];
                    for(uint j=0; j < tensorInfo.NDimensions; j++)
                    {
                        tensorInfo.Dimensions[j] = reader.ReadUInt64();
                    }
                    tensorInfo.Type = (GgmlType)reader.ReadUInt32();
                    tensorInfo.Offset = reader.ReadUInt64();
                    model.Tensors.Add(tensorInfo);
                }
            }
            return model;
        }

        private static string ReadGgufString(BinaryReader reader)
        {
            var length = reader.ReadUInt64();
            var bytes = reader.ReadBytes((int)length);
            return Encoding.UTF8.GetString(bytes);
        }

        private static object ReadValue(BinaryReader reader, GgufValueType type)
        {
            switch (type)
            {
                case GgufValueType.Uint8: return reader.ReadByte();
                case GgufValueType.Int8: return reader.ReadSByte();
                case GgufValueType.Uint16: return reader.ReadUInt16();
                case GgufValueType.Int16: return reader.ReadInt16();
                case GgufValueType.Uint32: return reader.ReadUInt32();
                case GgufValueType.Int32: return reader.ReadInt32();
                case GgufValueType.Float32: return reader.ReadSingle();
                case GgufValueType.Bool: return reader.ReadBoolean();
                case GgufValueType.String: return ReadGgufString(reader);
                case GgufValueType.Array:
                    var arrayType = (GgufValueType)reader.ReadUInt32();
                    var count = reader.ReadUInt64();
                    var array = new List<object>();
                    for (ulong i = 0; i < count; i++)
                    {
                        array.Add(ReadValue(reader, arrayType));
                    }
                    return array;
                case GgufValueType.Uint64: return reader.ReadUInt64();
                case GgufValueType.Int64: return reader.ReadInt64();
                case GgufValueType.Float64: return reader.ReadDouble();
                default:
                    throw new NotSupportedException($"GGUF value type {type} is not supported");
            }
        }
    }

    public class Model
    {
        public Header Header { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
        public List<TensorInfo> Tensors { get; set; }
    }

    public class Header
    {
        public uint Magic { get; set; }
        public uint Version { get; set; }
        public ulong TensorCount { get; set; }
        public ulong MetadataKVCount { get; set; }
    }

    public class TensorInfo
    {
        public string Name { get; set; }
        public uint NDimensions { get; set; }
        public ulong[] Dimensions { get; set; }
        public GgmlType Type { get; set; }
        public ulong Offset { get; set; }
    }

    public enum GgufValueType : uint
    {
        Uint8 = 0,
        Int8 = 1,
        Uint16 = 2,
        Int16 = 3,
        Uint32 = 4,
        Int32 = 5,
        Float32 = 6,
        Bool = 7,
        String = 8,
        Array = 9,
        Uint64 = 10,
        Int64 = 11,
        Float64 = 12,
    }

    public enum GgmlType : uint
    {
        F32 = 0,
        F16 = 1,
        Q4_0 = 2,
        Q4_1 = 3,
        Q5_0 = 6,
        Q5_1 = 7,
        Q8_0 = 8,
        Q8_1 = 9,
        Q2_K = 10,
        Q3_K = 11,
        Q4_K = 12,
        Q5_K = 13,
        Q6_K = 14,
        Q8_K = 15,
        I8 = 16,
        I16 = 17,
        I32 = 18,
        Count,
    }
}
