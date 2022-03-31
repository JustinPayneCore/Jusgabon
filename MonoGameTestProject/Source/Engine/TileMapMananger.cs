using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;

namespace Jusgabon.Source.Engine
{
    public class TileMapMananger
    {
        // Tiled map file
        TmxMap map;

        // Tileset image file
        Texture2D tileset;

        // Tile width count for map
        int tilesetTilesWide;

        // Width size of single tile
        int tileWidth;

        // Height size of single tile
        int tileHeight;

        public TileMapMananger(TmxMap _map, Texture2D _tileset,
            int _tilesetTilesWide, int _tileWidth, int _tileHeight)
        {
            map = _map;
            tileset = _tileset;
            tilesetTilesWide = _tilesetTilesWide;
            tileWidth = _tileWidth;
            tileHeight = _tileHeight;
        }

        // 
        public void Draw(SpriteBatch spriteBatch)
        {
            // Loops through each tile in the Tiled map file
            for (var i = 0; i < map.Layers.Count; i++)
            {
                // Loops through each tile in that layer of the Tiled map file
                for (var j = 0; j < map.Layers[i].Tiles.Count; j++)
                {
                    // Select that specific tile
                    int gid = map.Layers[i].Tiles[j].Gid;
                    if (gid == 0)
                    {
                        // Do nothing
                    } 
                    else if (gid == 1049)
                    {
                        // do something
                        
 
                    }
                    else
                    {
                        // Draws the tile
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);
                        float x = (j % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(j / (double)map.Width) * map.TileHeight;
                        Rectangle tilesetRec = new Rectangle((tileWidth) * column, (tileHeight) * row, tileWidth, tileHeight);
                        spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                    }
                }
            }
        }
    }
}
