using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSKT.TiledResolvers
{
    public static class CoordinateUtility
    {
        static public Vector2 GetLayerPixelSize(Orientation orientation, int width, int height, int tileWidth, int tileHeight,
            StaggerAxis staggerAxis, int hexSideLength)
        {
            if (orientation == Orientation.Orthogonal)
            {
                return new Vector2(
                    tileWidth * width,
                    tileHeight * height);
            }
            if (orientation == Orientation.Isometric)
            {
                var length = (tileWidth * width + tileHeight * height) / 2f;
                return new Vector2(length, length);
            }
            if (orientation == Orientation.Staggered)
            {
                return GetLayerPixelSize(Orientation.Hexagonal, width, height, tileWidth, tileHeight, staggerAxis, 0);
            }
            if (orientation == Orientation.Hexagonal)
            {
                if (staggerAxis == StaggerAxis.X)
                {
                    var x = (width - 1) * (tileWidth + hexSideLength) / 2 + tileWidth;
                    float y = height * tileHeight;
                    if (width > 1)
                    {
                        y += tileHeight / 2f;
                    }
                    return new Vector2(x, y);
                }
                if (staggerAxis == StaggerAxis.Y)
                {
                    var reversedSize = GetLayerPixelSize(orientation, height, width, tileHeight, tileWidth, StaggerAxis.X, hexSideLength);
                    return new Vector2(reversedSize.y, reversedSize.x);
                }
                throw new System.ArgumentException(staggerAxis.ToString());
            }
            throw new System.ArgumentException(orientation.ToString());
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

    }
}
