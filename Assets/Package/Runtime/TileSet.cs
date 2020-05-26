﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace TSKT.TiledResolvers
{
    [System.Xml.Serialization.XmlRoot("tileset")]
    [System.Serializable]
    public struct TileSet
    {
        [System.Serializable]
        public struct Image
        {
            [System.Xml.Serialization.XmlAttribute]
            public string source;

            [System.Xml.Serialization.XmlElement]
            public int width;

            [System.Xml.Serialization.XmlElement]
            public int height;
        }

        [System.Serializable]
        public struct Tile
        {
            [System.Xml.Serialization.XmlAttribute]
            public int id;

            [System.Xml.Serialization.XmlElement("properties")]
            public PropertyGroup propertyGroup;

            public Animation animation;
        }

        [System.Serializable]
        public struct Animation
        {
            [System.Serializable]
            public struct Frame
            {
                [System.Xml.Serialization.XmlAttribute("tileid")]
                public int tileId;
                [System.Xml.Serialization.XmlAttribute]
                public int duration;

                public float DurationSeconds => duration / 1000f;
            }

            [System.Xml.Serialization.XmlElement("frame")]
            public Frame[] frames;
            public Frame[] Frames => frames ?? System.Array.Empty<Frame>();
        }

        [System.Xml.Serialization.XmlAttribute]
        public string source;

        [System.Xml.Serialization.XmlAttribute("firstgid")]
        public int firstGid;

        [System.Xml.Serialization.XmlAttribute]
        public string name;

        [System.Xml.Serialization.XmlAttribute("tilewidth")]
        public int tileWidth;

        [System.Xml.Serialization.XmlAttribute("tileheight")]
        public int tileHeight;

        [System.Xml.Serialization.XmlAttribute("tilecount")]
        public int tileCount;

        [System.Xml.Serialization.XmlAttribute]
        public int columns;

        [System.Xml.Serialization.XmlElement]
        public Image image;

        [System.Xml.Serialization.XmlElement("tile")]
        public Tile[] tiles;
        public Tile[] Tiles => tiles ?? System.Array.Empty<Tile>();

        public static TileSet Build(string xmlText)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TileSet));
            var reader = new System.IO.StringReader(xmlText);
            return (TileSet)serializer.Deserialize(reader);
        }
    }
}