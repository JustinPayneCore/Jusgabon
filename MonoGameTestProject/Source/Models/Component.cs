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
    /// Component Class (Abstract).
    /// Inherited Classes must implemenet Draw and Update methods.
    /// </summary>
    public abstract class Component
    {
        /// <summary>
        /// Abstract Draw method.
        /// The Class that inherits Component must implement a Draw method.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        /// <summary>
        /// Abstract Update method.
        /// The Class that inherits Component must implement an Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprite"></param>
        public abstract void Update(GameTime gameTime, List<Sprite> sprite);
    }
}
