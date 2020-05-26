using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Linq;
using System.Xml.Serialization;

namespace TSKT.TiledResolvers
{
    public enum Orientation
    {
        [XmlEnum("orthogonal")]
        Orthogonal,
        [XmlEnum("isometric")]
        Isometric,
        [XmlEnum("staggered")]
        Staggered,
        [XmlEnum("hexagonal")]
        Hexagonal,
    }

    public enum PropertyType
    {
        [XmlEnum("string")]
        String,
        [XmlEnum("bool")]
        Bool,
        [XmlEnum("float")]
        Float,
        [XmlEnum("int")]
        Int,
        [XmlEnum("color")]
        Color,
    }
    public enum RenderOrder
    {
        [XmlEnum("right-down")]
        RightDown,
        [XmlEnum("right-up")]
        RightUp,
        [XmlEnum("left-down")]
        LeftDown,
        [XmlEnum("left-up")]
        LeftUp,
    }

    public enum Compression
    {
        None,
        [XmlEnum("gzip")]
        Gzip,
        [XmlEnum("zlib")]
        Zlib,
        [XmlEnum("zstd")]
        Zstandard,
    }
    public enum Encoding
    {
        Invalid,
        [XmlEnum("csv")]
        Csv,
        [XmlEnum("base64")]
        Base64,
    }
    public enum DrawOrder
    {
        Topdown,
        [XmlEnum("index")]
        Index,
    }
}
