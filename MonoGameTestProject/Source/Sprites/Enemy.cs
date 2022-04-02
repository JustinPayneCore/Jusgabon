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
    /// Enemy Class - Inherits NPC Class.
    /// Inheritance Order: Enemy -> NPC -> Sprite -> Component
    /// </summary>
    public class Enemy : Npc
    {
        #region Members

        // Distance for Sprite to get aggro and follow target
        public float AggroDistance { get; set; } = 75f;

        // Modifier to increase/decrease aggro once following target
        public float AggroModifier = 1.5f;

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
        /// Invoke this method when Enemy Sprite collides with target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        public override void OnCollide(Sprite sprite)
        {
            //Console.WriteLine("Enemy hp (before): " + this.Health);
            //this.Health -= sprite.Parent.Attack;
            //Console.WriteLine("Enemy hp (after):  " + this.Health);
            //Console.WriteLine("Enemy.OnCollide method called.");
        }

        #endregion Methods - Collision Detection


        #region Methods - Follow Sprite Logic

        /// <summary>
        /// Follow the Player.
        /// If this Enemy is not yet Aggro'd, then this method returns.
        /// </summary>
        protected override void Follow()
        {
            var currentDistance = Vector2.Distance(this.Position, FollowTarget.Position);

            if (IsAggressive(currentDistance) == false)
                return;

            base.Follow();
        }

        /// <summary>
        /// Checks if this Enemy is Aggro'd to the Player.
        /// </summary>
        /// <param name="currentDistance"></param>
        /// <returns></returns>
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

        #region Methods - Random Movement

        /// <summary>
        /// Determine Random Enemy movement.
        /// Overrides NPC RandomMovement method.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void RandomMovement(GameTime gameTime)
        {
            // stop random movement if enemy is in aggro
            if (IsAggro)
                return;

            base.RandomMovement(gameTime);
        }

        #endregion Methods - Random Movement

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
