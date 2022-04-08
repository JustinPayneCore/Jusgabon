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
    /// Enemy Class - Inherits NPC Class.
    /// Inheritance Order: Enemy -> NPC -> Sprite -> Component
    /// </summary>
    public class Enemy : Npc
    {
        #region Members

        // timer to check if enemy should respawn
        private float _respawnTimer;

        // Distance for Sprite to get aggro and follow target
        public float AggroDistance { get; set; } = 75f;

        // Modifier to increase/decrease aggro once following target
        public float AggroModifier = 1.5f;

        // If sprite is currently aggro'd to target
        public bool IsAggro { get; set; }

        // check if enemy is dead
        public bool IsDead { get; set; } = false;

        // Gold to give on kill
        public int GoldGiven { get; set; } = 10;

        // time it takes before Enemy respawns
        public float RespawnTime { get; set; } = 15f;

        #endregion Members


        #region Methods

        /// <summary>
        /// Enemy Constructor.
        /// Overrides base Npc constructor with an Animation Dictionary, optional SpawnPosition Vector2, and optional Attributes.
        /// </summary>
        /// <param name="animations"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="baseAttributes"></param>
        public Enemy(Dictionary<string, Animation> animations, Attributes baseAttributes) : base(animations, baseAttributes) { }

        #region Methods - Follow Sprite & Aggro Logic

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
        /// IsAggressive method - Checks if the Enemy is Aggro'd to the FollowTarget.
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

        #endregion Methods - Follow Sprite & Aggro Logic

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

        /// <summary>
        /// OnCollide method.
        /// Invoke this method when Enemy Sprite collides with target sprite.
        /// </summary>
        /// <param name="sprite"></param>
        public override void OnCollide(Sprite sprite)
        {
            if (sprite is Weapon && ((Weapon)sprite).Parent is Player)
            {
                TakeHit(((Weapon)sprite).Parent.Attack);
            }

            if (sprite is Spell && ((Spell)sprite).Parent is Player)
            {
                TakeHit(((Spell)sprite).Magic);

                if (sprite is Projectile)
                    ((Projectile)sprite).IsRemoved = true;
                //((Spell)sprite).IsRemoved = true;
            }

        }

        /// <summary>
        /// SetTakeHit method (Enemy) - update take hit method.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void SetTakeHit(GameTime gameTime)
        {
            HitVelocity = -Velocity / 2;

            base.SetTakeHit(gameTime);
        }

        /// <summary>
        /// Kill method (Enemy) - Temporarily kills enemy without removing from list of sprites; enemy will respawn later.
        /// </summary>
        protected override void Kill()
        {
            Console.WriteLine(this.GetType().Name + " killed.");
            Globals.player.GetGold(GoldGiven);

            // remove enemy
            IsDead = true;

            // reset enemy members for next respawn
            Reset();
        }

        /// <summary>
        /// Reset enemy properties for next enemy respawn.
        /// </summary>
        protected virtual void Reset()
        {
            Position = SpawnPosition;
            currentHealth = Health;
            if (IsAggro == true)
            {
                IsAggro = false;
                AggroDistance = (float)(AggroDistance / AggroModifier);
            }
        }

        /// <summary>
        /// Enemy Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sprites"></param>
        public override void Update(GameTime gameTime, List<Sprite> sprites)
        {
            // update respawn timer if enemy is currently dead
            if (IsDead == true)
            {
                _respawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_respawnTimer >= RespawnTime)
                {
                    _respawnTimer = 0f;
                    // respawn enemy
                    IsDead = false;
                }
            }

            // stop updating if enemy is still dead
            if (IsDead == true)
                return;


            base.Update(gameTime, sprites);
        }

        /// <summary>
        /// Enemy Draw method.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsDead == true)
                return;
            
            base.Draw(gameTime, spriteBatch);
        }

        #endregion Methods

    }
}
