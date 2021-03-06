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
        public Orientation orientation;
        [System.Xml.Serialization.XmlAttribute("staggerindex")]
        public StaggerIndex staggerIndex;
        [System.Xml.Serialization.XmlAttribute("staggeraxis")]
        public StaggerAxis staggerAxis;
        [System.Xml.Serialization.XmlAttribute("hexsidelength")]
        public int hexSideLength;

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
        readonly public PropertyGroup PropertyGroup => new PropertyGroup(properties);

        [System.Xml.Serialization.XmlElement("tileset")]
        public TileSet[] tileSets;
        readonly public TileSet[] TileSets => tileSets ?? System.Array.Empty<TileSet>();

        [System.Xml.Serialization.XmlElement("layer", typeof(TileLayer))]
        [System.Xml.Serialization.XmlElement("objectgroup", typeof(ObjectLayer))]
        [System.Xml.Serialization.XmlElement("group", typeof(GroupLayer))]
        [System.Xml.Serialization.XmlElement("image", typeof(ImageLayer))]
        public Layer[] layers;
        readonly public Layer[] Layers => layers ?? System.Array.Empty<Layer>();

        public static Map Build(string xmlText)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Map));
            var reader = new System.IO.StringReader(xmlText);
            var map =(Map)serializer.Deserialize(reader);
            map.BuildHierarchy();
            return map;
        }

        public void BuildHierarchy()
        {
            foreach (var layer in FlattenLayers)
            {
                if (layer is GroupLayer groupLayer)
                {
                    foreach (var child in groupLayer.Layers)
                    {
                        child.parent = groupLayer;
                    }
                }
                else if (layer is ObjectLayer objectLayer)
                {
                    foreach (var obj in objectLayer.Objects)
                    {
                        obj.parent = objectLayer;
                    }
                }
            }
        }

        readonly public Vector2 LayerPixelSize => CoordinateUtility.GetLayerPixelSize(orientation, width, height, tileWidth, tileHeight, staggerAxis, hexSideLength);

        readonly public Vector2 GetCellPosition(int i, int j)
        {
            return CoordinateUtility.GetPosition(i, j, orientation, tileWidth, tileHeight, height, staggerAxis, staggerIndex, hexSideLength);
        }

        readonly public bool GetTileByGid(int gid, out TileSet tileSet, out int id)
        {
            if (gid == 0)
            {
                tileSet = default;
                id = default;
                return false;
            }

            if (TileSets.Length == 0)
            {
                tileSet = default;
                id = default;
                return false;
            }

            var index = System.Array.FindLastIndex(TileSets, _ => _.firstGid <= gid);
            tileSet = TileSets[index];
            id = gid - tileSet.firstGid;

            if (tileSet.Exported)
            {
                return true;
            }
            return id < tileSet.tileCount;
        }

        readonly public ObjectLayer.Object TryGetObjectById(int id)
        {
            var objectLayes = FlattenLayers.OfType<ObjectLayer>();

            foreach (var it in objectLayes)
            {
                foreach(var obj in it.Objects)
                {
                    if (obj.id == id)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        readonly public IEnumerable<Layer> FlattenLayers
        {
            get
            {
                var tasks = new Stack<Layer>();
                foreach(var it in Layers.Reverse())
                {
                    tasks.Push(it);
                }

                while (tasks.Count > 0)
                {
                    var task = tasks.Pop();
                    yield return task;

                    if (task is GroupLayer groupLayer)
                    {
                        foreach (var it in groupLayer.Layers.Reverse())
                        {
                            tasks.Push(it);
                        }
                    }
                }
            }
        }

        readonly public HashSet<int> UsedGids
        {
            get
            {
                var usedGids = new HashSet<int>();

                foreach (var layer in FlattenLayers)
                {
                    if (layer is TileLayer tileLayer)
                    {
                        usedGids.UnionWith(tileLayer.data.Values);
                    }
                    else if (layer is ObjectLayer objectLayer)
                    {
                        foreach (var obj in objectLayer.Objects)
                        {
                            usedGids.Add(obj.Gid);
                        }
                    }
                    else if (layer is ImageLayer)
                    {
                        // nop
                    }
                    else if (layer is GroupLayer)
                    {
                        // nop
                    }
                    else
                    {
                        throw new System.ArgumentException(layer.GetType().ToString());
                    }
                }

                return usedGids;
            }
        }

        readonly public TileSet[] UsedTileSets
        {
            get
            {
                var usedGids = UsedGids;

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
