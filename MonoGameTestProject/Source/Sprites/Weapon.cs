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

        private float _timer;

        public float ActionSpeed;

        public Vector2 InitPosition { get; set; }

        public Vector2 FinalPosition { get; set; }

        #endregion Members

        #region Methods

        /// <summary>
        /// Weapon constructor - made up of a weapon texture with baseAttributes for weapon stats.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="baseAttributes"></param>
        public Weapon(Texture2D texture, Attributes baseAttributes) : base(texture)
        {
            BaseAttributes = baseAttributes;
        }

        protected virtual void PickUp(Player parent)
        {
            if (parent != Globals.player)
                return;

            Parent = parent;
            parent.AttributeModifiers.Add(BaseAttributes);
            ActionSpeed = parent.AttackSpeed;
        }

        protected virtual void Action()
        {
            if (Parent == null)
                return;

            var parentPosition = Parent.Position;
            var parentDirection = Parent.Direction;

            if (parentDirection == Directions.Down)
            {
                InitPosition = new Vector2(Parent.Position.X, Parent.Position.Y + 16);
                FinalPosition = new Vector2(Parent.Position.X, Parent.Position.Y + 16 + _texture.Height);
            } 
            else if (parentDirection == Directions.Up)
            {
                InitPosition = new Vector2(Parent.Position.X, Parent.Position.Y - 16);
                FinalPosition = new Vector2(Parent.Position.X, Parent.Position.Y - 16 - _texture.Height);
            } 
            else if (parentDirection == Directions.Right)
            {
                InitPosition = new Vector2(Parent.Position.X + 16, Parent.Position.Y);
                FinalPosition = new Vector2(Parent.Position.X + 16 + _texture.Height, Parent.Position.Y);
            }
            else if (parentDirection == Directions.Right)
            {
                InitPosition = new Vector2(Parent.Position.X - 16, Parent.Position.Y);
                FinalPosition = new Vector2(Parent.Position.X - 16 - _texture.Height, Parent.Position.Y);
            }
        }

        #endregion Methods
    }
}
