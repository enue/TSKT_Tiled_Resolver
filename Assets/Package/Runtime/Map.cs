using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Serializable]
    [System.Xml.Serialization.XmlRoot("map")]
    public struct Map
    {
        [System.Xml.Serialization.XmlAttribute]
        public string version;
        [System.Xml.Serialization.XmlAttribute("tiledversion")]
        public string tiledVersion;
        [System.Xml.Serialization.XmlAttribute]
        public Orientation orientation;
        [System.Xml.Serialization.XmlAttribute("renderorder")]
        public RenderOrder renderOrder;
        [System.Xml.Serialization.XmlAttribute("compressionlevel")]
        public int compressionLevel;

        [System.Xml.Serialization.XmlAttribute]
        public int width;
        [System.Xml.Serialization.XmlAttribute]
        public int height;
        [System.Xml.Serialization.XmlAttribute("tilewidth")]
        public int tileWidth;
        [System.Xml.Serialization.XmlAttribute("tileheight")]
        public int tileHeight;

        [System.Xml.Serialization.XmlAttribute]
        public bool inifinite;
        [System.Xml.Serialization.XmlAttribute("nextlayerid")]
        public int nextLayerId;
        [System.Xml.Serialization.XmlAttribute("nextobjectid")]
        public int nextObjectId;

        [System.Xml.Serialization.XmlArrayItem("property")]
        public Property[] properties;
        public PropertyGroup PropertyGroup => new PropertyGroup(properties);

        [System.Xml.Serialization.XmlElement("tileset")]
        public TileSet[] tileSets;
        public TileSet[] TileSets => tileSets ?? System.Array.Empty<TileSet>();

        [System.Xml.Serialization.XmlElement("layer", typeof(TileLayer))]
        [System.Xml.Serialization.XmlElement("objectgroup", typeof(ObjectLayer))]
        [System.Xml.Serialization.XmlElement("group", typeof(GroupLayer))]
        public Layer[] layers;
        public Layer[] Layers => layers ?? System.Array.Empty<Layer>();

        public static Map Build(string xmlText)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Map));
            var reader = new System.IO.StringReader(xmlText);
            return (Map)serializer.Deserialize(reader);
        }

        public TileSet[] UsedTileSets
        {
            get
            {
                var usedGids = new HashSet<int>();
                foreach (var layer in Layers.OfType<TileLayer>())
                {
                    usedGids.UnionWith(layer.data.Values);
                }
                foreach (var obj in Layers.OfType<ObjectLayer>().SelectMany(_ => _.Objects))
                {
                    usedGids.Add(obj.Gid);
                }
                foreach (var gid in Layers.OfType<GroupLayer>().SelectMany(_ => _.UsedTileGids))
                {
                    usedGids.Add(gid);
                }

                var result = new List<TileSet>();
                for (int i = 0; i < TileSets.Length; ++i)
                {
                    var min = TileSets[i].firstGid;
                    int max;
                    if (i + 1 < TileSets.Length)
                    {
                        max = TileSets[i + 1].firstGid - 1;
                    }
                    else
                    {
                        max = int.MaxValue;
                    }

                    if (usedGids.Any(_ => min <= _ && _ <= max))
                    {
                        result.Add(TileSets[i]);
                    }
                }
                return result.ToArray();
            }
        }
    }

}
