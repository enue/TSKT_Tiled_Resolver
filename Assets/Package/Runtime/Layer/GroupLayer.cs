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
        public Layer[] layers;
        public Layer[] Layers => layers ?? System.Array.Empty<Layer>();

        public IEnumerable<int> UsedTileGids
        {
            get
            {
                foreach (var layer in Layers.OfType<TileLayer>())
                {
                    foreach (var it in layer.data.Values)
                    {
                        yield return it;
                    }
                }
                foreach (var objectGroup in Layers.OfType<ObjectLayer>())
                {
                    foreach (var obj in objectGroup.Objects)
                    {
                        yield return obj.Gid;
                    }
                }
                foreach (var groupLayer in Layers.OfType<GroupLayer>())
                {
                    foreach (var it in groupLayer.UsedTileGids)
                    {
                        yield return it;
                    }
                }
            }
        }
    }
}
