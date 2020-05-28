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
    }
}
