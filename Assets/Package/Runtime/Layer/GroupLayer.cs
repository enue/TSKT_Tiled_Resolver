using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    public class GroupLayer : Layer
    {
        [System.Xml.Serialization.XmlElement("layer", typeof(TileLayer))]
        [System.Xml.Serialization.XmlElement("objectgroup", typeof(ObjectLayer))]
        [System.Xml.Serialization.XmlElement("group", typeof(GroupLayer))]
        [System.Xml.Serialization.XmlElement("image", typeof(ImageLayer))]
        public Layer[] layers;
        public Layer[] Layers => layers ?? System.Array.Empty<Layer>();
    }
}
