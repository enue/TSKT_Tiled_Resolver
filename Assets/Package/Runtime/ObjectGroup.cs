using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    public class ObjectGroup
    {
        [System.Serializable]
        public struct Object
        {
            [System.Serializable]
            public class Polygon
            {
                [System.Xml.Serialization.XmlAttribute]
                public string points;

                public Vector2[] Points
                {
                    get
                    {
                        return points.Split(' ')
                            .Select(_ => _.Split(','))
                            .Select(_ => new Vector2(
                                float.Parse(_[0], System.Globalization.CultureInfo.InvariantCulture),
                                float.Parse(_[1], System.Globalization.CultureInfo.InvariantCulture)))
                            .ToArray();
                    }
                }
            }

            [System.Serializable]
            public class Ellipse
            {
            }

            [System.Serializable]
            public class Point
            {
            }

            [System.Serializable]
            public class Text
            {
                [System.Xml.Serialization.XmlAttribute]
                public string fontfamily;

                [System.Xml.Serialization.XmlAttribute]
                public int pixelsize;

                [System.Xml.Serialization.XmlAttribute]
                public string color;
                public Color32 Color => PropertyGroup.ParseColor(color);

                [System.Xml.Serialization.XmlAttribute]
                public string halign;

                [System.Xml.Serialization.XmlAttribute]
                public bool wrap;

                [System.Xml.Serialization.XmlText]
                public string text;
            }

            [System.Xml.Serialization.XmlAttribute]
            public bool visible;
            [System.Xml.Serialization.XmlAttribute]
            public string name;
            [System.Xml.Serialization.XmlAttribute]
            public string type;
            [System.Xml.Serialization.XmlAttribute]
            public float x;
            [System.Xml.Serialization.XmlAttribute]
            public float y;
            [System.Xml.Serialization.XmlAttribute]
            public float width;
            [System.Xml.Serialization.XmlAttribute]
            public float height;
            [System.Xml.Serialization.XmlAttribute]
            public float rotation;
            [System.Xml.Serialization.XmlAttribute]
            public long gid;

            [System.Xml.Serialization.XmlArrayItem("property")]
            public Property[] properties;
            public PropertyGroup PropertyGroup => new PropertyGroup(properties);

            [System.Xml.Serialization.XmlElement]
            public Ellipse ellipse;
            [System.Xml.Serialization.XmlElement]
            public Polygon polygon;
            [System.Xml.Serialization.XmlElement]
            public Polygon polyline;
            [System.Xml.Serialization.XmlElement]
            public Point point;
            [System.Xml.Serialization.XmlElement]
            public Text text;

            // https://github.com/bjorn/tiled/blob/d027f7c14b5e0e24584277fd4752d67fcde90ffd/src/libtiled/wangset.cpp#L38-L41
            public int Gid => (int)(gid & (~(1 << 30)) & (~(1 << 31)));
            public bool FlipX => (gid & (1 << 31)) != 0;
            public bool FlipY => (gid & (1 << 30)) != 0;
        }

        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string name;

        [System.Xml.Serialization.XmlAttribute]
        public string color;
        public Color32 Color => PropertyGroup.ParseColor(color);

        [System.Xml.Serialization.XmlAttribute]
        public DrawOrder draworder;

        [System.Xml.Serialization.XmlAttribute]
        public float opacity = 1f;

        [System.Xml.Serialization.XmlAttribute]
        public float offsetx = 0f;

        [System.Xml.Serialization.XmlAttribute]
        public float offsety = 0f;

        [System.Xml.Serialization.XmlArrayItem("property")]
        public Property[] properties;
        public PropertyGroup PropertyGroup => new PropertyGroup(properties);

        [System.Xml.Serialization.XmlElement("object")]
        public Object[] objects;
        public Object[] Objects => objects ?? System.Array.Empty<Object>();
    }

}
