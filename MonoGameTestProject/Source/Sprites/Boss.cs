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


        #endregion Members


        #region Methods

        /// <summary>
        /// Boss Constructor.
        /// Overrides base Enemy constructor with an Animation Dictionary, optional SpawnPosition Vector2, and optional Attributes.
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="baseAttributes"></param>
        public Boss(Dictionary<string, Animation> animations, Vector2 spawnPosition, Attributes baseAttributes) : base(animations, spawnPosition, baseAttributes)
        {
            // default enemy modifier = 1.5f; increase if boss should be "stickier" to player
            AggroModifier = 2f;

            // change default TakeHit animation length & cooldowns
            _hitCooldown = 1f;
            HitSpeed = 0.45f;

            // change amount of gold to give on kill
            GoldGiven = 250;
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
            HitVelocity = -Velocity / 2;
            Velocity = HitVelocity;

            // reset boss take hit cooldown
            if (_hitTimer > HitSpeed)
            {
                IsHit = false;

                // check if boss is dead
                if (_currentHealth <= 0)
                    Remove();
            }
        }

        #endregion

    }
}
