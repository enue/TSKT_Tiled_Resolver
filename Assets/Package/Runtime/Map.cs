﻿using System.Collections;
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
        public string orientation;
        [System.Xml.Serialization.XmlAttribute("renderorder")]
        public string renderOrder;
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

        [System.Xml.Serialization.XmlElement("properties")]
        public PropertyGroup propertyGroup;

        [System.Xml.Serialization.XmlElement("tileset")]
        public TileSet[] tileSets;
        public TileSet[] TileSets => tileSets ?? System.Array.Empty<TileSet>();

        [System.Xml.Serialization.XmlElement("layer")]
        public Layer[] layers;
        public Layer[] Layers => layers ?? System.Array.Empty<Layer>();

        [System.Xml.Serialization.XmlElement("objectgroup")]
        public ObjectGroup[] objectGroups;
        public ObjectGroup[] ObjectGroups => objectGroups ?? System.Array.Empty<ObjectGroup>();

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
                foreach (var layer in Layers)
                {
                    usedGids.UnionWith(layer.data.Values);
                }
                foreach (var objectGroup in ObjectGroups)
                {
                    foreach (var obj in objectGroup.Objects)
                    {
                        usedGids.Add(obj.Gid);
                    }
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