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
        /// </summary>
        /// <param name="animations"></param>
        public Boss(Dictionary<string, Animation> animations, Attributes baseAttributes) : base(animations, baseAttributes)
        {
            AggroModifier = 2f; // default enemy modifier = 1.5f; increase if boss should be "stickier" to player

            // change default TakeHit animation length & cooldowns
            _hitCooldown = 1f;
            HitSpeed = 0.45f;
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
                {
                    Console.WriteLine(this.GetType().Name + " killed.");
                    IsRemoved = true;
                }
            }
        }

        #endregion

    }
}
