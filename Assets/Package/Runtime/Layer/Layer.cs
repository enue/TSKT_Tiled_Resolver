using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    public class Layer
    {
        [System.Xml.Serialization.XmlAttribute]
        public int id;

        [System.Xml.Serialization.XmlAttribute]
        public string name;

        [System.Xml.Serialization.XmlAttribute]
        public bool visible = true;

        [System.Xml.Serialization.XmlAttribute]
        public bool locked;

        [System.Xml.Serialization.XmlAttribute]
        public float opacity = 1f;

        [System.Xml.Serialization.XmlAttribute]
        public float offsetx = 0f;

        [System.Xml.Serialization.XmlAttribute]
        public float offsety = 0f;

        [System.Xml.Serialization.XmlAttribute("tintcolor")]
        public string tintColor;
        public Color32? TintColor => string.IsNullOrEmpty(tintColor) ? (Color32?)null : PropertyGroup.ParseColor(tintColor);

        [System.Xml.Serialization.XmlArrayItem("property")]
        public Property[] properties;
        public PropertyGroup PropertyGroup => new PropertyGroup(properties);

        [System.Xml.Serialization.XmlIgnore]
        public GroupLayer parent;

        public bool VisibleInHierarchy
        {
            get
            {
                if (!visible)
                {
                    return false;
                }
                var layer = parent;
                while (layer != null)
                {
                    if (!layer.visible)
                    {
                        return false;
                    }
                    layer = layer.parent;
                }
                return true;
            }
        }
        public Vector2 WorldOffset
        {
            get
            {
                var result = new Vector2(offsetx, offsety);
                var layer = parent;
                while (layer != null)
                {
                    result.x += layer.offsetx;
                    result.y += layer.offsety;
                    layer = layer.parent;
                }
                return result;
            }
        }

        public float MergedOpacity
        {
            get
            {
                var result = opacity;
                var layer = parent;
                while (layer != null)
                {
                    result *= layer.opacity;
                    layer = layer.parent;
                }
                return result;
            }
        }

        public Color? MergedTintColor
        {
            get
            {
                Color? result = TintColor;
                var layer = parent;
                while (layer != null)
                {
                    if (result.HasValue)
                    {
                        result *= layer.TintColor;
                    }
                    else
                    {
                        result = layer.TintColor;
                    }
                    layer = layer.parent;
                }
                return result;
            }
        }
    }
}
