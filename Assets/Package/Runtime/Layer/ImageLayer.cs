using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    public class ImageLayer : Layer
    {
        [System.Xml.Serialization.XmlAttribute]
        public string source;

        [System.Xml.Serialization.XmlAttribute]
        public int width;

        [System.Xml.Serialization.XmlAttribute]
        public int height;
    }
}
