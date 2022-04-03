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
    /// Sprite Class - The base Sprite/Entity objects in this game.
    /// </summary>
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

        #region Members - Take Hit

        protected int _currentHealth;

        protected float _hitTimer = 0;

        protected float _hitCooldown = 0.5f;

        public float HitSpeed = 0.25f;

        public bool IsHit = false;

        #endregion Members - Take Damage

        #region Members - Attributes

        /// <summary>
        /// These are the types of 'permanent' attributes to only change on level-up, or other sorts of events
        /// </summary>
        public Attributes BaseAttributes { get; set; }

        /// <summary>
        /// These are the extra 'temporary' attributes that can be gained from different sources (equipment, power-ups, spells, etc...)
        /// </summary>
        public List<Attributes> AttributeModifiers { get; set; }
        
        // The Actual Attributes that this sprite will have; (sum of BaseAttributes and AttributeModifiers)
        public Attributes TotalAttributes
        {
            get
            {
                return BaseAttributes + AttributeModifiers.Sum();
            }
        }
        
        // Speed Attribute
        public float Speed { get => TotalAttributes.Speed; set => TotalAttributes.Speed = value; }

        // Health Attribute
        public int Health { get => TotalAttributes.Health; set => TotalAttributes.Health = value; }

        // Mana Attribute (cannot cast magic without mana)
        public int Mana { get => TotalAttributes.Mana; set => TotalAttributes.Mana = value; }

        // Stamina Attribute (cannot dash without stamina; probably only player-related)
        public int Stamina { get => TotalAttributes.Stamina; set => TotalAttributes.Stamina = value; }

        // Attack Attribute; the amount of damage a normal attack will do
        public int Attack { get => TotalAttributes.Attack; set => TotalAttributes.Attack = value; }

        // Magic Attribute; the amount of damage a magic attack will do
        public int Magic { get => TotalAttributes.Magic; set => TotalAttributes.Magic = value; }

        #endregion Members - Attributes

        #region Members - Input
        // Input object for Keyboard/Mouse input
        public Input Input;

        // Current key input
        protected KeyboardState _currentKey;

        // Previous key input
        protected KeyboardState _previousKey;

        #endregion Members - Input

        // List of Child sprites
        public List<Sprite> Children { get; set; }

        // The Parent Sprite if this sprite is a Child
        public Sprite Parent;

        // Velocity of Sprite
        public Vector2 Velocity;

        // Vector Direction of Sprite
        public Vector2 DirectionVector;

        // The Directions that Sprite can face
        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }
        // Direction that Sprite faces
        public Directions Direction = Directions.Down;

        // How fast sprite rotates (for sprites with only 1 walk animation)
        public float RotationVelocity = 3f;

        // The Alive boolean for Sprite
        public bool IsRemoved = false;

        // The Lifespan of a Sprite
        // (ex. A spell has a lifespan before the spell reaches max range and fizzles out)
        public float LifeSpan = 0f;

        // Width of Sprite
        public int Width
        {
            get
            {
                if (_texture != null)
                    return _texture.Width;
                else
                    return _animationManager.Animation.FrameWidth;
            }
        }

        // Height of Sprite
        public int Height
        {
            get
            {
                if (_texture != null)
                    return _texture.Height;
                else
                    return _animationManager.Animation.FrameHeight;
            }
        }


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
                return new Rectangle(
                    (int)Position.X + (Width / 10),
                    (int)Position.Y + (Height / 10),
                    Width - (Width / 10),
                    Height - (Height / 10));
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
                return new Vector2(Position.X + (Width / 2), Position.Y + (Height / 2));
            }
        }

        // Layer Depth of Sprite
        protected float _layer;
        public float Layer
        {
            get { return _layer; }
            set
            {
                // for a texture
                _layer = value;

                // for an animation
                if (_animationManager != null)
                    _animationManager.Layer = _layer;
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

            // by default, initialize an empty list of AttributeModifiers
            AttributeModifiers = new List<Attributes>();

            Children = new List<Sprite>();

            Colour = Color.White;

            Layer = 0.5f;
        }
        
        /// <summary>
        /// Sprite Constructor for a sprite with a static texture i.e. an idle texture.
        /// </summary>
        /// <param name="texture"></param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;

            // by default, initialize an empty list of AttributeModifiers
            AttributeModifiers = new List<Attributes>();

            Children = new List<Sprite>();

            Colour = Color.White;

            Layer = 0.5f;
        }

        #region Methods - Collision Detection
        /// <summary>
        /// Check Collision method.
        /// Checks player with all other collidable sprites to detect if they are colliding.
        /// </summary>
        /// <param name="sprites"></param>
        protected virtual void CheckCollision(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (sprite.Parent == this)
                    continue;
                
                if (this.IsTouching(sprite))
                {
                    // hit sprite
                    sprite.OnCollide(this);

                    // check and stop horizontal movement collision
                    if ((this.Velocity.X > 0 && this.IsTouchingLeft(sprite)) ||
                    (this.Velocity.X < 0 && this.IsTouchingRight(sprite)))
                    {
                        this.Velocity.X = 0;
                    }

                    // check and stop vertical movement collision
                    if ((this.Velocity.Y > 0 && this.IsTouchingTop(sprite)) ||
                    (this.Velocity.Y < 0 && this.IsTouchingBottom(sprite)))
                    {
                        this.Velocity.Y = 0;
                    }

                }
            }

            CheckTileCollision();
        }

        /// <summary>
        /// CheckTileCollision method to check if this Sprite is colliding into Tile.
        /// </summary>
        protected void CheckTileCollision()
        {
            // if this sprite isn't moving, don't need to check for collision
            if (Velocity == Vector2.Zero)
                return;

            foreach (var tile in Globals.tilesCollidable)
            {
                if (tile.IsTouching(this))
                {
                    // check and stop horizontal movement collision
                    if ((this.Velocity.X > 0 && tile.IsTouchingLeft(this)) ||
                    (this.Velocity.X < 0 && tile.IsTouchingRight(this)))
                    {
                        this.Velocity.X = 0;
                    }

                    // check and stop vertical movement collision
                    if ((this.Velocity.Y > 0 && tile.IsTouchingTop(this)) ||
                    (this.Velocity.Y < 0 && tile.IsTouchingBottom(this)))
                    {
                        this.Velocity.Y = 0;
                    }
                }
            }
        }


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
        protected virtual bool IsTouchingLeft(Sprite sprite)
        {
            return this.Rectangle.Right + this.Velocity.X > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Left &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the right side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected virtual bool IsTouchingRight(Sprite sprite)
        {
            return this.Rectangle.Left + this.Velocity.X < sprite.Rectangle.Right &&
                this.Rectangle.Right > sprite.Rectangle.Right &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the top side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected virtual bool IsTouchingTop(Sprite sprite)
        {
            return this.Rectangle.Bottom + this.Velocity.Y > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the bottom side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected virtual bool IsTouchingBottom(Sprite sprite)
        {
            return this.Rectangle.Top + this.Velocity.Y < sprite.Rectangle.Bottom &&
                this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        /// <summary>
        /// OnCollide method.
        /// Invoke this method when this Sprite collides with target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        public virtual void OnCollide(Sprite sprite)
        {
            Console.WriteLine("OnCollide method called");
        }

        #region Per-pixel collision detection (OBSOLETE)
        /// <summary>
        /// Intersects method.
        /// Detects for per-pixel collision between the two sprites.
        /// Source (tutorial): https://www.youtube.com/watch?v=5R3qY68fKm0
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        //[Obsolete("Intersects is deprecated, please use normal rect-rect collision methods instead.")]
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

        #endregion Methods - Collision Detection

        #region Methods - Take Damage
        public virtual void TakeDamage(int damage)
        {
            if (IsHit == false && _hitTimer > _hitCooldown)
            {
                _hitTimer = 0f;
                IsHit = true;

                _currentHealth -= damage;
                Console.WriteLine(this.GetType().Name + " Health: " + _currentHealth);
            }
        }

        protected virtual void SetTakeDamage(GameTime gameTime)
        {
            // increment timers
            _hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // check if sprite is taking hit
            if (IsHit == false)
                return;

            // set take hit properties
            Colour = Color.Red;
            Velocity = Vector2.Zero;

            // reset sprite take hit cooldown
            if (_hitTimer > HitSpeed)
            {
                Colour = Color.White;
                IsHit = false;

                // check if sprite is dead
                if (_currentHealth <= 0)
                {
                    Console.WriteLine(this.GetType().Name + " killed.");
                    IsRemoved = true;
                }
            }
        }

        #endregion Methods - Take Damage


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
                {
                    _animationManager.Play(_animations["WalkRight"]);
                    Direction = Directions.Right;
                }
                else if (Velocity.X < 0)
                {
                    _animationManager.Play(_animations["WalkLeft"]);
                    Direction = Directions.Left;
                }
                else if (Velocity.Y > 0)
                {
                    _animationManager.Play(_animations["WalkDown"]);
                    Direction = Directions.Down;
                }
                else if (Velocity.Y < 0)
                {
                    _animationManager.Play(_animations["WalkUp"]);
                    Direction = Directions.Up;
                }
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
            // General Order of Updates
            // 1. Update Timers
            // 2. Update Input
            // 3. Set Input Actions/Animations
            // 4. Update Animation Manager
            // 5. Update Child
            // 6. Check Collision
            // 7. Check Take Damage Action
            // 8. Update Position

            if (_animationManager != null)
            {
                // Set Input Animations
                SetAnimations();

                // Update Animation Manager
                _animationManager.Update(gameTime, sprites);
            }

            // Check Collision
            CheckCollision(sprites);

            // Check Take Damage Action
            SetTakeDamage(gameTime);

            // Update Position
            Position += Velocity;
            Velocity = Vector2.Zero;

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
                    layerDepth: Layer);
            else if (_animationManager != null)
                _animationManager.Draw(gameTime, spriteBatch);
            else throw new Exception("Error: No texture/animations found for Sprite.");
        }

        #endregion
    }
}
