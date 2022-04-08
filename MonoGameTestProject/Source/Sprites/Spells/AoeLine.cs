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
    /// Area-of-Effect (Aoe) Line Spell.
    /// Channel and Summon this spell in front of parent sprite.
    /// Target sprites only gets hit once while this spell is summoned.
    /// </summary>
    public class AoeLine : Spell
    {
        #region Members

        // timer to track spell update action
        private float _timer;

        // sprites that have been hit by the summoned spell
        private List<Sprite> _hitSprites;

        // checks if spell is casting and not yet summoned
        public bool _isCasting = false;

        // the individual positions of each section of the aoe spell
        private List<Vector2> _positions;

        // the aoe width of the spell (different from texture width)
        public int AoeWidth { get; set; }
        
        // the aoe length of the spell (diff from texture height)
        public int AoeLength { get; set; }

        // if set, the amount of time where the spell is "casting" and not yet summoned
        public float CastDelay { get; set; }

        // rectangle hitbox of the summoned aoe spell
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
                        Width * AoeWidth,
                        Height * AoeLength
                        );
                }
                else if (Direction == Directions.Up)
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y - (Height * (AoeLength - 1)),
                        Width * AoeWidth,
                        Height * AoeLength
                        );
                }
                else if (Direction == Directions.Left)
                {
                    rectangle = new Rectangle(
                        (int)Position.X - (Width * (AoeLength - 1)),
                        (int)Position.Y,
                        Width * AoeLength,
                        Height * AoeWidth
                        );
                }
                else // Direction == Directions.Right
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        Width * AoeLength,
                        Height * AoeWidth
                        );
                }
                return rectangle;
            }
        }

        // members already declared in abstract Spell class.

        //public float LifeSpan { get; set; }
        //public bool IsAction = false;
        //public new int Magic { get; set; }
        //public new float Speed { get; set; }
        //public new int Width { get; set; }
        //public new int Height { get; set; }


        #endregion Members


        #region Methods

        /// <summary>
        /// Area-of-Effect Line Spell constructor.
        /// Provide spell casting properties including the delay before the spell is summoned, and aoe width and length of the spell.
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="castingDelay"></param>
        /// <param name="spellWidth"></param>
        /// <param name="spellLength"></param>
        public AoeLine(Dictionary<string, Animation> animations, float castingDelay, int spellWidth, int spellLength) : base(animations)
        {
            // no channel time
            if (castingDelay <= 0f)
            {
                CastDelay = 0f;
                _isCasting = false;
            }
            else // set channel time
            {
                CastDelay = castingDelay;
                _isCasting = true;
            }

            // aoe size of the spell
            AoeWidth = spellWidth;
            AoeLength = spellLength;

            // width and height of one individual spell texture
            Width = _animationManager.Animation.FrameWidth;
            Height = _animationManager.Animation.FrameHeight;
        }

        /// <summary>
        /// Action method - Start the spell action.
        /// </summary>
        public override void Action()
        {
            // cannot make action if spell does not have a parent caster
            if (Parent == null)
                return;

            // start action for update method
            IsAction = true;

            // initialize list of sprites that are hit from this spell
            _hitSprites = new List<Sprite>();

            SetSpellProperties();
        }

        /// <summary>
        /// Set Spell Properties method (AoeLine spell).
        /// - Set animation frame speeds based on cast time and lifespan of spell
        /// - Set positions of each individual spell texture that makes up the whole aoe spell
        /// </summary>
        protected override void SetSpellProperties()
        {
            // set animation frame speeds based on cast time of spell
            if (_isCasting == true)
            {
                _animations["Cast"].FrameSpeed = CastDelay / _animations["Cast"].FrameCount;
            }

            // set animation frame speeds based on summoned lifespan of spell
            _animations["Sprite"].FrameSpeed = LifeSpan / _animations["Sprite"].FrameCount;

            // initialize list of positions
            _positions = new List<Vector2>();

            // get parent sprite direction
            Direction = Parent.Direction;

            // set positions of each individual spell texture, which will make up the whole aoe spell
            if (Direction == Directions.Down)
            {
                Position = new Vector2(Parent.Origin.X - Origin.X - ((AoeWidth - 1) * Height / 2), Parent.Origin.Y + (Origin.Y / 2));
                for(int i = 0; i < AoeWidth; i++)
                {
                    for (int j = 0; j < AoeLength; j++)
                    {
                        var position = new Vector2(Position.X + (i * Width), Position.Y + (j * Height));
                        _positions.Add(position);
                    }
                }
            }
            else if (Direction == Directions.Up)
            {
                Position = new Vector2(Parent.Origin.X - Origin.X - ((AoeWidth - 1) * Width / 2), Parent.Origin.Y - Height - (Origin.Y / 2));
                for (int i = 0; i < AoeWidth; i++)
                {
                    for (int j = 0; j < AoeLength; j++)
                    {
                        var position = new Vector2(Position.X + (i * Width), Position.Y - (j * Height));
                        _positions.Add(position);
                    }
                }
            }
            else if (Direction == Directions.Left)
            {
                Position = new Vector2(Parent.Origin.X - Width - (Origin.Y / 2), Parent.Origin.Y - Origin.Y - ((AoeWidth - 1) * Height / 2));
                for (int i = 0; i < AoeWidth; i++)
                {
                    for (int j = 0; j < AoeLength; j++)
                    {
                        var position = new Vector2(Position.X - (j * Width), Position.Y + (i * Height));
                        _positions.Add(position);
                    }
                }
            }
            else // Direction == Directions.Right
            {
                Position = new Vector2(Parent.Origin.X + (Origin.Y / 2), Parent.Origin.Y - Origin.Y - ((AoeWidth - 1) * Height / 2));
                for (int i = 0; i < AoeWidth; i++)
                {
                    for (int j = 0; j < AoeLength; j++)
                    {
                        var position = new Vector2(Position.X + (j * Width), Position.Y + (i * Height));
                        _positions.Add(position);
                    }
                }
            }

        }

        /// <summary>
        /// Update action method (AoeLine Spell) - update specific action traits of this spell.
        /// Check if the spell is done casting.
        /// </summary>
        protected override void UpdateAction() 
        {
            if (_isCasting == true && _timer >= CastDelay)
            {
                // done casting
                _isCasting = false;
            }
        }

        /// <summary>
        /// Set animations method for AoeLine Spell.
        /// </summary>
        protected override void SetAnimations()
        {
            if (_isCasting == true)
                _animationManager.Play(_animations["Cast"]);
            else
                _animationManager.Play(_animations["Sprite"]);

        }

        /// <summary>
        /// CheckCollision method (AoeLine Spell).
        /// </summary>
        /// <param name="sprites"></param>
        protected override void CheckCollision(List<Sprite> sprites)
        {
            // no collision if spell is still casting
            if (_isCasting)
                return;

            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (sprite == Parent)
                    continue;

                if (this.IsTouching(sprite))
                    // check if sprite has been hit before
                    if (_hitSprites.Contains(sprite) == false)
                    {
                        // add sprite to list of hit sprites
                        _hitSprites.Add(sprite);

                        // hit sprite
                        sprite.OnCollide(this);

                        // Note: this spell is still not removed upon collision with sprite.
                    }
            }
        }

        /// <summary>
        /// Update method for Aoe Line spell.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            // update timer
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // spell action is over
            if (_timer >= LifeSpan + CastDelay)
                IsRemoved = true;
            
            // set & update animations
            SetAnimations();
            _animationManager.Update(gameTime, sprites);

            // update action
            UpdateAction();

            // check collision
            CheckCollision(sprites);

            // Note: this summoned spell stays still, so position does not add velocity
        }


        /// <summary>
        /// Draw method for Aoe Line spell.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAction == false)
                return;

            // Draw spell animation for each individual position
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


        #endregion Methods
    }
}
