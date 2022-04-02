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
using TiledSharp;
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

        #region Members - Action
        
        // attack timer to check cooldown and animation length
        private float _attackTimer = 0;

        // item timer to check cooldown and animation length
        private float _itemTimer = 0;

        // special1 timer to check cooldown and animation length
        private float _special1Timer = 0;

        // special2 timer to check cooldown and animation length
        private float _special2Timer = 0;

        // jump timer to check cooldown and animation length
        private float _jumpTimer = 0;

        // interact timer to check cooldown and animation length
        private float _interactTimer = 0;

        private float _switchTimer = 0;

        // attack cooldown before next attack action can be set
        private float _attackCooldown = 0.5f;

        // item cooldown before next item action can be set
        private float _itemCooldown = 1f;

        // jump cooldown before next jump action can be set
        private float _jumpCooldown = 1f;

        // special1 cooldown before next special1 action can be set
        private float _special1Cooldown = 1f;

        // special2 cooldown before next special2 action can be set
        private float _special2Cooldown = 1f;

        // interact cooldown before next interact action can be set
        private float _interactCooldown = 1f;

        private float _switchCooldown = 1f;

        public float AttackSpeed = 0.25f;
        public float JumpSpeed = 0.25f;
        public float ItemSpeed = 0.75f;
        public float Special1Speed = 0.75f;
        public float Special2Speed = 0.75f;

        // if true, perform Attack action during Update
        public bool IsActionAttack = false;

        // if true, perform Interact action during Update
        public bool IsActionInteract = false;

        // if true, perform Item action during Update
        public bool IsActionItem = false;

        // if true, perform Jump action during Update
        public bool IsActionJump = false;

        // if true, perform Special1 action during Update
        public bool IsActionSpecial1 = false;

        // if true, perform Special2 action during Update
        public bool IsActionSpecial2 = false;

        // Check if any actions are being performed
        public bool IsAction
        {
            get
            {
                return IsActionAttack || IsActionInteract || IsActionItem ||
                    IsActionJump || IsActionSpecial1 || IsActionSpecial2;
            }
        }

        #endregion Members - Action


        // Check if Player is Dead
        public bool IsDead
        {
            get { return Health <= 0; }
        }

        public List<Weapon> WeaponInventory;

        public Weapon EquippedWeapon { get; set; }

        public int WeaponIndex = 0;


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
                Interact = Keys.F,
                SwitchWeapon = Keys.D1,
                SwitchItem = Keys.D2,
            };

            // set animation length for all actions
            _animations["AttackDown"].FrameSpeed = AttackSpeed;
            _animations["AttackUp"].FrameSpeed = AttackSpeed;
            _animations["AttackLeft"].FrameSpeed = AttackSpeed;
            _animations["AttackRight"].FrameSpeed = AttackSpeed;
            _animations["JumpDown"].FrameSpeed = JumpSpeed;
            _animations["JumpUp"].FrameSpeed = JumpSpeed;
            _animations["JumpLeft"].FrameSpeed = JumpSpeed;
            _animations["JumpRight"].FrameSpeed = JumpSpeed;
            _animations["Item"].FrameSpeed = ItemSpeed;
            _animations["Special1"].FrameSpeed = Special1Speed;
            _animations["Special2"].FrameSpeed = Special2Speed;


            // default player direction is facing down
            Direction = Directions.Down;

            WeaponInventory = new List<Weapon>();

        }

        #region Methods - Weapon methods
        public void PickUp(Weapon weapon)
        {
            weapon.PickUp(this);
            WeaponInventory.Add(weapon);

            if (WeaponInventory.Count == 1)
            {
                this.Equip(weapon);
            }
        }

        public void Equip(Weapon weapon)
        {
            EquippedWeapon = weapon;
            EquippedWeapon.Equip();
            Console.WriteLine("Weapon Attack:  " + TotalAttributes.Attack);
        }

        public void Unequip()
        {
            EquippedWeapon.Unequip();
        }

        public void SwitchWeapon()
        {
            if (WeaponInventory.Count <= 1)
                return;

            if (WeaponIndex == WeaponInventory.Count - 1)
                WeaponIndex = 0;
            else
                WeaponIndex++;

            Unequip();
            Equip(EquippedWeapon = WeaponInventory[WeaponIndex]);
        }

        #endregion Methods - Weapon methods

        #region Methods - Update Methods

        /// <summary>
        /// UpdateInput method - Gets Keyboard input and checks for Action/Movement.
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

        /// <summary>
        /// UpdateAction method - Checks for Action input.
        /// </summary>
        private void UpdateAction()
        {
            // 1. Start the action if key is pressed and action cooldown is done
            if (_currentKey.IsKeyDown(Input.Jump) && _jumpTimer > _jumpCooldown)
            {
                _jumpTimer = 0f;
                IsActionJump = true;
            }
            else if (_currentKey.IsKeyDown(Input.Attack) && _attackTimer > _attackCooldown)
            {
                _attackTimer = 0f;
                IsActionAttack = true;
                EquippedWeapon.Action();
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
            } else if (_currentKey.IsKeyDown(Input.SwitchWeapon) && _switchTimer > _switchCooldown)
            {
                _switchTimer = 0f;
                SwitchWeapon();
            }

            // Quick check to see if any actions are in progress
            if (!IsAction)
                return;

            // 2. Stop the action if the animation is done.
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
        /// UpdateMovement method - Checks for Movement input.
        /// </summary>
        protected void UpdateMovement()
        {
            // if an action is in progress, player cannot move yet
            if (IsAction)
                return;

            // Check movements up, down, left, right
            if (_currentKey.IsKeyDown(Input.Up))
            {
                Velocity.Y = -Speed;
                Direction = Directions.Up;
            }
            else if (_currentKey.IsKeyDown(Input.Down))
            {
                Velocity.Y = Speed;
                Direction = Directions.Down;
            }
            if (_currentKey.IsKeyDown(Input.Left))
            {
                Velocity.X = -Speed;
                Direction = Directions.Left;
            }
            else if (_currentKey.IsKeyDown(Input.Right))
            {
                Velocity.X = Speed;
                Direction = Directions.Right;
            }
        }

        /// <summary>
        /// UpdateTimers method - increments all action timers.
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateTimers(GameTime gameTime)
        {
            _jumpTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _interactTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _itemTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _special1Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _special2Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _switchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        #endregion Methods - Update Methods

        #region Methods - Action Events

        /// <summary>
        /// SetAction method.
        /// </summary>
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

        /// <summary>
        /// SetActionJump method - update and animate Jump action.
        /// </summary>
        protected void SetActionJump()
        {

            if (Direction == Directions.Up)
            {
                Velocity.Y = -Speed * 2;
                _animationManager.Play(_animations["JumpUp"]);
            }
            else if (Direction == Directions.Down)
            {
                Velocity.Y = Speed * 2;
                _animationManager.Play(_animations["JumpDown"]);
            } 
            else if (Direction == Directions.Left)
            {
                Velocity.X = -Speed * 2;
                _animationManager.Play(_animations["JumpLeft"]);
            } 
            else if (Direction == Directions.Right)
            {
                Velocity.X = Speed * 2;
                _animationManager.Play(_animations["JumpRight"]);
            }
        }

        /// <summary>
        /// SetActionAttack method - update and animate Attack action.
        /// </summary>
        protected void SetActionAttack()
        {
            if (Direction == Directions.Up)
                _animationManager.Play(_animations["AttackUp"]);
            else if (Direction == Directions.Down)
                _animationManager.Play(_animations["AttackDown"]);
            else if (Direction == Directions.Left)
                _animationManager.Play(_animations["AttackLeft"]);
            else if (Direction == Directions.Right)
                _animationManager.Play(_animations["AttackRight"]);
            
            // todo
            // weapon update animations and oncollides between weapon and enemy
        }

        /// <summary>
        /// SetActionSpecial1 method - update and animate Special1 action.
        /// </summary>
        protected void SetActionSpecial1()
        {
            _animationManager.Play(_animations["Special1"]);
            Colour = Color.Blue;
        }

        /// <summary>
        /// SetActionSpecial2 method - update and animate Special2 action.
        /// </summary>
        protected void SetActionSpecial2()
        {
            _animationManager.Play(_animations["Special2"]);
            Colour = Color.Green;
        }

        /// <summary>
        /// SetActionItem method - update and animate Item action.
        /// </summary>
        protected void SetActionItem()
        {
            _animationManager.Play(_animations["Item"]);
        }

        /// <summary>
        /// SetActionInteract method - update and animate Interact action.
        /// </summary>
        protected void SetActionInteract()
        {
            if (Direction == Directions.Up)
                _animationManager.Play(_animations["IdleUp"]);
            else if (Direction == Directions.Down)
                _animationManager.Play(_animations["IdleDown"]);
            else if (Direction == Directions.Left)
                _animationManager.Play(_animations["IdleLeft"]);
            else if (Direction == Directions.Right)
                _animationManager.Play(_animations["IdleRight"]);
        }

        #endregion Methods - Action Events


        /// <summary>
        /// (Player) SetAnimations method - set movement animations.
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

        /// <summary>
        /// SetIdleAnimations - reset player animations and play idle animation.
        /// </summary>
        protected void SetIdleAnimations()
        {
            if (IsAction)
                return;

            Colour = Color.White;

            if (Direction == Directions.Up)
                _animationManager.Play(_animations["IdleUp"]);
            else if (Direction == Directions.Down)
                _animationManager.Play(_animations["IdleDown"]);
            else if (Direction == Directions.Left)
                _animationManager.Play(_animations["IdleLeft"]);
            else if (Direction == Directions.Right)
                _animationManager.Play(_animations["IdleRight"]);
        }

        /// <summary>
        /// (Player) OnCollide method.
        /// </summary>
        /// <param name="sprite"></param>
        public override void OnCollide(Sprite sprite)
        {
            Console.WriteLine("Player.OnCollide method called.");
        }

        /// <summary>
        /// Update method.
        /// The player update method also invokes methods to detect player action/movement/collision.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (IsDead)
                return;

            UpdateTimers(gameTime);

            UpdateInput();

            SetAction();

            SetAnimations();

            _animationManager.Update(gameTime, sprites);

            EquippedWeapon.Update(gameTime, sprites);

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

            if (IsActionAttack)
                EquippedWeapon.Draw(gameTime, spriteBatch);

            base.Draw(gameTime, spriteBatch);

        }

        #endregion Methods
    }
}
