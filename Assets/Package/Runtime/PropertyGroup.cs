using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    public struct PropertyGroup
    {
        [System.Serializable]
        public struct Property
        {
            [System.Xml.Serialization.XmlAttribute]
            public string name;
            [System.Xml.Serialization.XmlAttribute]
            public string type;
            [System.Xml.Serialization.XmlAttribute]
            public string value;
        }

        [System.Xml.Serialization.XmlElement("property")]
        public Property[] properties;
        public Property[] Properties => properties ?? System.Array.Empty<Property>();

        public bool TryGetString(string key, out string value)
        {
            foreach (var it in Properties)
            {
                if (it.name == key
                    && (it.type == "string" || string.IsNullOrEmpty(it.type)))
                {
                    value = it.value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetBool(string key, out bool value)
        {
            foreach (var it in Properties)
            {
                if (it.name == key
                    && it.type == "bool")
                {
                    value = bool.Parse(it.value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetInt(string key, out int value)
        {
            foreach (var it in Properties)
            {
                if (it.name == key
                    && it.type == "int")
                {
                    value = int.Parse(it.value, System.Globalization.CultureInfo.InvariantCulture);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetFloat(string key, out float value)
        {
            foreach (var it in Properties)
            {
                if (it.name == key
                    && it.type == "float")
                {
                    value = float.Parse(it.value, System.Globalization.CultureInfo.InvariantCulture);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetColor(string key, out Color32 value)
        {
            foreach (var it in Properties)
            {
                if (it.name == key
                    && it.type == "color")
                {
                    value = ParseColor(it.value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public static Color32 ParseColor(string src)
        {
            var value = System.Convert.ToInt32(src.TrimStart('#'), 16);
            var a = (value >> 24) & 0xff;
            var r = (value >> 16) & 0xff;
            var g = (value >> 8) & 0xff;
            var b = value & 0xff;
            return new Color32((byte)r, (byte)g, (byte)b, (byte)a);

        }
    }
}
