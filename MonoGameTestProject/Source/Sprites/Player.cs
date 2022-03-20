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
    public class Player : Sprite
    {

        public Player(Dictionary<string, Animation> animations, Vector2 spawnPosition) : base(animations)
        {
            Position = spawnPosition;

            Input = new Input()
            {
                Up = Keys.Up,
                Down = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right
            };

            // make player faster than base sprites
            Speed = 2.5f;
        }

        protected void Move()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.None))
                return;

            {
                if (Keyboard.GetState().IsKeyDown(Input.Up))
                    Velocity.Y = -Speed;
                else if (Keyboard.GetState().IsKeyDown(Input.Down))
                    Velocity.Y = Speed;

                if (Keyboard.GetState().IsKeyDown(Input.Left))
                    Velocity.X = -Speed;
                else if (Keyboard.GetState().IsKeyDown(Input.Right))
                    Velocity.X = Speed;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Move();

            SetAnimations();

            _animationManager.Update(gameTime);

            Position += Velocity;
            Velocity = Vector2.Zero;
        }
    }
}
