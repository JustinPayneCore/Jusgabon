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
    public class Enemy : Sprite
    {
        #region Members

        // Distance for Sprite to get aggro and follow target
        public float AggroDistance { get; set; } = 90f;

        // Modifier to increase/decrease aggro once following target
        public float AggroModifier = 2;

        // If sprite is currently aggro'd to target
        public bool IsAggro { get; set; }

        #endregion Members


        #region Methods

        /// <summary>
        /// Enemy Constructor.
        /// Overrides base Sprite constructor with an Animation Dictionary.
        /// </summary>
        /// <param name="animations"></param>
        public Enemy(Dictionary<string, Animation> animations) : base(animations)
        {
            // set aggro target to player & follow distance of 16 -> player width/height
            FollowTarget = Globals.player;
            FollowDistance = 16f;
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

        protected override void Follow()
        {
            if (FollowTarget == null)
                return;

            var currentDistance = Vector2.Distance(this.Position, FollowTarget.Position);

            if (currentDistance <= FollowDistance || IsAggressive(currentDistance) == false)
            {
                Velocity = Vector2.Zero;
                return;
            }

            var distance = FollowTarget.Position - this.Position;
            _rotation = (float)Math.Atan2(distance.Y, distance.X);

            Direction = new Vector2((float)Math.Cos(_rotation), (float)Math.Sin(_rotation));

            if (currentDistance > FollowDistance)
            {
                var t = MathHelper.Min((float)Math.Abs(currentDistance - FollowDistance), Speed);
                Velocity = Direction * t;

                //Position += Velocity;
            }
        }

        protected virtual bool IsAggressive(float currentDistance)
        {
            // if current distance is over aggro distance, return false
            if (currentDistance >= AggroDistance)
            {
                // revert aggro distance if monster was already aggro'd
                if (IsAggro == true)
                {
                    IsAggro = false;
                    AggroDistance = (float)(AggroDistance / AggroModifier);
                }

                return false;
            }

            // increase aggro distance once sprite has been aggro'd
            if (IsAggro == false)
            {
                IsAggro = true;
                AggroDistance = (float)(AggroDistance * AggroModifier);
            }

            return true;
        }

        #endregion Methods - Follow Sprite Logic

        /// <summary>
        /// Enemy Update method.
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            base.Update(gameTime, sprites);
        }


        #endregion Methods

        }
}
