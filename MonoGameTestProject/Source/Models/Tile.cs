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
    /// A Tile from TileMapManagerMap that should have collision
    /// This tile will be added to the list of collidableComponents
    /// </summary>
    public class Tile : Component
    {
        // Tile texture
        protected Texture2D _texture;

        // Destination rectangle - game position of where to place tile
        public Rectangle DestinationRec { get; set; }

        // Source rectangle - where to take tile rectangle from texture
        public Rectangle SourceRec { get; set; }

        // Hitbox rectangle
        public Rectangle Rectangle { get; set; }

        // Colour of Tile
        public Color Colour { get; set; }

        /// <summary>
        /// Tile constructor - takes arguments from TileMapManager.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="destinationRec"></param>
        /// <param name="sourceRec"></param>
        /// <param name="colour"></param>
        /// <param name="hitboxRec"></param>
        public Tile(Texture2D texture, Rectangle destinationRec, Rectangle sourceRec, Color colour, Rectangle hitboxRec)
        {
            _texture = texture;
            DestinationRec = destinationRec;
            SourceRec = sourceRec;
            Colour = colour;
            Rectangle = hitboxRec;
        }

        #region Methods - Collision detection
        /// <summary>
        /// Wrapper collision method to detect if this sprite is touching target sprite from any direction.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool IsTouching(Sprite sprite)
        {
            return this.IsTouchingLeft(sprite) ||
                this.IsTouchingRight(sprite) ||
                this.IsTouchingTop(sprite) ||
                this.IsTouchingBottom(sprite);
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the left side of target sprite.
        /// Sources (tutorial): https://www.youtube.com/watch?v=CV8P9aq2gQo
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool IsTouchingLeft(Sprite sprite)
        {
            return sprite.Rectangle.Right + sprite.Velocity.X > this.Rectangle.Left &&
                sprite.Rectangle.Left < this.Rectangle.Left &&
                sprite.Rectangle.Bottom > this.Rectangle.Top &&
                sprite.Rectangle.Top < this.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the right side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool IsTouchingRight(Sprite sprite)
        {
            return sprite.Rectangle.Left + sprite.Velocity.X < this.Rectangle.Right &&
                sprite.Rectangle.Right > this.Rectangle.Right &&
                sprite.Rectangle.Bottom > this.Rectangle.Top &&
                sprite.Rectangle.Top < this.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the top side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool IsTouchingTop(Sprite sprite)
        {
            return sprite.Rectangle.Bottom + sprite.Velocity.Y > this.Rectangle.Top &&
               sprite.Rectangle.Top < this.Rectangle.Top &&
               sprite.Rectangle.Right > this.Rectangle.Left &&
               sprite.Rectangle.Left < this.Rectangle.Right;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the bottom side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool IsTouchingBottom(Sprite sprite)
        {
            return sprite.Rectangle.Top + sprite.Velocity.Y < this.Rectangle.Bottom &&
                sprite.Rectangle.Bottom > this.Rectangle.Bottom &&
                sprite.Rectangle.Right > this.Rectangle.Left &&
                sprite.Rectangle.Left < this.Rectangle.Right;
        }

        #endregion Methods - Collision detection

        /// <summary>
        /// Update method for Tile.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprite"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprite)
        {
            // No update method needed for this static tile object.
            // Note: Sprite objects will use Collision detection methods on the Tiles, so no need to CheckCollision here either.
        }

        /// <summary>
        /// Draw method for Tile.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DestinationRec, SourceRec, Colour);
        }

    }
}
