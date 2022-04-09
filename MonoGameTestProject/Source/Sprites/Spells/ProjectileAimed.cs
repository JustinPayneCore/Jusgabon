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
    /// Projectile Aimed Spell - A straight projectile missile aimed towards the player.
    /// </summary>
    public class ProjectileAimed : Spell
    {
        #region Members

        // timer to track spell update action
        private float _timer;


        // The target sprite that the spell should be aimed at
        public Sprite FollowTarget { get; set; }


        // hitbox rectangle of texture
        public new Rectangle Rectangle
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + (Width / 5),
                    (int)Position.Y + (Height / 5),
                    Width - (Width / 5),
                    Height - (Height / 5)
                    );
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
        /// Projectile Aimed Spell Constructor.
        /// </summary>
        /// <param name="animations"></param>
        public ProjectileAimed(Dictionary<string, Animation> animations) : base(animations)
        {
            Width = _animationManager.Animation.FrameWidth;
            Height = _animationManager.Animation.FrameHeight;
        }

        /// <summary>
        /// Action method - Start the spell action.
        /// </summary>
        public override void Action()
        {
            // cannot make action is spell does not have a parent caster
            if (Parent == null)
                return;

            // start action for update method
            IsAction = true;

            // make enemy spell follow player
            FollowTarget = Globals.player;

            SetSpellProperties();
        }

        /// <summary>
        /// Set Spell Properties method (ProjectileAimed spell).
        /// - set direction that the projectile should be moving towards
        /// - set starting position of spell (center of parent)
        /// </summary>
        protected override void SetSpellProperties()
        {
            // set direction that the projectile should be moving towards
            var distance = FollowTarget.Position - Parent.Position;
            var _rotation = (float)Math.Atan2(distance.Y, distance.X);
            var DirectionVector = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));

            Velocity = DirectionVector * Speed;

            // set starting position of spell (center of parent)
            Position = new Vector2(Parent.Origin.X - Origin.X, Parent.Origin.Y - Origin.Y);

        }

        /// <summary>
        /// Update action method - update spell traits, like the way it moves and activates.
        /// </summary>
        protected override void UpdateAction() {}

        /// <summary>
        /// Set animations method for ProjectileAimed Spell.
        /// </summary>
        protected override void SetAnimations()
        {
            _animationManager.Play(_animations["Sprite"]);
        }

        #region Methods - Collision detection

        /// <summary>
        /// CheckCollision method (Spell).
        /// </summary>
        /// <param name="sprites"></param>
        protected override void CheckCollision(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (sprite == Parent)
                    continue;

                if (sprite is Spell)
                    continue;

                // hit sprite
                if (this.IsTouching(sprite))
                {
                    IsRemoved = true;
                    sprite.OnCollide(this);
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

        /// <summary>
        /// Update method for Projectile Aimed Spell.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            // update timer
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // spell action is over
            if (_timer >= LifeSpan)
                IsRemoved = true;

            // set & update animations
            SetAnimations();
            _animationManager.Update(gameTime, sprites);

            // update action
            UpdateAction();

            // check collision
            CheckCollision(sprites);

            Position += Velocity;
        }


        /// <summary>
        /// Draw method for Projectile Aimed Spell.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // if spell is not in action
            if (IsAction == false)
                return;

            base.Draw(gameTime, spriteBatch);
        }


        #endregion Methods


    }
}
