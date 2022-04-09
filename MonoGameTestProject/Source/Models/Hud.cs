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
    /// Heads-up display (Hud) class for the game player interface.
    /// </summary>
    public class Hud
    {
        #region Members

        // Health Textures
        Texture2D healthTexture;
        Rectangle healthRectangle;
        Rectangle healthBackgroundRectangle;
        SpriteFont healthFont;

        // Mana Textures
        Texture2D manaTexture;
        Rectangle manaRectangle;
        Rectangle manaBackgroundRectangle;

        // Stamina Textures
        Texture2D staminaTexture;
        Rectangle staminaRectangle;
        Rectangle staminaBackgroundRectangle;

        // Bars Background
        Texture2D barsBackgroundTexture;

        // Equipped Weapon
        Texture2D weaponTexture;
        Rectangle weaponRectangle;
        Texture2D weaponBackgroundTexture;
        Rectangle weaponBackgroundRectangle;

        // Player Gold
        SpriteFont goldFont;
        Texture2D goldBackgroundTexture;
        Rectangle goldBackgroundRectangle;

        // Boss Health Bar
        Texture2D bossHealthTexture;
        Rectangle bossHealthRectangle;
        Rectangle bossHealthBackgroundRectangle;
        SpriteFont bossHealthFont;

        // Player instance and attributes
        Player player { get => Globals.player; }
        int playerXPos;
        int playerYPos;

        // The aggro'd Boss instance for the boss health bar
        Boss boss { get; set; }


        #endregion Members

        #region Methods

        /// <summary>
        /// Heads-up display constructor - intialize content files and get boss instance.
        /// </summary>
        public Hud()
        {
            // Load image files
            healthTexture = Globals.content.Load<Texture2D>("HUD/Health");
            manaTexture = Globals.content.Load<Texture2D>("HUD/Mana");
            staminaTexture = Globals.content.Load<Texture2D>("HUD/Stamina");
            barsBackgroundTexture = Globals.content.Load<Texture2D>("HUD/Black");
            weaponBackgroundTexture = Globals.content.Load<Texture2D>("HUD/WeaponBackground");
            goldBackgroundTexture = Globals.content.Load<Texture2D>("HUD/GoldBackground");
            bossHealthTexture = Globals.content.Load<Texture2D>("HUD/Health");

            // Load font files
            goldFont = Globals.content.Load<SpriteFont>("HUD/Font/Gold");
            healthFont = Globals.content.Load<SpriteFont>("HUD/Font/PlayerHealth");
            bossHealthFont = Globals.content.Load<SpriteFont>("HUD/Font/BossHealth");

            // Locate and load boss instance
            foreach (var sprite in Globals.sprites)
            {
                if (sprite is Boss)
                {
                    boss = (Boss)sprite;
                    break;
                }
            }

        }

        /// <summary>
        /// Update method for Hud.
        /// </summary>
        public void Update()
        {
            // Update the player positions
            playerXPos = (int)player.Position.X;
            playerYPos = (int)player.Position.Y;

            // Update the position of the health bar
            healthRectangle = new Rectangle((playerXPos - 148), (playerYPos - 80), (int)(player.currentHealth / 1), 6);
            healthBackgroundRectangle = new Rectangle((playerXPos - 149), (playerYPos - 81), ((int)(player.Health / 1) + 2), 8);

            // Update the position of the mana bar
            manaRectangle = new Rectangle((playerXPos - 148), (playerYPos - 73), (int)(player.currentMana / 1.5), 3);
            manaBackgroundRectangle = new Rectangle((playerXPos - 149), (playerYPos - 74), ((int)(player.Mana / 1.5) + 2), 5);

            // Update the position of the stamina bar
            staminaRectangle = new Rectangle((playerXPos - 148), (playerYPos - 69), (int)(player.currentStamina / 1.5), 3);
            staminaBackgroundRectangle = new Rectangle((playerXPos - 149), (playerYPos - 70), ((int)(player.Stamina / 1.5) + 2), 5);

            // Update the weapon texture
            weaponTexture = Globals.content.Load<Texture2D>("Items/Weapons/" + player.EquippedWeapon.Name + "/Sprite");
            weaponRectangle = new Rectangle(playerXPos - 142, playerYPos + 73, 13, 19);
            weaponBackgroundRectangle = new Rectangle(playerXPos - 148, playerYPos + 70, 25, 25);

            // Update the gold
            goldBackgroundRectangle = new Rectangle((playerXPos + 133), (playerYPos + 80), 26, 12);

            // Update boss health bar
            bossHealthRectangle = new Rectangle((playerXPos - 90), (playerYPos + 75), ((int)boss.currentHealth / 2), 10);
            bossHealthBackgroundRectangle = new Rectangle((playerXPos - 91), (playerYPos + 74), ((int)(boss.Health / 2) + 2), 12);
        }

        /// <summary>
        /// Draw method for Hud.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {

            // Draw the health bar
            spriteBatch.Draw(barsBackgroundTexture, healthBackgroundRectangle, Color.White);
            spriteBatch.Draw(healthTexture, healthRectangle, Color.White);
            spriteBatch.DrawString(healthFont, player.currentHealth + "/" + player.Health, new Vector2((playerXPos - 111), (playerYPos - 81)), Color.White);

            // Draw the mana bar
            spriteBatch.Draw(barsBackgroundTexture, manaBackgroundRectangle, Color.White);
            spriteBatch.Draw(manaTexture, manaRectangle, Color.White);

            // Draw the stamina bar
            spriteBatch.Draw(barsBackgroundTexture, staminaBackgroundRectangle, Color.White);
            spriteBatch.Draw(staminaTexture, staminaRectangle, Color.White);

            // Draw the weapon box
            spriteBatch.Draw(weaponBackgroundTexture, weaponBackgroundRectangle, Color.White);
            spriteBatch.Draw(weaponTexture, weaponRectangle, Color.White);

            // Draw the gold display
            spriteBatch.Draw(goldBackgroundTexture, goldBackgroundRectangle, Color.White);
            spriteBatch.DrawString(goldFont, player.Gold.ToString(), new Vector2(playerXPos + 135, playerYPos + 82), Color.Yellow);

            // Draw the boss health bar if boss is aggroed to player
            if (boss.IsAggro == true)
            {
                spriteBatch.Draw(barsBackgroundTexture, bossHealthBackgroundRectangle, Color.White);
                spriteBatch.Draw(bossHealthTexture, bossHealthRectangle, Color.White);
                spriteBatch.DrawString(bossHealthFont, boss.currentHealth + "/" + boss.Health, new Vector2((playerXPos - 8), (playerYPos + 75)), Color.White);
            }

        }


        #endregion Methods
    }
}
