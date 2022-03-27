﻿#region Includes
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
    public class Sprite : Component
    {
        #region Members

        // Texture of sprite
        // All sprites either have a Texture or Animation Manager
        protected Texture2D _texture;

        // Animation Manager of sprite
        // All sprites either have an Animation Manager or Texture
        protected AnimationManager _animationManager;

        // Dictionary of Animations to manage
        protected Dictionary<string, Animation> _animations;

        // Input object for Keyboard/Mouse input
        public Input Input;

        // Current key input
        protected KeyboardState _currentKey;

        // Previous key input
        protected KeyboardState _previousKey;

        // List of Child sprites
        public List<Sprite> Children { get; set; }

        // The Parent Sprite if this sprite is a Child
        public Sprite Parent;

        // Speed of Sprite
        public float Speed = 0.6f;

        // Velocity of Sprite
        public Vector2 Velocity;

        // Direction of Sprite
        public Vector2 Direction;

        // How fast sprite rotates (for sprites with only 1 walk animation)
        public float RotationVelocity = 3f;

        // The Alive boolean for Sprite
        public bool IsRemoved = false;

        // The Lifespan of a Sprite
        // (ex. A spell has a lifespan before the spell reaches max range and fizzles out)
        public float LifeSpan = 0f;

        // Game-Position of where to draw Sprite
        protected Vector2 _position;
        public Vector2 Position 
        {
            get { return _position; }
            set
            {
                // for a texture
                _position = value;

                // for an animation
                if (_animationManager != null)
                    _animationManager.Position = _position;
            }
        }

        // Rectangle hit-box of Sprite
        // returns 80% of the actual Sprite rectangle (hitbox is smaller than sprite)
        public Rectangle Rectangle
        {
            get
            {
                if (_texture != null) // for a texture
                    return new Rectangle(
                        (int)Position.X + (_texture.Width / 10),
                        (int)Position.Y + (_texture.Height / 10),
                        _texture.Width - (_texture.Width / 10),
                        _texture.Height - (_texture.Height / 10));
                else // for an animation
                    return new Rectangle(
                        (int)Position.X + (_animationManager.Animation.FrameWidth / 10),
                        (int)Position.Y + (_animationManager.Animation.FrameHeight / 10),
                        _animationManager.Animation.FrameWidth - (_animationManager.Animation.FrameWidth / 5),
                        _animationManager.Animation.FrameHeight - (_animationManager.Animation.FrameHeight / 5));
            }
        }

        // Colour field & property of Sprite
        protected Color _color;
        public Color Colour
        {
            get { return _color; }
            set
            {
                // for a texture
                _color = value;

                // for an animation
                if (_animationManager != null)
                    _animationManager.Colour = _color;
            }
        }

        // Rotation field & property of Sprite
        protected float _rotation;
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                // for a texture
                _rotation = value;

                // for an animation
                if (_animationManager != null)
                    _animationManager.Rotation = _rotation;
            }
        }

        // Default origin (centre) of Sprite
        public Vector2 Origin
        {
            get
            {
                if (_texture != null) // for a texture
                    return new Vector2(_texture.Width / 2, _texture.Height / 2);
                else // for an animation
                    return new Vector2(
                        _animationManager.Animation.FrameWidth / 2,
                        _animationManager.Animation.FrameHeight / 2);
            }
        }

        // Color TextureData of Sprite
        // (DEPRECATED) only used for per-pixel collision detection
        public Color[] TextureData
        {
            get
            {
                if (_texture != null) // for a texture
                {
                    var _textureData = new Color[_texture.Width * _texture.Height];
                    _texture.GetData(_textureData);
                    return _textureData;
                }
                else // for an animation
                {
                    var _textureData = new Color[
                        _animationManager.Animation.FrameWidth *
                        _animationManager.Animation.FrameHeight];
                    _animationManager.Animation.Texture.GetData(0, _animationManager.SourceRectangle, _textureData, 0, _textureData.Length);
                    return _textureData;
                }
            }
        }
        
        // Transform Matrix of Sprite
        // (DEPRECATED) only used for per-pixel collision detection
        public Matrix Transform
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Origin, 0)) *
                  Matrix.CreateRotationZ(Rotation) *
                  Matrix.CreateTranslation(new Vector3(Position, 0));
            }
        }

        #endregion Members


        #region Methods

        /// <summary>
        /// Sprite Constructor for a sprite with a dictionary(ex. spritesheet) of animations
        /// </summary>
        /// <param name="animations"></param>
        public Sprite(Dictionary<string, Animation> animations)
        {
            _animations = animations;
            _animationManager = new AnimationManager(_animations.First().Value);

            Children = new List<Sprite>();

            Colour = Color.White;

        }
        
        /// <summary>
        /// Sprite Constructor for a sprite with a static texture i.e. an idle texture.
        /// </summary>
        /// <param name="texture"></param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;

            Children = new List<Sprite>();

            Colour = Color.White;

        }

        #region Methods - Collision Detection
        /// <summary>
        /// Check Collision method.
        /// Checks player with all other collidable sprites to detect if they are colliding.
        /// </summary>
        /// <param name="sprites"></param>
        public virtual void CheckCollision(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if ((this.Velocity.X > 0 && this.IsTouchingLeft(sprite)) ||
                    (this.Velocity.X < 0 && this.IsTouchingRight(sprite)))
                {
                    this.Velocity.X = 0;
                }

                if ((this.Velocity.Y > 0 && this.IsTouchingTop(sprite)) ||
                (this.Velocity.Y < 0 && this.IsTouchingBottom(sprite)))
                {
                    this.Velocity.Y = 0;
                }


            }
        }


        public bool IsTouching(Sprite sprite)
        {
            return this.IsTouchingLeft(sprite) ||
                this.IsTouchingRight(sprite) ||
                this.IsTouchingTop(sprite) ||
                this.IsTouchingBottom(sprite);
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the left side of another sprite.
        /// Sources (tutorial): https://www.youtube.com/watch?v=CV8P9aq2gQo
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected bool IsTouchingLeft(Sprite sprite)
        {
            return this.Rectangle.Right + this.Velocity.X > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Left &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the right side of another sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected bool IsTouchingRight(Sprite sprite)
        {
            return this.Rectangle.Left + this.Velocity.X < sprite.Rectangle.Right &&
                this.Rectangle.Right > sprite.Rectangle.Right &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the top side of another sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected bool IsTouchingTop(Sprite sprite)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the bottom side of another sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected bool IsTouchingBottom(Sprite sprite)
        {
            return this.Rectangle.Top + this.Velocity.Y < sprite.Rectangle.Bottom &&
                this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        /// <summary>
        /// OnCollide method.
        /// Virtual method to decide what to do if an event happens between two sprites colliding.
        /// </summary>
        /// <param name="sprite"></param>
        public virtual void OnCollide(Sprite sprite)
        {

        }

        #region Per-pixel collision detection (OBSOLETE)
        /// <summary>
        /// Intersects method.
        /// Detects for per-pixel collision between the two sprites.
        /// Source (tutorial): https://www.youtube.com/watch?v=5R3qY68fKm0
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        [Obsolete("Intersects is deprecated, please use normal rect-rect collision methods instead.")]
        public bool Intersects(Sprite sprite)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            var transformAToB = this.Transform * Matrix.Invert(sprite.Transform);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            var stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            var stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            var yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < this.Rectangle.Height; yA++)
            {
                // Start at the beginning of the row
                var posInB = yPosInB;

                for (int xA = 0; xA < this.Rectangle.Width; xA++)
                {
                    // Round to the nearest pixel
                    var xB = (int)Math.Round(posInB.X);
                    var yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < sprite.Rectangle.Width &&
                        0 <= yB && yB < sprite.Rectangle.Height)
                    {
                        // Get the colors of the overlapping pixels
                        var colourA = this.TextureData[xA + yA * this.Rectangle.Width];
                        var colourB = sprite.TextureData[xB + yB * sprite.Rectangle.Width];

                        // If both pixel are not completely transparent
                        if (colourA.A != 0 && colourB.A != 0)
                            return true;
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        #endregion

        #endregion


        /// <summary>
        /// SetAnimations Method.
        /// If sprite has animations, then set logic on which animation to use here.
        /// </summary>
        protected virtual void SetAnimations()
        {
            // Movement Animations
            if (_animations.ContainsKey("Walk")) // Walk animation does not have different directions
            {
                if (Velocity != Vector2.Zero)
                    _animationManager.Play(_animations["Walk"]);
                else
                    _animationManager.Stop();
            }
            else // Walk animations do have different directions
            {
                if (Velocity.X > 0)
                    _animationManager.Play(_animations["WalkRight"]);
                else if (Velocity.X < 0)
                    _animationManager.Play(_animations["WalkLeft"]);
                else if (Velocity.Y > 0)
                    _animationManager.Play(_animations["WalkDown"]);
                else if (Velocity.Y < 0)
                    _animationManager.Play(_animations["WalkUp"]);
                else
                    _animationManager.Stop();
            }

        }

        /// <summary>
        /// Update method.
        /// If sprite is moving, then update collision detection logic and position.
        /// If sprite only has an animation, then update animation movement & frames.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {

            if (Velocity != Vector2.Zero)
            {
                CheckCollision(sprites);
                Position += Velocity;
            }


            if (_animationManager != null)
            {
                // Update moveset animation
                SetAnimations();

                // Update animation frame
                _animationManager.Update(gameTime, sprites);
            }

        }

        /// <summary>
        /// Draw method.
        /// Draw texture (as a static image) or draw animation (moving sprite).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
                spriteBatch.Draw(
                    texture: _texture, 
                    position: Position,
                    sourceRectangle: null,
                    color: Colour,
                    rotation: Rotation,
                    origin: Origin,
                    scale: 1,
                    effects: SpriteEffects.None,
                    layerDepth: 0);
            else if (_animationManager != null)
                _animationManager.Draw(gameTime, spriteBatch);
            else throw new Exception("Error: No texture/animations found for Sprite.");
        }

        #endregion
    }
}
