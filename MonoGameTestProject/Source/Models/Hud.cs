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

        // Player Positions
        int playerXPos;
        int playerYPos;

        public Hud()
        {
            healthTexture = Globals.content.Load<Texture2D>("HUD/Health");
        }

        public void Update(Sprite player)
        {
            // Update the player positions
            playerXPos = (int)player.Position.X;
            playerYPos = (int) player.Position.Y;

            // Update the position of the health bar
            healthRectangle = new Rectangle((playerXPos - 148), (playerYPos - 80), (int)(player._currentHealth / 1.5), 6);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the health bar
            spriteBatch.Draw(healthTexture, healthRectangle, Color.White);
        }
    }
}

// Hud import
//Hud hud;

// Instantiate Hud
//hud = new Hud();

// Update Hud
//hud.Update(_player);

// Draw Hud
//hud.Draw(Globals.spriteBatch);