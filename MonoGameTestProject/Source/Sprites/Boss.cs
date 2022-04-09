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
    /// Boss Class - Inherits Enemy Class.
    /// Inheritance Order: Boss -> Enemy -> NPC -> Sprite -> Component
    /// </summary>
    public class Boss : Enemy
    {
        #region Members

        // random member to get next attack cooldown
        private Random _random;

        // timers to check cooldowns and animation length
        private float _special1Timer = 0;
        private float _special2Timer = 0;

        // cooldowns before action can be done again
        private float _special1Cooldown = 2f;
        private float _special2Cooldown = 4f;

        // animation length of action
        public float AttackAnimationLength = 0.75f;

        // when true, perform action during Update
        public bool IsActionSpecial1 = false;
        public bool IsActionSpecial2 = false;

        // check if actions are being performed
        public bool IsAction { get => IsActionSpecial1 || IsActionSpecial2; }

        
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

        // boss phases, different phase may change action trait
        public int Phase = 0;

        // health threshold to move to next phase
        // if not set is still -1, then boss does not have that phase
        public int HealthToPhase1 { get; set; } = -1;
        public int HealthToPhase2 { get; set; } = -1;

        #endregion Members


        #region Methods

        /// <summary>
        /// Boss Constructor.
        /// Overrides base Enemy constructor with an Animation Dictionary, optional SpawnPosition Vector2, and optional Attributes.
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="baseAttributes"></param>
        public Boss(Dictionary<string, Animation> animations, Attributes baseAttributes) : base(animations, baseAttributes)
        {
            // default enemy modifier = 1.5f; increase if boss should be "stickier" to player
            AggroModifier = 2f;

            // change default TakeHit animation length & cooldowns
            _hitCooldown = 1f;
            HitSpeed = 0.3f;
            _animations["Hit"].FrameSpeed = HitSpeed / _animations["Hit"].FrameCount;

            // change amount of gold to give on kill
            GoldGiven = 250;

            // change respawn time to be longer
            RespawnTime = 120f;

            // initialize random
            _random = new Random();

        }

        protected override void Follow()
        {
            // don't move while is in action
            if (IsAction)
                return;
            
            base.Follow();
        }

        public override void TakeHit(int hitDamage)
        {
            base.TakeHit(hitDamage);

            CheckPhase();
        }

        /// <summary>
        /// SetTakeDamage method (Boss) - update take hit action.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void SetTakeHit(GameTime gameTime)
        {
            // increment timers
            _hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // check if boss is taking hit
            if (IsHit == false)
                return;

            // set boss hit properties including playing a Hit animation
            _animationManager.Play(_animations["Hit"]);
            HitVelocity = Vector2.Zero;
            Velocity = HitVelocity;

            // reset boss take hit cooldown
            if (_hitTimer > HitSpeed)
            {
                IsHit = false;

                // check if boss is dead
                if (currentHealth <= 0)
                {
                    Kill();
                }
                    
            }
        }

        protected virtual void CheckPhase()
        {
            if (Phase < 1 && currentHealth <= HealthToPhase1)
            {
                Phase = 1;
            } else if (Phase < 2 && currentHealth <= HealthToPhase2)
            {
                Phase = 2;
                _special2Timer = 2f;
            }
        }

        public virtual void SetPhase1Threshold(int health)
        {
            HealthToPhase1 = health;
        }

        public virtual void SetPhase2Threshold(int health)
        {
            HealthToPhase2 = health;
        }


        /// <summary>
        /// Set Special 1 method - sets spell and action properties for special1 action.
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="magic"></param>
        /// <param name="speed"></param>
        /// <param name="lifespan"></param>
        public virtual void SetSpecial1(Spell spell, int magic, float speed, float lifespan)
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
        public virtual void SetSpecial2(Spell spell, int magic, float speed, float lifespan)
        {
            Special2 = spell;
            Special2Magic = magic;
            Special2Speed = speed;
            Special2LifeSpan = lifespan;
        }

        protected virtual void StartAction()
        {
            // Don't look for another action to start
            if (IsAction)
                return;

            // Don't start action if boss is not yet aggro'd
            if (IsAggro == false)
                return;

            // Don't start action if boss has been hit
            if (IsHit)
                return;

            // Start the action if timer is past the action cooldown
            if (_special1Timer > _special1Cooldown)
            {
                _special1Timer = 0f;

                IsActionSpecial1 = true;

                switch (Phase)
                {
                    case 0: // phase 0 - no special attacks
                        break;

                    case 1: // phase 1 attack - special 1
                        // create special1 instance object
                        var special1 = Special1.Clone() as Spell;
                        special1.Parent = this;
                        special1.Magic = this.Magic + Special1Magic;
                        special1.Speed = Special1Speed;
                        special1.LifeSpan = Special1LifeSpan;

                        Children.Add(special1);

                        // cast special1's action
                        special1.Action();

                        // make next attack in 1-2 seconds
                        _special1Cooldown = (float)_random.NextDouble() + 1;

                        break;

                    case 2: // phase 2 attack - continue using special 1 attack
                        // create special1 instance object
                        special1 = Special1.Clone() as Spell;
                        special1.Parent = this;
                        special1.Magic = this.Magic + Special1Magic;
                        special1.Speed = Special1Speed;
                        special1.LifeSpan = Special1LifeSpan;

                        Children.Add(special1);

                        // cast special1's action
                        special1.Action();

                        // make next attack in 2-3 seconds
                        _special1Cooldown = (float)_random.NextDouble() + 2;

                        break;

                    default:
                        break;
                }

                // Start the action if timer is past the action cooldown
                if (_special2Timer > _special2Cooldown)
                {
                    _special2Timer = 0f;

                    IsActionSpecial2 = true;

                    switch (Phase)
                    {
                        case 0: // phase 0 - no special 2 attack
                            break;
                        case 1: // phase 1 - no special 2 attack
                            break;
                        case 2: // phase 2 attack - special 2
                            // create special2 instance object
                            var special2 = Special2.Clone() as Spell;
                            special2.Parent = this;
                            special2.Magic = this.Magic + Special2Magic;
                            special2.Speed = Special2Speed;
                            special2.LifeSpan = Special2LifeSpan;

                            Children.Add(special2);

                            // cast special2's action
                            special2.Action();

                            // make next attack in 4-5 seconds
                            _special2Cooldown = (float)_random.NextDouble() + 4;

                            break;

                        default:
                            break;
                    }
                }
            }
        }


        protected virtual void StopAction()
        {
            // Don't need to check if no actions are in progress
            if (!IsAction)
                return;

            // Stop the action if the animation is done or boss has been hit.
            if (IsActionSpecial1 && (_special1Timer > AttackAnimationLength || IsHit))
            {
                IsActionSpecial1 = false;
                _animationManager.Play(_animations["Idle"]);
            }
            // Stop the action if the animation is done or boss has been hit.
            if (IsActionSpecial2 && (_special2Timer > AttackAnimationLength || IsHit))
            {
                IsActionSpecial2 = false;
                _animationManager.Play(_animations["Idle"]);
            }
        }

        protected virtual void SetAction()
        {
            if (!IsAction)
                return;
            
            if (IsActionSpecial1)
            {
                Velocity = Vector2.Zero;
                _animationManager.Play(_animations["Idle"]);
            }
            if (IsActionSpecial2)
            {
                Velocity = Vector2.Zero;
                _animationManager.Play(_animations["Idle"]);
            }
        }

        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            _special1Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _special2Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            StartAction();
            StopAction();
            SetAction();


            base.Update(gameTime, sprites);
        }




        #endregion

    }
}
