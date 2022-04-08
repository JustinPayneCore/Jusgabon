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
    /// <summary>
    /// Global class for global fields.
    /// </summary>
    class Globals
    {
        // load in sprites
        public static ContentManager content;

        // draw sprites
        public static SpriteBatch spriteBatch;

        // player object
        public static Player player;

        // collidable tile objects
        public static List<Tile> tilesCollidable;

        // collidable sprite objects
        public static List<Sprite> sprites;

        // content libraries
        public static Dictionary<string, Game1.SpriteProperties> libraryWeapons;
        public static Dictionary<string, Game1.SpriteProperties> libraryNpcs;
        public static Dictionary<string, Game1.SpriteProperties> libraryAnimals;
        public static Dictionary<string, Game1.SpriteProperties> libraryEnemies;
        public static Dictionary<string, Game1.SpriteProperties> libraryBosses;
        // spell content libaries does not need attributes
        public static Dictionary<string, Dictionary<string, Animation>> librarySpells;

    }
}
