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
    public class Spell : Sprite
    {

        private float _timer;

        // A spell has a lifespan before the spell reaches max range and fizzles out)
        public float LifeSpan { get; set; }

        public bool IsAction = false;

        public new int Magic { get; set; }
        
        // Velocity Speed
        public new float Speed { get; set; }

        public new int Width { get; set; }

        public new int Height { get; set; }

        public new Rectangle Rectangle 
        { 
            get 
            {
                Rectangle rectangle;
                if (Direction == Directions.Down)
                {
                    rectangle = new Rectangle(
                        (int)Position.X - Width,
                        (int)Position.Y,
                        Width,
                        Height
                        );
                } else if (Direction == Directions.Up)
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y - Height,
                        Width,
                        Height
                        );
                } else if (Direction == Directions.Left)
                {
                    rectangle = new Rectangle(
                        (int)Position.X - Width,
                        (int)Position.Y - Height,
                        Width,
                        Height
                        );
                } else // Direction == Directions.Right
                {
                    rectangle = new Rectangle(
                        (int)Position.X,
                        (int)Position.Y,
                        Width,
                        Height
                        );
                }
                return rectangle;
            } 
        }



        public Spell(Dictionary<string, Animation> animations) : base(animations)
        {
            Width = _animationManager.Animation.FrameWidth;
            Height = _animationManager.Animation.FrameHeight;
        }

        /// <summary>
        /// Action method to set up the weapon attack action.
        /// </summary>
        public virtual void Action()
        {
            // cannot make action is no player is holding this weapon
            if (Parent == null)
                return;

            // start action for update method
            IsAction = true;

            SetSpellProperties();
        }

        protected virtual void SetSpellProperties()
        {
            Direction = Parent.Direction;

            if (Direction == Directions.Down)
            {
                Rotation = MathHelper.ToRadians(90);
                Velocity.Y = Speed;

                // switch width and heights because of its rotation
                var temp = Width;
                Width = Height;
                Height = temp;
            }
            else if (Direction == Directions.Up)
            {
                Rotation = MathHelper.ToRadians(-90);
                Velocity.Y = -Speed;

                // switch width and heights because of its rotation
                var temp = Width;
                Width = Height;
                Height = temp;
            }
            else if (Direction == Directions.Left)
            {
                Rotation = MathHelper.ToRadians(180);
                Velocity.X = -Speed;
            }
            else // Direction == Directions.Right
            {
                Rotation = 0;
                Velocity.X = Speed;
            }

            var position = new Vector2(Parent.Origin.X, Parent.Origin.Y - Origin.Y);
            Position = Helpers.RotateAboutOrigin(position, Parent.Origin, Rotation);

        }

        protected virtual void UpdateAction() { }

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

                // hit sprite
                if (this.IsTouching(sprite))
                    sprite.OnCollide(this);
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

            if (_timer >= LifeSpan)
                IsRemoved = true;

            SetAnimations();
            _animationManager.Update(gameTime, sprites);

            // update action
            UpdateAction();

            // check collision
            CheckCollision(sprites);

            Position += Velocity;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAction == false)
                return;

            //Helpers.DrawRectangle(Rectangle, Color.White);

            base.Draw(gameTime, spriteBatch);

        }

    }
}
