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
    }
}
