using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    public struct Property
    {
        [System.Xml.Serialization.XmlAttribute]
        public string name;
        [System.Xml.Serialization.XmlAttribute]
        public PropertyType type;
        [System.Xml.Serialization.XmlAttribute]
        public string value;
    }

    public readonly struct PropertyGroup
    {
        readonly Property[] properties;

        public PropertyGroup(Property[] properties)
        {
            this.properties = properties ?? System.Array.Empty<Property>();
        }

        public bool TryGetString(string key, out string value)
        {
            foreach (var it in properties)
            {
                if (it.name == key
                    && (it.type == PropertyType.String))
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
            foreach (var it in properties)
            {
                if (it.name == key
                    && it.type == PropertyType.Bool)
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
            foreach (var it in properties)
            {
                if (it.name == key
                    && it.type == PropertyType.Int)
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
            foreach (var it in properties)
            {
                if (it.name == key
                    && it.type == PropertyType.Float)
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
            foreach (var it in properties)
            {
                if (it.name == key
                    && it.type == PropertyType.Color)
                {
                    value = ParseColor(it.value);
                    return true;
                }
            }
            value = default;
            return false;
        }

        public bool TryGetFile(string key, out string value)
        {
            foreach (var it in properties)
            {
                if (it.name == key
                    && it.type == PropertyType.File)
                {
                    value = it.value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public static Color32 ParseColor(string src)
        {
            var colorCode = src.TrimStart('#');
            var value = System.Convert.ToInt32(colorCode, 16);

            int a;
            if (colorCode.Length == 6)
            {
                a = (value >> 24) & 0xff;
            }
            else if (colorCode.Length == 8)
            {
                a = 0xff;
            }
            else
            {
                throw new System.ArgumentException(src);
            }
            var r = (value >> 16) & 0xff;
            var g = (value >> 8) & 0xff;
            var b = value & 0xff;
            return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}
