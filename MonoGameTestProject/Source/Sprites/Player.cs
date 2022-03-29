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
    /// Player Class - The Player object that the user controls.
    /// Inheritance Order: Player -> Sprite -> Component
    /// </summary>
    public class Player : Sprite
    {
        #region Members

        // The Directions that Player can face
        public enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }
        // Direction that Player faces
        public Directions PlayerDirection;

        #endregion Members


        #region Methods

        /// <summary>
        /// Player constructor.
        /// Initializes Input Keys and Properties like Player speed.
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="spawnPosition"></param>
        public Player(Dictionary<string, Animation> animations, Vector2 spawnPosition, Attributes baseAttributes) : base(animations)
        {
            Position = spawnPosition;

            BaseAttributes = baseAttributes;

            // set player keybindings
            Input = new Input()
            {
                Up = Keys.Up,
                Down = Keys.Down,
                Left = Keys.Left,
                Right = Keys.Right,
                Dash = Keys.Space,
                Attack = Keys.Q,
                Special1 = Keys.W,
                Special2 = Keys.E,
                Item = Keys.R,
                Interact = Keys.F
            };

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
                if (Keyboard.GetState().IsKeyDown(Input.Dash))
                {
                    Console.WriteLine("Player is Dashing");
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Input.Attack))
                {
                    Console.WriteLine("Player is Attacking");
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Input.Special1))
                {
                    Console.WriteLine("Player is Casting Special 1");
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Input.Special2))
                {
                    Console.WriteLine("Player is Casting Special 2");
                    return;
                }

                if (Keyboard.GetState().IsKeyDown(Input.Item))
                {
                    Console.WriteLine("Player is Using Item");
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

        #endregion Methods
    }
}
