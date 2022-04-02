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

        // timer to check action animation length
        private float _timer;

        // Action animation length
        public float ActionSpeed;

        // Parent player that has this weapon.
        public new Player Parent;

        // when true, weapon can be updated and drawn.
        public bool IsAction = false;

        // when true, weapon can be used by parent.
        public bool IsEquipped = false;

        // Speed of weapon action
        public new float Speed { get; set; }

        #endregion Members

        #region Methods

        /// <summary>
        /// Weapon constructor - made up of a weapon animation dictionary and baseAttributes for weapon attributes.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="baseAttributes"></param>
        public Weapon(Dictionary<string, Animation> animations, Attributes baseAttributes) : base(animations)
        {
            BaseAttributes = baseAttributes;
        }

        /// <summary>
        /// PickUp method to set Parent and Attack animation length
        /// </summary>
        /// <param name="parent"></param>
        public virtual void PickUp(Player parent)
        {
            if (parent != Globals.player)
                return;

            Parent = parent;
            ActionSpeed = parent.AttackSpeed;
        }

        /// <summary>
        /// Equip method for weapon and add weapon attributes to player attributes.
        /// </summary>
        public virtual void Equip()
        {
            Parent.AttributeModifiers.Add(BaseAttributes);
            IsEquipped = true;
        }

        /// <summary>
        /// Unequip method for weapon and remove weapon attributes from player attributes.
        /// </summary>
        public virtual void Unequip()
        {
            Parent.AttributeModifiers.Remove(BaseAttributes);
            IsEquipped = false;
        }

        /// <summary>
        /// Action method to set up the weapon attack action.
        /// </summary>
        public virtual void Action()
        {
            // cannot make action is no player is holding this weapon
            if (Parent == null)
                return;

            // cannot make action is weapon is not equipped
            if (IsEquipped == false)
                return;

            // start action for update method
            IsAction = true;

            // get weapon direction and inital position
            Direction = Parent.Direction;
            Position = new Vector2(Parent.Position.X + (5), Parent.Position.Y + (4));

            // get rotation, new position from rotation, and final position based on direction of action
            if (Direction == Directions.Down)
            {
                Rotation = 0;
                Position = Helpers.RotateAboutOrigin(Position, Parent.Origin, Rotation);
                var finalPosition = new Vector2(Position.X, Position.Y + (Parent.Height));
                Speed = (finalPosition.Y - Position.Y) / 7;     
            }
            else if (Direction == Directions.Up)
            {
                Rotation = MathHelper.ToRadians(180);
                Position = Helpers.RotateAboutOrigin(Position, Parent.Origin, Rotation);
                var finalPosition = new Vector2(Position.X, Position.Y - (Parent.Height));
                Speed = (Position.Y - finalPosition.Y) / 7;
            }
            else if (Direction == Directions.Left)
            {
                Rotation = MathHelper.ToRadians(90);
                Position = Helpers.RotateAboutOrigin(Position, Parent.Origin, Rotation);
                var finalPosition = new Vector2(Position.X - (Parent.Height), Position.Y);
                Speed = (Position.X - finalPosition.X) / 7;
            }
            else // Direction == Directions.Right
            {
                Rotation = MathHelper.ToRadians(-90);
                Position = Helpers.RotateAboutOrigin(Position, Parent.Origin, Rotation);
                var finalPosition = new Vector2(Position.X + (Parent.Height), Position.Y);
                Speed = (finalPosition.X - Position.X) / 7;
            }
        }

        /// <summary>
        /// CheckCollision method for weapon.
        /// </summary>
        /// <param name="sprites"></param>
        protected override void CheckCollision(List<Sprite> sprites)
        {
            foreach(var sprite in sprites)
            {
                if (sprite == this)
                    continue;

                if (sprite == Parent)
                    continue;

                
                if (this.IsTouching(sprite))
                {
                    //Console.WriteLine("hit something");
                    sprite.OnCollide(this);
                }
                
            }
        }

        /// <summary>
        /// UpdateAction method - moves weapon in a stabbing motion.
        /// </summary>
        protected virtual void UpdateAction()
        {
            // stop action if timer is past animation length
            if (_timer > ActionSpeed)
            {
                IsAction = false;
                _timer = 0f;
                _animationManager.Stop();
                return;
            }

            // move weapon in forward direction in first half of action, then move backwards for second half of action
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

            // set animations
            _animationManager.Play(_animations["Sprite"]);
        }

        /// <summary>
        /// Update method for weapon - only updates if there is an action and weapon is equipped.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (IsAction == false)
                return;

            if (IsEquipped == false)
                return;

            // update timer
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // update action
            UpdateAction();

            // check collision
            CheckCollision(sprites);

            // update weapon movement
            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        /// <summary>
        /// Draw method for weapon - only draws if there is an action and weapon is equipped.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
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
