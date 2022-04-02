#region Includes
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TiledSharp;
#endregion

namespace Jusgabon
{
    public class TileMapManager
    {
        // List of Tiles that have collision
        private List<Tile> _tilesCollidable;

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

        public TileMapManager(TmxMap _map, Texture2D _tileset,
            int _tilesetTilesWide, int _tileWidth, int _tileHeight)
        {
            map = _map;
            tileset = _tileset;
            tilesetTilesWide = _tilesetTilesWide;
            tileWidth = _tileWidth;
            tileHeight = _tileHeight;

            // initialize list of collidable Tiles
            _tilesCollidable = LoadTiles();
            Globals.tilesCollidable = _tilesCollidable;
        }

        public List<Tile> LoadTiles()
        {
            var tiles = new List<Tile>();

            // Loops through each layer in the Tiled map file
            // layers 0,1,2 are non-collidable tiles because 0 is floor, 1 is floor details, 2 is spawn positions
            // todo: change 'var i = 3' after level1.tmx map file has an additional layer (layer 2) for spawn positions
            for (var i = 2; i < map.Layers.Count; i++)
            {
                // Loops through each tile in that layer of the Tiled map file
                for (var j = 0; j < map.Layers[i].Tiles.Count; j++)
                {
                    // Select that specific tile
                    int gid = map.Layers[i].Tiles[j].Gid;

                    if (gid == 0)
                        continue;

                    else if (gid == 1049)
                    {
                        // Creates the collidable and invisible FloorTile
                        int tileFrame = gid + 4;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);
                        float x = (j % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(j / (double)map.Width) * map.TileHeight;
                        Rectangle destinationRec = new Rectangle((int)x, (int)y, tileWidth, tileHeight);
                        Rectangle tilesetRec = new Rectangle((tileWidth) * column, (tileHeight) * row, tileWidth, tileHeight);
                        Rectangle hitboxRec = new Rectangle(
                            (int)x + (tileWidth / 4), 
                            (int)y + (tileHeight / 4),
                            tileWidth - (tileWidth / 2), 
                            tileHeight - (tileHeight / 2));
                        tiles.Add(new Tile(tileset, destinationRec, tilesetRec, Color.White, hitboxRec));
                    }
                    else
                    {
                        // Creates the collidable Tile
                        int tileFrame = gid - 1;
                        int column = tileFrame % tilesetTilesWide;
                        int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);
                        float x = (j % map.Width) * map.TileWidth;
                        float y = (float)Math.Floor(j / (double)map.Width) * map.TileHeight;
                        Rectangle destinationRec = new Rectangle((int)x, (int)y, tileWidth, tileHeight);
                        Rectangle tilesetRec = new Rectangle((tileWidth) * column, (tileHeight) * row, tileWidth, tileHeight);
                        Rectangle hitboxRec = new Rectangle(
                            (int)x + (tileWidth / 10),
                            (int)y + (tileHeight / 10),
                            tileWidth - (tileWidth / 5),
                            tileHeight - (tileHeight / 5));
                        tiles.Add(new Tile(tileset, destinationRec, tilesetRec, Color.White, hitboxRec));
                    }
                }
            }

            return tiles;
        }

        // 
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Loops through each layer in the Tiled map file
            // layers 0 & 1 are drawn because they are background tiles
            for (var i = 0; i <= 1; i++)
            {
                // Loops through each tile in that layer of the Tiled map file
                for (var j = 0; j < map.Layers[i].Tiles.Count; j++)
                {
                    // Select that specific tile
                    int gid = map.Layers[i].Tiles[j].Gid;

                    if (gid == 0)
                        continue;

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

            // draw all collidable tiles
            foreach (var tile in _tilesCollidable)
                tile.Draw(gameTime, spriteBatch);
        }
    }
}
