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

        private float _attackTimer = 0;

        private float _itemTimer = 0;

        private float _special1Timer = 0;

        private float _special2Timer = 0;

        private float _jumpTimer = 0;

        private float _interactTimer = 0;

        private float _attackCooldown = 1f;
        private float _itemCooldown = 1f;
        private float _jumpCooldown = 1f;
        private float _special1Cooldown = 1f;
        private float _special2Cooldown = 1f;
        private float _interactCooldown = 1f;

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

        public bool IsAction
        {
            get 
            {
                return IsActionAttack || IsActionInteract || IsActionItem ||
                    IsActionJump || IsActionSpecial1 || IsActionSpecial2;
            }
        }

        public bool IsActionAttack = false;

        public bool IsActionInteract = false;

        public bool IsActionItem = false;

        public bool IsActionJump = false;

        public bool IsActionSpecial1 = false;

        public bool IsActionSpecial2 = false;

        // Check if Player is Dead
        public bool IsDead
        {
            get { return Health <= 0; }
        }


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
                Jump = Keys.Space,
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
        protected void UpdateInput()
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.None))
                return;

            UpdateAction();
            UpdateMovement();
        }

        private void UpdateAction()
        {
            // 1. get keyboard input to set action
            if (_currentKey.IsKeyDown(Input.Jump) && _jumpTimer > _jumpCooldown)
            {
                _jumpTimer = 0f;
                IsActionJump = true;
            }
            else if (_currentKey.IsKeyDown(Input.Attack) && _attackTimer > _attackCooldown)
            {
                _attackTimer = 0f;
                IsActionAttack = true;
            }
            else if (_currentKey.IsKeyDown(Input.Special1) && _special1Timer > _special1Cooldown)
            {
                _special1Timer = 0f;
                IsActionSpecial1 = true;
            }
            else if (_currentKey.IsKeyDown(Input.Special2) && _special2Timer > _special2Cooldown)
            {
                _special2Timer = 0f;
                IsActionSpecial2 = true;
            }
            else if (_currentKey.IsKeyDown(Input.Item) && _itemTimer > _itemCooldown)
            {
                _itemTimer = 0f;
                IsActionItem = true;
            }
            else if (_currentKey.IsKeyDown(Input.Interact) && _interactTimer > _interactCooldown)
            {
                _interactTimer = 0f;
                IsActionInteract = true;
            }

            // 2. check if action event is done
            if (IsActionJump && _jumpTimer > _animations["JumpDown"].FrameSpeed)
            {
                IsActionJump = false;
                SetIdleAnimations();
            } 
            else if (IsActionAttack && _attackTimer > _animations["AttackDown"].FrameSpeed)
            {
                IsActionAttack = false;
                SetIdleAnimations();
            }
            else if (IsActionSpecial1 && _special1Timer > _animations["Special1"].FrameSpeed)
            {
                IsActionSpecial1 = false;
                SetIdleAnimations();
            }
            else if (IsActionSpecial2 && _special2Timer > _animations["Special2"].FrameSpeed)
            {
                IsActionSpecial2 = false;
                SetIdleAnimations();
            }
            else if (IsActionItem && _itemTimer > _animations["Item"].FrameSpeed)
            {
                IsActionItem = false;
                SetIdleAnimations();
            }
            else if (IsActionInteract && _interactTimer > _animations["IdleDown"].FrameSpeed)
            {
                IsActionInteract = false;
                SetIdleAnimations();
            }
        }

        /// <summary>
        /// CheckMovement method.
        /// Checks Keyboard input for player movement.
        /// </summary>
        protected void UpdateMovement()
        {
            if (IsAction)
                return;

            if (_currentKey.IsKeyDown(Input.Up))
            {
                Velocity.Y = -Speed;
                PlayerDirection = Directions.Up;
            }
            else if (_currentKey.IsKeyDown(Input.Down))
            {
                Velocity.Y = Speed;
                PlayerDirection = Directions.Down;
            }
            if (_currentKey.IsKeyDown(Input.Left))
            {
                Velocity.X = -Speed;
                PlayerDirection = Directions.Left;
            }
            else if (_currentKey.IsKeyDown(Input.Right))
            {
                Velocity.X = Speed;
                PlayerDirection = Directions.Right;
            }
        }

        protected void IncrementTimers(GameTime gameTime)
        {
            _jumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _interactTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _itemTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _special1Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _special2Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        #region Methods - Action Events

        private void SetAction()
        {
            if (!IsAction)
                return;

            if (IsActionJump)
                SetActionJump();
            else if (IsActionAttack)
                SetActionAttack();
            else if (IsActionSpecial1)
                SetActionSpecial1();
            else if (IsActionSpecial2)
                SetActionSpecial2();
            else if (IsActionItem)
                SetActionItem();
            else if (IsActionInteract)
                SetActionInteract();
        }

        protected void SetActionJump()
        {

            if (PlayerDirection == Directions.Up)
            {
                Velocity.Y = -Speed * 2;
                _animationManager.Play(_animations["JumpUp"]);
            }
            else if (PlayerDirection == Directions.Down)
            {
                Velocity.Y = Speed * 2;
                _animationManager.Play(_animations["JumpDown"]);
            } 
            else if (PlayerDirection == Directions.Left)
            {
                Velocity.X = -Speed * 2;
                _animationManager.Play(_animations["JumpLeft"]);
            } 
            else if (PlayerDirection == Directions.Right)
            {
                Velocity.X = Speed * 2;
                _animationManager.Play(_animations["JumpRight"]);
            }
        }

        protected void SetActionAttack()
        {
            if (PlayerDirection == Directions.Up)
                _animationManager.Play(_animations["AttackUp"]);
            else if (PlayerDirection == Directions.Down)
                _animationManager.Play(_animations["AttackDown"]);
            else if (PlayerDirection == Directions.Left)
                _animationManager.Play(_animations["AttackLeft"]);
            else if (PlayerDirection == Directions.Right)
                _animationManager.Play(_animations["AttackRight"]);
        }

        protected void SetActionSpecial1()
        {
            _animationManager.Play(_animations["Special1"]);
        }

        protected void SetActionSpecial2()
        {
            _animationManager.Play(_animations["Special2"]);
        }

        protected void SetActionItem()
        {
            _animationManager.Play(_animations["Item"]);
        }

        protected void SetActionInteract()
        {
            if (PlayerDirection == Directions.Up)
                _animationManager.Play(_animations["IdleUp"]);
            else if (PlayerDirection == Directions.Down)
                _animationManager.Play(_animations["IdleDown"]);
            else if (PlayerDirection == Directions.Left)
                _animationManager.Play(_animations["IdleLeft"]);
            else if (PlayerDirection == Directions.Right)
                _animationManager.Play(_animations["IdleRight"]);
        }

        #endregion Methods - Action Events


        /// <summary>
        /// SetAnimations Method (Overridden in Player).
        /// Set which animation to use depending on player state/input.
        /// </summary>
        protected override void SetAnimations()
        {
            if (IsAction)
                return;

            if (Velocity.X > 0)
                _animationManager.Play(_animations["WalkRight"]);
            else if (Velocity.X < 0)
                _animationManager.Play(_animations["WalkLeft"]);
            else if (Velocity.Y > 0)
                _animationManager.Play(_animations["WalkDown"]);
            else if (Velocity.Y < 0)
                _animationManager.Play(_animations["WalkUp"]);
            else
                _animationManager.Stop();
        }

        protected void SetIdleAnimations()
        {
            if (IsAction)
                return;
            
            if (PlayerDirection == Directions.Up)
                _animationManager.Play(_animations["IdleUp"]);
            else if (PlayerDirection == Directions.Down)
                _animationManager.Play(_animations["IdleDown"]);
            else if (PlayerDirection == Directions.Left)
                _animationManager.Play(_animations["IdleLeft"]);
            else if (PlayerDirection == Directions.Right)
                _animationManager.Play(_animations["IdleRight"]);
        }

        public override void OnCollide(Sprite sprite)
        {
            Console.WriteLine("Player.OnCollide method called.");
        }

        /// <summary>
        /// Update method.
        /// The player update method also invokes methods to detect player movement/collision.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (IsDead)
                return;

            IncrementTimers(gameTime);

            UpdateInput();

            SetAction();

            SetAnimations();

            _animationManager.Update(gameTime, sprites);

            CheckCollision(sprites);

            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        /// <summary>
        /// Draw method.
        /// The player method only overrides this to check player isn't drawn if it is dead.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsDead)
                return;

            base.Draw(gameTime, spriteBatch);
        }

        #endregion Methods
    }
}
