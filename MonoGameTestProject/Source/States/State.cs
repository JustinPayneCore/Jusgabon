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
    /// State Class (Abstract).
    /// Inherited classes must create a State instance that Game1 can update and draw on.
    /// </summary>
    public abstract class State
    {
        #region Members

        // content manager
        protected ContentManager _content { get => Globals.content; }

        // graphics instance
        protected GraphicsDevice _graphics;

        // the main game class instance
        protected Game1 _game;

        // spritebatch drawing object
        protected SpriteBatch _spriteBatch { get => Globals.spriteBatch; }

        #endregion Members


        #region Methods

        /// <summary>
        /// State class constructor.
        /// The Class that inherits State must pass the game instance and the graphics instance.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        public State(Game1 game, GraphicsDevice graphics)
        {
            _game = game;
            _graphics = graphics;
        }

        /// <summary>
        /// Abstract Update method.
        /// The Class that inherits State must implement an Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Abstract PostUpdate method.
        /// The Class that inherits State must implement a PostUpdate method.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void PostUpdate(GameTime gameTime);


        /// <summary>
        /// Abstract Draw method.
        /// The Class that inherits State must implement a Draw method.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Draw(GameTime gameTime);

        #endregion Methods

    }
}
