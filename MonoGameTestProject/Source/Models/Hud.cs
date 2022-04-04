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

        // Equipped Weapon
        Texture2D weaponTexture;
        Rectangle weaponRectangle;
        Texture2D weaponBackgroundTexture;
        Rectangle weaponBackgroundRectangle;

        // Player Positions
        int playerXPos;
        int playerYPos;

        public Hud()
        {
            // Load image files
            healthTexture = Globals.content.Load<Texture2D>("HUD/Health");
            manaTexture = Globals.content.Load<Texture2D>("HUD/Mana");
            staminaTexture = Globals.content.Load<Texture2D>("HUD/Stamina");

        }

        public void Update(Player player)
        {
            // Update the player positions
            playerXPos = (int)player.Position.X;
            playerYPos = (int) player.Position.Y;

            // Update the position of the health bar
            healthRectangle = new Rectangle((playerXPos - 148), (playerYPos - 80), (int)(player.currentHealth / 1.5), 6);

            // Update the position of the mana bar
            manaRectangle = new Rectangle((playerXPos - 148), (playerYPos - 73), (int)(player.Mana / 1.5), 6);

            // Update the position of the stamina bar
            staminaRectangle = new Rectangle((playerXPos - 148), (playerYPos - 66), (int)(player.Stamina / 1.5), 6);

            // Update the weapon texture
            weaponTexture = Globals.content.Load<Texture2D>("Items/Weapons/" + player.EquippedWeapon.Name + "/Sprite");
            weaponRectangle = new Rectangle(playerXPos - 143, playerYPos + 72, 15, 21);
            weaponBackgroundTexture = Globals.content.Load<Texture2D>("HUD/WeaponBackground");
            weaponBackgroundRectangle = new Rectangle(playerXPos - 148, playerYPos + 70, 25, 25);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the health bar
            spriteBatch.Draw(healthTexture, healthRectangle, Color.White);

            // Draw the mana bar
            spriteBatch.Draw(manaTexture, manaRectangle, Color.White);

            // Draw the stamina bar
            spriteBatch.Draw(staminaTexture, staminaRectangle, Color.White);

            // Draw the weapon box
            spriteBatch.Draw(weaponBackgroundTexture, weaponBackgroundRectangle, Color.White);
            spriteBatch.Draw(weaponTexture, weaponRectangle, Color.White);
        }
    }
}
