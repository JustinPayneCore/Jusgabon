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

    }
}
