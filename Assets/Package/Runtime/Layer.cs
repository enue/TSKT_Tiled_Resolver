﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO.Compression;
using System.IO;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    public class Layer
    {
        [System.Serializable]
        public struct Data
        {
            [System.Xml.Serialization.XmlAttribute]
            public Encoding encoding;

            [System.Xml.Serialization.XmlAttribute]
            public Compression compression;

            [System.Xml.Serialization.XmlText]
            public string value;

            public int[] Values
            {
                get
                {
                    if (encoding == Encoding.Csv)
                    {
                        return value.Split(',')
                            .Select(_ => int.Parse(_, System.Globalization.CultureInfo.InvariantCulture))
                            .ToArray();
                    }
                    else if (encoding == Encoding.Base64)
                    {
                        var bytes = System.Convert.FromBase64String(value);
                        if (compression == Compression.None)
                        {
                            // nop
                        }
                        else if (compression == Compression.Gzip)
                        {
                            bytes = DecompressGzip(bytes);
                        }
                        else if (compression == Compression.Zlib)
                        {
                            bytes = DecompressZlib(bytes);
                        }
                        else
                        {
                            throw new System.ArgumentException("not support compresion " + compression);
                        }
                        return BytesToInts(bytes);
                    }
                    throw new System.ArgumentException("not support encoding " + encoding);
                }
            }

            public static byte[] DecompressGzip(byte[] compressedBytes)
            {
                using (var compressed = new MemoryStream(compressedBytes))
                {
                    using (var decompressionStream = new GZipStream(compressed, CompressionMode.Decompress))
                    {
                        using (var decompressed = new MemoryStream())
                        {
                            decompressionStream.CopyTo(decompressed);
                            return decompressed.ToArray();
                        }
                    }
                }
            }

            public static byte[] DecompressZlib(byte[] compressedBytes)
            {
                using (var compressed = new MemoryStream(compressedBytes))
                {
                    // skip header(2bytes)
                    compressed.Position = 2;
                    using (var deflateStream = new DeflateStream(compressed, CompressionMode.Decompress))
                    {
                        using (var decompressed = new MemoryStream())
                        {
                            deflateStream.CopyTo(decompressed);
                            return decompressed.ToArray();
                        }
                    }
                }
            }

            static int[] BytesToInts(byte[] bytes)
            {
                var ints = new List<int>();
                for (int i = 0; i < bytes.Length / 4; ++i)
                {
                    var index = i * 4;
                    var id = bytes[index]
                        + (bytes[index + 1] << 8)
                        + (bytes[index + 2] << 16)
                        + (bytes[index + 3] << 24);
                    ints.Add(id);
                }
                return ints.ToArray();
            }
        }

        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string name;

        [System.Xml.Serialization.XmlAttribute]
        public int width;

        [System.Xml.Serialization.XmlAttribute]
        public int height;

        [System.Xml.Serialization.XmlAttribute]
        public bool visible;

        [System.Xml.Serialization.XmlArrayItem("property")]
        public Property[] properties;
        public PropertyGroup PropertyGroup => new PropertyGroup(properties);

        [System.Xml.Serialization.XmlElement]
        public Data data;

        [System.Xml.Serialization.XmlAttribute]
        public float opacity = 1f;

        [System.Xml.Serialization.XmlAttribute]
        public float offsetx = 0f;

        [System.Xml.Serialization.XmlAttribute]
        public float offsety = 0f;
    }
}
