using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jusgabon.Source.Models
{
    class Hud
    {
        // Health Textures
        Texture2D healthTexture;
        Rectangle healthRectangle;

        // Mana Textures
        Texture2D manaTexture;
        Rectangle manaRectangle;

        // Stamina Textures
        Texture2D staminaTexture;
        Rectangle staminaRectangle;

        // Bars Background
        Texture2D barsBackgroundTexture;
        Rectangle barsBackgroundRectangle;

        // Equipped Weapon
        Texture2D weaponTexture;
        Rectangle weaponRectangle;
        Texture2D weaponBackgroundTexture;
        Rectangle weaponBackgroundRectangle;

        // Player Gold
        SpriteFont font;
        Texture2D goldBackgroundTexture;
        Rectangle goldBackgroundRectangle;

        Player player;

        // Player Positions
        int playerXPos;
        int playerYPos;

        public Hud()
        {
            // Load image files
            healthTexture = Globals.content.Load<Texture2D>("HUD/Health");
            manaTexture = Globals.content.Load<Texture2D>("HUD/Mana");
            staminaTexture = Globals.content.Load<Texture2D>("HUD/Stamina");
            barsBackgroundTexture = Globals.content.Load<Texture2D>("HUD/Black");
            weaponBackgroundTexture = Globals.content.Load<Texture2D>("HUD/WeaponBackground");
            goldBackgroundTexture = Globals.content.Load<Texture2D>("HUD/GoldBackground");

            // Load font files
            font = Globals.content.Load<SpriteFont>("Gold");

        }

        public void Update(Player player)
        {
            // Update the player positions
            playerXPos = (int)player.Position.X;
            playerYPos = (int) player.Position.Y;
            this.player = player;

            // Update the position of the health bar
            healthRectangle = new Rectangle((playerXPos - 148), (playerYPos - 80), (int)(player.currentHealth / 1.5), 6);

            // Update the position of the mana bar
            manaRectangle = new Rectangle((playerXPos - 148), (playerYPos - 73), (int)(player.currentMana / 1.5), 6);

            // Update the position of the stamina bar
            staminaRectangle = new Rectangle((playerXPos - 148), (playerYPos - 66), (int)(player.currentStamina / 1.5), 6);

            //Update the background for the bars
            barsBackgroundRectangle = new Rectangle((playerXPos - 149), (playerYPos - 81), ((int)(player.Health / 1.5) + 2), 22);
            
            // Update the weapon texture
            weaponTexture = Globals.content.Load<Texture2D>("Items/Weapons/" + player.EquippedWeapon.Name + "/Sprite");
            weaponRectangle = new Rectangle(playerXPos - 142, playerYPos + 73, 13, 19);
            weaponBackgroundRectangle = new Rectangle(playerXPos - 148, playerYPos + 70, 25, 25);

            // Update the gold
            goldBackgroundRectangle = new Rectangle((playerXPos + 133), (playerYPos + 80), 26, 12);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background for the bars
            spriteBatch.Draw(barsBackgroundTexture, barsBackgroundRectangle, Color.White);

            // Draw the health bar
            spriteBatch.Draw(healthTexture, healthRectangle, Color.White);

            // Draw the mana bar
            spriteBatch.Draw(manaTexture, manaRectangle, Color.White);

            // Draw the stamina bar
            spriteBatch.Draw(staminaTexture, staminaRectangle, Color.White);

            // Draw the weapon box
            spriteBatch.Draw(weaponBackgroundTexture, weaponBackgroundRectangle, Color.White);
            spriteBatch.Draw(weaponTexture, weaponRectangle, Color.White);

            // Draw the gold display
            spriteBatch.Draw(goldBackgroundTexture, goldBackgroundRectangle, Color.White);
            spriteBatch.DrawString(font, player.Gold.ToString(), new Vector2(playerXPos + 135, playerYPos + 82), Color.Yellow);
            

        }
    }
}
