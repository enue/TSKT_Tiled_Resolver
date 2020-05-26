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
        public PropertyGroup PropertyGroup => new PropertyGroup(properties);

        [System.Xml.Serialization.XmlElement("tileset")]
        public TileSet[] tileSets;
        public TileSet[] TileSets => tileSets ?? System.Array.Empty<TileSet>();

        [System.Xml.Serialization.XmlElement("layer", typeof(TileLayer))]
        [System.Xml.Serialization.XmlElement("objectgroup", typeof(ObjectLayer))]
        [System.Xml.Serialization.XmlElement("group", typeof(GroupLayer))]
        [System.Xml.Serialization.XmlElement("image", typeof(ImageLayer))]
        public Layer[] layers;
        public Layer[] Layers => layers ?? System.Array.Empty<Layer>();

        public static Map Build(string xmlText)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Map));
            var reader = new System.IO.StringReader(xmlText);
            return (Map)serializer.Deserialize(reader);
        }

        public Vector2 LayerPixelSize
        {
            get
            {
                if (orientation == Orientation.Orthogonal)
                {
                    return new Vector2(
                        tileWidth * width,
                        tileHeight * height);
                }
                else if (orientation == Orientation.Isometric)
                {
                    var length = (tileWidth * width + tileHeight * height) / 2f;
                    return new Vector2(length, length);
                }
                else if (orientation == Orientation.Staggered)
                {
                    if (staggerAxis == StaggerAxis.X)
                    {
                        return new Vector2(
                            GetStaggerAxisLength(width, tileWidth, 0),
                            GetLength(height, tileHeight, width));
                    }
                    else if (staggerAxis == StaggerAxis.Y)
                    {
                        return new Vector2(
                            GetLength(width, tileWidth, height),
                            GetStaggerAxisLength(height, tileHeight, 0));
                    }
                    else
                    {
                        throw new System.ArgumentException(staggerAxis.ToString());
                    }
                }
                else if (orientation == Orientation.Hexagonal)
                {
                    if (staggerAxis == StaggerAxis.X)
                    {
                        return new Vector2(
                            GetStaggerAxisLength(width, tileWidth, hexSideLength),
                            GetLength(height, tileHeight, width));
                    }
                    else if (staggerAxis == StaggerAxis.Y)
                    {
                        return new Vector2(
                            GetLength(width, tileWidth, height),
                            GetStaggerAxisLength(height, tileHeight, hexSideLength));
                    }
                    else
                    {
                        throw new System.ArgumentException(staggerAxis.ToString());
                    }
                }
                else
                {
                    throw new System.ArgumentException(orientation.ToString());
                }
            }
        }

        static float GetStaggerAxisLength(int tileCount, int tileLength, int hexSideLength)
        {
            return (tileCount - 1) * (tileLength + hexSideLength) / 2 + tileLength;
        }

        static float GetLength(int tileCount, int tileLength, int staggerAxisLength)
        {
            float length = tileLength * tileCount;
            if (staggerAxisLength > 1)
            {
                length += tileLength / 2f;
            }
            return length;
        }

        public IEnumerable<(Layer layer, Vector2 offset, float opacity)> FlattenLayers
        {
            get
            {
                var tasks = new Stack<(Layer layer, Vector2 offset, float opacity)>();
                foreach(var it in Layers.Reverse())
                {
                    tasks.Push((it, Vector2.zero, 1f));
                }

                while (tasks.Count > 0)
                {
                    var taks = tasks.Pop();
                    yield return taks;

                    if (taks.layer is GroupLayer groupLayer)
                    {
                        var offset = new Vector2(
                            taks.offset.x + groupLayer.offsetx,
                            taks.offset.y + groupLayer.offsety);
                        var opacity = taks.opacity * groupLayer.opacity;
                        foreach (var it in groupLayer.Layers.Reverse())
                        {
                            tasks.Push((it, offset, opacity));
                        }
                    }
                }
            }
        }

        public TileSet[] UsedTileSets
        {
            get
            {
                var usedGids = new HashSet<int>();

                foreach (var (layer, _, _) in FlattenLayers)
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
