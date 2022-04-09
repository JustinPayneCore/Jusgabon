﻿#region Includes
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

        // timers to check cooldowns and animation length
        private float _attackTimer = 0;
        private float _itemTimer = 0;
        private float _jumpTimer = 0;
        private float _special1Timer = 0;
        private float _special2Timer = 0;
        private float _interactTimer = 0;
        private float _switchTimer = 0;
        private float _manaTimer = 0;
        private float _staminaTimer = 0;

        // cooldowns before action can be done again
        private float _attackCooldown = 0.5f;
        private float _itemCooldown = 1f;
        private float _jumpCooldown = 0.75f;
        private float _special1Cooldown = 0.75f;
        private float _special2Cooldown = 1f;
        private float _interactCooldown = 1f;
        private float _switchCooldown = 0.5f;

        // Mana/Stamina regen at a rate of (1 / this value) per second
        public float ManaRegenCooldown = 0.10f;
        public float StaminaRegenCooldown = 0.10f;

        // animation length of actions
        public float AttackAnimationLength = 0.25f;
        public float JumpAnimationLength = 0.25f;
        public float ItemAnimationLength = 0.75f;
        public float Special1AnimationLength = 0.375f;
        public float Special2AnimationLength = 0.5f;

        // when true, perform action during Update
        public bool IsActionAttack = false;
        public bool IsActionInteract = false;
        public bool IsActionItem = false;
        public bool IsActionJump = false;
        public bool IsActionSpecial1 = false;
        public bool IsActionSpecial2 = false;

        // action stamina/mana costs
        private int _jumpStaminaCost = 30;
        private int _special1ManaCost = 30;
        private int _special2ManaCost = 45;

        // special 1 spell properties
        public int Special1Magic { get; set; }
        public float Special1Speed { get; set; }
        public float Special1LifeSpan { get; set; }
        public Spell Special1 { get; set; }

        // special 2 spell properties
        public int Special2Magic { get; set; }
        public float Special2Speed { get; set; }
        public float Special2LifeSpan { get; set; }
        public Spell Special2 { get; set; }



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

        // current mana of player, will regenerate until it reaches value of Mana attribute
        public int currentMana;
        
        // current stamina of player, will regenerate until it reaches value of Stamina attribute
        public int currentStamina;

        // Check if Player is Dead
        public bool IsDead
        {
            get { return currentHealth <= 0; }
        }

        // Weapon Inventory
        public List<Weapon> WeaponInventory;

        // Currently equipped weapon
        public Weapon EquippedWeapon { get; set; }
        
        // Index to track which weapon from the inventory is equipped
        public int WeaponIndex = 0;

        // Gold in-game currency
        public int Gold = 0;

        #endregion Members


        #region Methods

        /// <summary>
        /// Player constructor.
        /// - set input keybindings
        /// - set action animation lengths
        /// - get/set player properties
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="baseAttributes"></param>
        public Player(Dictionary<string, Animation> animations,  Attributes baseAttributes) : base(animations, baseAttributes)
        {
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
            _animations["AttackDown"].FrameSpeed = AttackAnimationLength;
            _animations["AttackUp"].FrameSpeed = AttackAnimationLength;
            _animations["AttackLeft"].FrameSpeed = AttackAnimationLength;
            _animations["AttackRight"].FrameSpeed = AttackAnimationLength;
            _animations["JumpDown"].FrameSpeed = JumpAnimationLength;
            _animations["JumpUp"].FrameSpeed = JumpAnimationLength;
            _animations["JumpLeft"].FrameSpeed = JumpAnimationLength;
            _animations["JumpRight"].FrameSpeed = JumpAnimationLength;
            _animations["Item"].FrameSpeed = ItemAnimationLength;
            _animations["Special1"].FrameSpeed = Special1AnimationLength;
            _animations["Special2"].FrameSpeed = Special2AnimationLength;

            // default player direction is facing down
            Direction = Directions.Down;

            // get current stamina and mana values
            currentMana = Mana;
            currentStamina = Stamina;

            // create weapon inventory
            WeaponInventory = new List<Weapon>();

            // change default Take Hit animation length & cooldowns
            _hitCooldown = 1f;
            HitSpeed = 0.15f;
        }

        /// <summary>
        /// GetGold method to give player gold.
        /// </summary>
        /// <param name="goldAmount"></param>
        public void GetGold(int goldAmount)
        {
            Gold += goldAmount;
            Console.WriteLine("\nPlayer Gold: " + Gold + "\n");
        }

        #region Methods - Weapon methods

        /// <summary>
        /// PickUp method to add Weapon to inventory.
        /// If this is the first weapon in the inventory, then equip it.
        /// </summary>
        /// <param name="weapon"></param>
        public void PickUp(Weapon weapon)
        {
            weapon.PickUp(this);
            WeaponInventory.Add(weapon);

            if (WeaponInventory.Count == 1)
            {
                this.Equip(weapon);
            }
        }

        /// <summary>
        /// Equip Weapon method.
        /// </summary>
        /// <param name="weapon"></param>
        public void Equip(Weapon weapon)
        {
            EquippedWeapon = weapon;
            EquippedWeapon.Equip();
            Console.WriteLine("\nPlayer Stats");
            Console.WriteLine("Attack:  " + TotalAttributes.Attack);
            Console.WriteLine("Magic:   " + TotalAttributes.Magic);
            Console.WriteLine("Mana:    " + TotalAttributes.Mana);
            Console.WriteLine("Speed:   " + TotalAttributes.Speed);
            Console.WriteLine("Stamina: " + TotalAttributes.Stamina + "\n");
            if (currentHealth >= Health)
                currentHealth = Health;
            if (currentMana >= Mana)
                currentMana = Mana;
            if (currentStamina >= Stamina)
                currentStamina = Stamina;
        }

        /// <summary>
        /// Unequip Weapon method.
        /// </summary>
        public void Unequip()
        {
            EquippedWeapon.Unequip();
        }

        /// <summary>
        /// SwitchWeapon method - Equip next weapon in Weapon Inventory.
        /// </summary>
        public void SwitchWeapon()
        {
            // can not switch if there is only one weapon
            if (WeaponInventory.Count <= 1)
                return;

            // increment weapon inventory index
            if (WeaponIndex == WeaponInventory.Count - 1)
                WeaponIndex = 0;
            else
                WeaponIndex++;

            // unequip current weapon
            Unequip();

            Console.WriteLine("\nSwitching Weapon...\n");
            // equip next weapon in inventory
            Equip(EquippedWeapon = WeaponInventory[WeaponIndex]);
        }

        #endregion Methods - Weapon methods

        #region Methods - Specials methods

        /// <summary>
        /// Set Special 1 method - sets spell and action properties for special1 action.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="magic"></param>
        /// <param name="speed"></param>
        /// <param name="lifespan"></param>
        public void SetSpecial1(Spell spell, int magic, float speed, float lifespan)
        {
            Special1 = spell;
            Special1Magic = magic;
            Special1Speed = speed;
            Special1LifeSpan = lifespan;
        }

        /// <summary>
        /// Set Special 2 method - sets spell and action properties for special2 action.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="magic"></param>
        /// <param name="speed"></param>
        /// <param name="lifespan"></param>
        public void SetSpecial2(Spell spell, int magic, float speed, float lifespan)
        {
            Special2 = spell;
            Special2Magic = magic;
            Special2Speed = speed;
            Special2LifeSpan = lifespan;
        }

        #endregion Methods - Specials methods

        /// <summary>
        /// UpdateRegen method - recovers missing mana and stamina of player.
        /// </summary>
        /// <param name="gametime"></param>
        protected void UpdateRegen(GameTime gametime)
        {
            if (_staminaTimer > StaminaRegenCooldown && currentStamina < Stamina)
            {
                _staminaTimer = 0f;
                currentStamina++;
            }
            if (_manaTimer > ManaRegenCooldown && currentMana < Mana)
            {
                _manaTimer = 0f;
                currentMana++;
            }
        }

        #region Methods - Check Input Methods

        /// <summary>
        /// UpdateInput method - Gets Keyboard input then checks for Action/Movement input.
        /// </summary>
        protected void UpdateInput()
        {
            _previousKey = _currentKey;
            _currentKey = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.None))
                return;

            StartAction();
            StopAction();
            CheckMovement();
        }

        /// <summary>
        /// StartAction method - Checks for Action input to start an action.
        /// </summary>
        private void StartAction()
        {
            // Don't look for another action to start
            if (IsAction)
                return;

            // Start the action if key is pressed and action cooldown is done
            if (_currentKey.IsKeyDown(Input.Jump) && _jumpTimer > _jumpCooldown && currentStamina >= _jumpStaminaCost)
            {
                _jumpTimer = 0f;
                IsActionJump = true;
                currentStamina -= _jumpStaminaCost;
            }
            else if (_currentKey.IsKeyDown(Input.Attack) && _attackTimer > _attackCooldown)
            {
                _attackTimer = 0f;
                IsActionAttack = true;
                EquippedWeapon.Action();
            }
            else if (_currentKey.IsKeyDown(Input.Special1) && _special1Timer > _special1Cooldown && currentMana >= _special1ManaCost)
            {
                _special1Timer = 0f;
                IsActionSpecial1 = true;
                currentMana -= _special1ManaCost;

                // create special1 instance object
                var special1 = Special1.Clone() as Spell;
                special1.Parent = this;
                special1.Magic = this.Magic + Special1Magic;
                special1.Speed = Special1Speed;
                special1.LifeSpan = Special1LifeSpan;
                Children.Add(special1);
                
                // cast special1's action
                special1.Action();
            }
            else if (_currentKey.IsKeyDown(Input.Special2) && _special2Timer > _special2Cooldown && currentMana >= _special2ManaCost)
            {
                _special2Timer = 0f;
                IsActionSpecial2 = true;
                currentMana -= _special2ManaCost;

                // create special2 instance object
                var special2 = Special2.Clone() as Spell;
                special2.Parent = this;
                special2.Magic = this.Magic + Special2Magic;
                special2.Speed = Special2Speed;
                special2.LifeSpan = Special2LifeSpan;
                Children.Add(special2);

                // cast special2's action
                special2.Action();
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
            else if (_currentKey.IsKeyDown(Input.SwitchWeapon) && _switchTimer > _switchCooldown)
            {
                _switchTimer = 0f;
                SwitchWeapon();
            }
        }

        /// <summary>
        /// EndAction method - Checks to stop a current action.
        /// </summary>
        private void StopAction()
        {
            // Don't need to check if no actions are in progress
            if (!IsAction)
                return;

            // Stop the action if the animation is done.
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
        /// CheckMovement method - Checks for Movement input.
        /// </summary>
        protected void CheckMovement()
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

        #endregion Methods - Set Input Methods

        #region Methods - Update Action Events

        /// <summary>
        /// SetAction method - update any current actions.
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
                SetActionAttack();
            else if (IsActionSpecial2)
                SetActionAttack();
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
        }

        /// <summary>
        /// SetActionSpecial1 method - update and animate Special1 action.
        /// </summary>
        protected void SetActionSpecial1()
        {
            _animationManager.Play(_animations["Special1"]);
        }

        /// <summary>
        /// SetActionSpecial2 method - update and animate Special2 action.
        /// </summary>
        protected void SetActionSpecial2()
        {
            _animationManager.Play(_animations["Special2"]);
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
            _staminaTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _manaTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        #endregion Methods - Update Action Events


        /// <summary>
        /// SetAnimations method (Player) - set movement animations.
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
        /// SetIdleAnimations method - reset player animations and play idle animation.
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
        /// OnCollide method (Player).
        /// </summary>
        /// <param name="sprite"></param>
        public override void OnCollide(Sprite sprite)
        {
            if (IsDead)
                return;
            
            if (sprite is Enemy)
                TakeHit(((Enemy)sprite).Attack);

            if (sprite is Spell && ((Spell)sprite).Parent is Enemy)
            {
                TakeHit(((Spell)sprite).Magic);
            }
        }

        /// <summary>
        /// SetTakeHit method (Player) - update take hit method.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void SetTakeHit(GameTime gameTime)
        {
            // increment timers
            _hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // check if sprite is taking hit
            if (IsHit == false)
                return;

            // set take hit properties
            Colour = Color.Red;
            Velocity = Vector2.Zero;

            // reset sprite take hit cooldown
            if (_hitTimer > HitSpeed)
            {
                Colour = Color.White;
                IsHit = false;
            }
        }

        /// <summary>
        /// Update method (Player).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            if (IsDead)
            {
                _animationManager.Play(_animations["Dead"]);
                return;
            }

            // General Order of Updates
            // 1. Update Timers
            // 2. Update Input
            // 3. Set Input Actions/Animations
            // 4. Update Animation Manager
            // 5. Update Child
            // 6. Check Collision
            // 7. Check Take Damage Action
            // 8. Update Position

            UpdateTimers(gameTime);

            UpdateRegen(gameTime);

            UpdateInput();

            SetAction();
            SetAnimations();

            _animationManager.Update(gameTime, sprites);

            EquippedWeapon.Update(gameTime, sprites);

            CheckCollision(sprites);

            SetTakeHit(gameTime);

            Position += Velocity;
            Velocity = Vector2.Zero;
        }

        /// <summary>
        /// Draw method (Player).
        /// The Draw method also draws any action items.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsActionAttack)
                EquippedWeapon.Draw(gameTime, spriteBatch);

            base.Draw(gameTime, spriteBatch);
        }

        #endregion Methods
    }
}
