using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    public class Layer
    {
        [System.Serializable]
        public struct Data
        {
            [System.Xml.Serialization.XmlAttribute]
            public string encoding;

            [System.Xml.Serialization.XmlText]
            public string value;

            public int[] Values
            {
                get
                {
                    if (encoding == "csv")
                    {
                        return value.Split(',')
                            .Select(_ => int.Parse(_, System.Globalization.CultureInfo.InvariantCulture))
                            .ToArray();
                    }
                    else if (encoding == "base64")
                    {
                        var ids = new List<int>();
                        var bytes = System.Convert.FromBase64String(value);

                        for (int i = 0; i < bytes.Length / 4; ++i)
                        {
                            var index = i * 4;
                            var id = bytes[index]
                                + (bytes[index + 1] << 8)
                                + (bytes[index + 2] << 16)
                                + (bytes[index + 3] << 24);
                            ids.Add(id);
                        }
                        return ids.ToArray();
                    }
                    else
                    {
                        throw new System.ArgumentException("not support encoding " + encoding);
                    }
                }
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

        [System.Xml.Serialization.XmlElement("properties")]
        public PropertyGroup propertyGroup;

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
