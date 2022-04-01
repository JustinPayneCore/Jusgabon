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
        // _texture = tileset
        protected Texture2D _texture;

        // destination rectangle = new Rectangle((int)x, (int)y, tileWidth, tileHeight)
        public Rectangle DestinationRec { get; set; }

        // source rectangle = tilesetRec = new Rectangle((tileWidth) * column, (tileHeight) * row, tileWidth, tileHeight);
        public Rectangle SourceRec { get; set; }

        public Rectangle Rectangle { get; set; }

        //private Vector2 _position;
        //public Vector2 Position
        //{
        //    get
        //    {
        //        return _position;
        //    }
        //    set
        //    {
        //        _position.X = DestinationRec.X;
        //        _position.Y = DestinationRec.Y;
        //    }
        //}
        public Vector2 Position { get; set; }

        public Color Colour {get; set;}

        public Tile(Texture2D texture, Rectangle destinationRec, Rectangle sourceRec, Color colour, Rectangle hitboxRec)
        {
            _texture = texture;
            DestinationRec = destinationRec;
            SourceRec = sourceRec;
            Colour = colour;
            Rectangle = hitboxRec;
        }

        /// <summary>
        /// Wrapper collision method to detect if this sprite is touching target sprite from any direction.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public  bool IsTouching(Sprite sprite)
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

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, DestinationRec, SourceRec, Colour);
        }

        public override void Update(GameTime gameTime, List<Sprite> sprite)
        {
            // no update method needed for this static tile object.
            // Sprite objects will have Collision detection methods.
        }
    }
}
