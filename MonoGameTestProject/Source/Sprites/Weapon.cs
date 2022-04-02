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
    public class Weapon : Sprite
    {
        #region Members

        public new Player Parent;

        public bool IsAction = false;

        public bool IsEquipped = false;

        private float _timer;

        public float ActionSpeed;

        public new float Speed { get; set; }

        #endregion Members

        #region Methods

        /// <summary>
        /// Weapon constructor - made up of a weapon texture with baseAttributes for weapon stats.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="baseAttributes"></param>
        public Weapon(Dictionary<string, Animation> animations, Attributes baseAttributes) : base(animations)
        {
            BaseAttributes = baseAttributes;
        }

        public virtual void PickUp(Player parent)
        {
            if (parent != Globals.player)
                return;

            Parent = parent;
            ActionSpeed = parent.AttackSpeed;
        }

        public virtual void Equip()
        {
            Parent.AttributeModifiers.Add(BaseAttributes);
            IsEquipped = true;
        }

        public virtual void Unequip()
        {
            Parent.AttributeModifiers.Remove(BaseAttributes);
            IsEquipped = false;
        }

        public virtual void Action()
        {
            if (Parent == null)
                return;

            if (IsEquipped == false)
                return;


            Direction = Parent.Direction;

            if (Direction == Directions.Down)
            {
                Position = new Vector2(Parent.Position.X + 4, Parent.Position.Y + 4);
                var finalPosition = new Vector2(Position.X, Position.Y + 12);
                Speed = (finalPosition.Y - Position.Y) / 7;
                Rotation = 0;
            } 
            else if (Direction == Directions.Up)
            {
                Position = new Vector2(Parent.Position.X + 8, Parent.Position.Y + 12);
                var finalPosition = new Vector2(Position.X, Position.Y - 12);
                Speed = (Position.Y - finalPosition.Y) / 7;
                Rotation = MathHelper.ToRadians(180);
            } 
            else if (Direction == Directions.Left)
            {
                Position = new Vector2(Parent.Position.X + 12, Parent.Position.Y + 9);
                var finalPosition = new Vector2(Position.X - 12, Position.Y);
                Speed = (Position.X - finalPosition.X) / 7;
                Rotation = MathHelper.ToRadians(90);
            }
            else // Direction == Directions.Right
            {
                Position = new Vector2(Parent.Position.X + 4, Parent.Position.Y + 14);
                var finalPosition = new Vector2(Position.X + 12, Position.Y);
                Speed = (finalPosition.X - Position.X) / 7;
                Rotation = MathHelper.ToRadians(-90);
            }

            IsAction = true;
        }

        protected virtual void UpdateAction()
        {
            if (_timer > ActionSpeed)
            {
                IsAction = false;
                _timer = 0f;
                _animationManager.Stop();
                return;
            }

            if (Direction == Directions.Down)
            {
                Velocity.Y = Speed;
                if (_timer > ActionSpeed / 2)
                    Velocity.Y = -Speed;
            }
            else if (Direction == Directions.Up)
            {
                Velocity.Y = -Speed;
                if (_timer > ActionSpeed / 2)
                    Velocity.Y = Speed;
            }
            else if (Direction == Directions.Left)
            {
                Velocity.X = -Speed;
                if (_timer > ActionSpeed / 2)
                    Velocity.X = Speed;
            }
            else if (Direction == Directions.Right)
            {
                Velocity.X = Speed;
                if (_timer > ActionSpeed / 2)
                    Velocity.X = -Speed;
            }

            _animationManager.Play(_animations["SpriteInHand"]);
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (IsAction == false)
                return;

            // update timer
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // update action
            UpdateAction();

            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAction == false)
                return;

            if (IsEquipped == false)
                return;

            base.Draw(gameTime, spriteBatch);
        }



        #endregion Methods
    }
}
