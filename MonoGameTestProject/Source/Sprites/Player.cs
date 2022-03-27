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
        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }
        public Directions PlayerDirection;

        /// <summary>
        /// Player constructor.
        /// Initializes Input Keys and Properties like Player speed.
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="spawnPosition"></param>
        public Player(Dictionary<string, Animation> animations, Vector2 spawnPosition) : base(animations)
        {
            Position = spawnPosition;

            Input = new Input()
            {
                Up = Keys.Up,
                Down = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right,
                Attack = Keys.A,
                Dash = Keys.Space,
                Magic = Keys.S,
                Interact = Keys.F
            };

            // make player faster than base sprites
            Speed = 2f;

            // default player direction is facing down
            PlayerDirection = Directions.Down;
        }

        /// <summary>
        /// CheckInput method.
        /// Checks Keyboard input for the player.
        /// </summary>
        protected void CheckInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.None))
                return;

            {
                if (Keyboard.GetState().IsKeyDown(Input.Attack))
                {
                    Console.WriteLine("Player is Attacking");
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Input.Magic))
                {
                    Console.WriteLine("Player is Casting Magic");
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Input.Interact))
                {
                    Console.WriteLine("Player is Interacting...");
                    return;
                }

                CheckMovement();
            }
        }

        /// <summary>
        /// CheckMovement method.
        /// Checks Keyboard input for player movement.
        /// </summary>
        protected void CheckMovement()
        {
            if (Keyboard.GetState().IsKeyDown(Input.Up))
            {
                Velocity.Y = -Speed;
                PlayerDirection = Directions.Up;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Down))
            {
                Velocity.Y = Speed;
                PlayerDirection = Directions.Down;
            }

            if (Keyboard.GetState().IsKeyDown(Input.Left))
            {
                Velocity.X = -Speed;
                PlayerDirection = Directions.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Input.Right))
            {
                Velocity.X = Speed;
                PlayerDirection = Directions.Right;
            }
        }

/*        /// <summary>
        /// Check Collision method.
        /// Checks player with all other collidable sprites to detect if they are colliding.
        /// </summary>
        /// <param name="sprites"></param>
        private void CheckCollision(List<Sprite> sprites)
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
        }*/

        /// <summary>
        /// Update method.
        /// The player update method also invokes methods to detect player movement/collision.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            CheckInput();

            SetAnimations();

            _animationManager.Update(gameTime, sprites);

            CheckCollision(sprites);

            Position += Velocity;
            Velocity = Vector2.Zero;
        }
    }
}
