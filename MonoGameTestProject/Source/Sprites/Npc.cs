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
#endregion

namespace Jusgabon
{
    public class Npc : Sprite
    {
        #region Members

        // Random member to get random npc movement
        private Random _random;

        // Clock timer to keep track of random
        private float _randomTimer;

        // Time it takes for next random update
        private float _nextRandomTime;

        // If npc is a non-moving npc (stops random movement)
        private bool _isStationary;
        public bool IsStationary { get => _isStationary; set => _isStationary = value; }

        // The target Sprite that this Sprite wants to follow
        public Sprite FollowTarget { get; set; }

        // How close we want to be to our target
        public float FollowDistance { get; set; }


        #endregion Members


        #region Methods

        /// <summary>
        /// Enemy Constructor.
        /// Overrides base Sprite constructor with an Animation Dictionary.
        /// </summary>
        /// <param name="animations"></param>
        public Npc(Dictionary<string, Animation> animations) : base(animations)
        {
            _random = new Random();
            _randomTimer = 0f;

            // random time from 1f - 2f
            _nextRandomTime = 1f + (float) _random.NextDouble();

        }


        #region Methods - Collision Detection
        /// <summary>
        /// OnCollide method.
        /// Invoked when Enemy Sprite collides with target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        public override void OnCollide(Sprite sprite)
        {

        }

        #endregion Methods - Collision Detection


        #region Methods - Follow Sprite Logic

        public Sprite SetFollowTarget(Sprite followTarget, float followDistance)
        {
            FollowTarget = followTarget;
            FollowDistance = followDistance;

            return this;
        }

        protected virtual void Follow()
        {
            if (FollowTarget == null)
                return;

            var currentDistance = Vector2.Distance(this.Position, FollowTarget.Position);

            var distance = FollowTarget.Position - this.Position;
            _rotation = (float)Math.Atan2(distance.Y, distance.X);

            Direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));

            if (currentDistance > FollowDistance)
            {
                var t = MathHelper.Min((float)Math.Abs(currentDistance - FollowDistance), Speed);
                Velocity = Direction * t;
            }
        }

        #endregion Methods - Follow Sprite Logic


        #region Methods - Random Movement

        protected virtual void RandomMovement(GameTime gameTime)
        {
            if (_isStationary)
                return;

            var currentDistance = Vector2.Distance(this.Position, Globals.player.Position);

            if (currentDistance > 180f)
            {
                Velocity = Vector2.Zero;
                return;
            }

            _randomTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_randomTimer < _nextRandomTime)
                return;

            int random = _random.Next(0, 8);

            if (random <= 3) // No movement
                Velocity = Vector2.Zero;

            if (random == 4) // Up
            {
                Velocity.X = 0;
                Velocity.Y = -Speed / 2;
            }
            if (random == 5) // Down
            {
                Velocity.X = 0;
                Velocity.Y = Speed / 2;
            }
            if (random == 6) // Left
            {
                Velocity.X = -Speed / 2;
                Velocity.Y = 0;
            }
            if (random == 7) // Right
            {
                Velocity.X = Speed / 2;
                Velocity.Y = 0;
            }

            // reset timer
            _randomTimer = 0f;

        }

        #endregion

        /// <summary>
        /// Enemy Update method.
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            // Invoke Follow Sprite logic
            Follow();

            // Generate random NPC movement
            RandomMovement(gameTime);

            base.Update(gameTime, sprites);
        }


        #endregion Methods

    }
}