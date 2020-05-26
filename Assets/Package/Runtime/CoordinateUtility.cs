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

        static public Vector2 GetPosition(int x, int y, Orientation orientation, int tileWidth, int tileHeight, int height,
            StaggerAxis staggerAxis, StaggerIndex staggerIndex, int hexSideLength)
        {
            if (orientation == Orientation.Orthogonal)
            {
                return new Vector2(x * tileWidth, y * tileHeight);
            }
            if (orientation == Orientation.Isometric)
            {
                var originX = (tileHeight * (height - 1)) / 2f;
                var originY = 0f;

                return new Vector2(
                    originX + (x * tileWidth - y * tileHeight) / 2f,
                    originY + (x * tileWidth + y * tileHeight) / 2f);
            }
            if (orientation == Orientation.Staggered)
            {
                return GetPosition(x, y, Orientation.Hexagonal, tileWidth, tileHeight, height, staggerAxis, staggerIndex, 0);
            }
            if (orientation == Orientation.Hexagonal)
            {
                if (staggerAxis == StaggerAxis.X)
                {
                    var p = x * (tileWidth + hexSideLength) / 2;
                    if (staggerIndex == StaggerIndex.Odd)
                    {
                        var q = y * tileHeight + (x % 2) * tileHeight * 0.5f;
                        return new Vector2(p, q);
                    }
                    if (staggerIndex == StaggerIndex.Even)
                    {
                        var q = y * tileHeight + (1 - x % 2) * tileHeight * 0.5f;
                        return new Vector2(p, q);
                    }
                    throw new System.ArgumentException(staggerIndex.ToString());
                }
                if (staggerAxis == StaggerAxis.Y)
                {
                    var reversedPosition = GetPosition(y, x, orientation, tileHeight, tileWidth, 0, StaggerAxis.X, staggerIndex, hexSideLength);
                    return new Vector2(reversedPosition.y, reversedPosition.x);
                }
                throw new System.ArgumentException(staggerAxis.ToString());
            }

            throw new System.ArgumentException(orientation.ToString());
        }
    }
}
