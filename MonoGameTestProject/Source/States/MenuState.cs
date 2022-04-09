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
    /// Main MenuState class.
    /// The State that is used to display the main menu.
    /// </summary>
    public class MenuState : State
    {
        // list of main menu components
        private List<Component> _components;

        // background image texture
        Texture2D backgroundTexture;

        // main title texture (JUSGABON)
        Texture2D titleTexture;

        /// <summary>
        /// MenuState constructor - creates the main menu state.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        public MenuState(Game1 game, GraphicsDevice graphics) : base(game, graphics)
        {
            // load the textures
            backgroundTexture = _content.Load<Texture2D>("background");
            titleTexture = _content.Load<Texture2D>("title");
            var buttonTexture = _content.Load<Texture2D>("HUD/Dialog/DialogueBoxSimple");
            var buttonFont = _content.Load<SpriteFont>("HUD/Font/Menu");

            // create buttons and assign them their click events
            var level1Button = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 250),
                Text = "Start Level 1",
            };
            level1Button.Click += Level1Button_Click;

            var level2Button = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 350),
                Text = "Start Level 2",
            };
            level2Button.Click += Level2Button_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 450),
                Text = "Quit Game",
            };
            quitGameButton.Click += QuitGameButton_Click;

            // create list of menu components (main menu buttons)
            _components = new List<Component>()
            {
                level1Button,
                level2Button,
                quitGameButton,
            };
            
        }

        /// <summary>
        /// Level1Button_Click method - creates the level 1 game state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Level1Button_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nStarting Level 1...\n");
            _game.ChangeState(new GameState(_game, _graphics, 1));
        }

        /// <summary>
        /// Level2Button_Click method - creates the level 2 game state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Level2Button_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nStarting Level 2...\n");
            _game.ChangeState(new GameState(_game, _graphics, 2));
        }

        /// <summary>
        /// QuitGameButton_Click method - exits the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }

        /// <summary>
        /// Update Method for Menu State - Called on a regular interval to update the menu state.
        /// Updating menu state includes checking for button input.
        /// This method is called multiple times per second.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime, Globals.sprites);
        }

        /// <summary>
        /// Post-Update method for Menu State - Called at the end of Update as a post-check to:
        /// - Empty
        /// </summary>
        /// <param name="gameTime"></param>
        public override void PostUpdate(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draw Method for Menu State - Draw the main menu entities to the screen.
        /// - draws background image and game title
        /// - draws main menu components (menu buttons)
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            // draw background image and game title
            _spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(titleTexture, new Vector2(640 - titleTexture.Width / 2, 100), Color.White);

            // draw main menu components
            foreach (var component in _components)
                component.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }
    }
}
