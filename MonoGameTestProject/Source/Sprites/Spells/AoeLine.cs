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
    public class AoeLine : Spell
    {

        private float _timer;

        // A spell has a lifespan before the spell reaches max range and fizzles out)
        //public float LifeSpan { get; set; }

        //public bool IsAction = false;

        //public new int Magic { get; set; }

        public float ChannelDelay { get; set; }

        public bool _isChanneling = false;

        private List<Sprite> _hitSprites;

        private List<Vector2> _positions;

        // Velocity Speed
        //public new float Speed { get; set; }

        public int SpellWidth { get; set; }
        
        public int SpellLength { get; set; }

        //public new int Width { get; set; }

        //public new int Height { get; set; }

        public new Rectangle Rectangle
        {
            get
            {
                Rectangle rectangle;
                if (Direction == Directions.Down)
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        Width * SpellWidth,
                        Height * SpellLength
                        );
                }
                else if (Direction == Directions.Up)
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y - (Height * (SpellLength - 1)),
                        Width * SpellWidth,
                        Height * SpellLength
                        );
                }
                else if (Direction == Directions.Left)
                {
                    rectangle = new Rectangle(
                        (int)Position.X - (Width * (SpellLength - 1)),
                        (int)Position.Y,
                        Width * SpellLength,
                        Height * SpellWidth
                        );
                }
                else // Direction == Directions.Right
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        Width * SpellLength,
                        Height * SpellWidth
                        );
                }
                return rectangle;
            }
        }



        public AoeLine(Dictionary<string, Animation> animations, float castDelay, int spellWidth, int spellLength) : base(animations)
        {
            if (castDelay <= 0f)
            {
                _isChanneling = false;
                ChannelDelay = 0f;
            }
            else
            {
                ChannelDelay = castDelay;
                _isChanneling = true;
            }

            SpellWidth = spellWidth;
            SpellLength = spellLength;

            Width = _animationManager.Animation.FrameWidth;
            Height = _animationManager.Animation.FrameHeight;
        }

        /// <summary>
        /// Action method to set up the weapon attack action.
        /// </summary>
        public override void Action()
        {
            // cannot make action is no player is holding this weapon
            if (Parent == null)
                return;

            // start action for update method
            IsAction = true;

            // initialize list of sprites that are hit from this spell
            _hitSprites = new List<Sprite>();

            SetSpellProperties();
        }

        protected override void SetSpellProperties()
        {
            if (_isChanneling == true)
            {
                _animations["Cast"].FrameSpeed = ChannelDelay / _animations["Cast"].FrameCount;
            }

            _animations["Sprite"].FrameSpeed = LifeSpan / _animations["Sprite"].FrameCount;


            _positions = new List<Vector2>();
            Direction = Parent.Direction;

            if (Direction == Directions.Down)
            {
                Position = new Vector2(Parent.Origin.X - Origin.X - ((SpellWidth - 1) * Height / 2), Parent.Origin.Y + (Origin.Y / 2));
                for(int i = 0; i < SpellWidth; i++)
                {
                    for (int j = 0; j < SpellLength; j++)
                    {
                        var position = new Vector2(Position.X + (i * Width), Position.Y + (j * Height));
                        _positions.Add(position);
                    }
                }
            }
            else if (Direction == Directions.Up)
            {
                Position = new Vector2(Parent.Origin.X - Origin.X - ((SpellWidth - 1) * Width / 2), Parent.Origin.Y - Height - (Origin.Y / 2));
                for (int i = 0; i < SpellWidth; i++)
                {
                    for (int j = 0; j < SpellLength; j++)
                    {
                        var position = new Vector2(Position.X + (i * Width), Position.Y - (j * Height));
                        _positions.Add(position);
                    }
                }
            }
            else if (Direction == Directions.Left)
            {
                Position = new Vector2(Parent.Origin.X - Width - (Origin.Y / 2), Parent.Origin.Y - Origin.Y - ((SpellWidth - 1) * Height / 2));
                for (int i = 0; i < SpellWidth; i++)
                {
                    for (int j = 0; j < SpellLength; j++)
                    {
                        var position = new Vector2(Position.X - (j * Width), Position.Y + (i * Height));
                        _positions.Add(position);
                    }
                }
            }
            else // Direction == Directions.Right
            {
                Position = new Vector2(Parent.Origin.X + (Origin.Y / 2), Parent.Origin.Y - Origin.Y - ((SpellWidth - 1) * Height / 2));
                for (int i = 0; i < SpellWidth; i++)
                {
                    for (int j = 0; j < SpellLength; j++)
                    {
                        var position = new Vector2(Position.X + (j * Width), Position.Y + (i * Height));
                        _positions.Add(position);
                    }
                }
            }

        }

        protected override void UpdateAction() 
        {
            if (_isChanneling == true && _timer >= ChannelDelay)
            {
                // done casting
                _isChanneling = false;
            }
        }

        protected override void SetAnimations()
        {
            if (_isChanneling == true)
                _animationManager.Play(_animations["Cast"]);
            else
            {
                _animationManager.Play(_animations["Sprite"]);
            }
        }

        #region Methods - Collision detection

        /// <summary>
        /// CheckCollision method (Spell).
        /// </summary>
        /// <param name="sprites"></param>
        protected override void CheckCollision(List<Sprite> sprites)
        {
            if (_isChanneling)
                return;

            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (sprite == Parent)
                    continue;

                // hit sprite if not hit before
                if (this.IsTouching(sprite))
                    if (_hitSprites.Contains(sprite) == false)
                    {
                        sprite.OnCollide(this);
                        _hitSprites.Add(sprite);
                    }
            }
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the left side of target sprite.
        /// Sources (tutorial): https://www.youtube.com/watch?v=CV8P9aq2gQo
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected override bool IsTouchingLeft(Sprite sprite)
        {
            return this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Left &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the right side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected override bool IsTouchingRight(Sprite sprite)
        {
            return this.Rectangle.Left < sprite.Rectangle.Right &&
                this.Rectangle.Right > sprite.Rectangle.Right &&
                this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Bottom;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the top side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected override bool IsTouchingTop(Sprite sprite)
        {
            return this.Rectangle.Bottom > sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        /// <summary>
        /// Collision method to detect if this sprite is touching the bottom side of target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        protected override bool IsTouchingBottom(Sprite sprite)
        {
            return this.Rectangle.Top < sprite.Rectangle.Bottom &&
                this.Rectangle.Bottom > sprite.Rectangle.Bottom &&
                this.Rectangle.Right > sprite.Rectangle.Left &&
                this.Rectangle.Left < sprite.Rectangle.Right;
        }

        #endregion Methods - Collision detection

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            // update timer
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= LifeSpan + ChannelDelay)
                IsRemoved = true;

            SetAnimations();
            _animationManager.Update(gameTime, sprites);

            // update action
            UpdateAction();

            // check collision
            CheckCollision(sprites);
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAction == false)
                return;

            foreach(var position in _positions)
            {
                spriteBatch.Draw(
                texture: _animationManager.Animation.Texture,
                position: position,
                sourceRectangle: _animationManager.SourceRectangle,
                color: _animationManager.Colour,
                rotation: _animationManager.Rotation,
                origin: _animationManager.Origin,
                scale: 1,
                effects: SpriteEffects.None,
                layerDepth: _animationManager.Layer);
            }

        }

    }
}
