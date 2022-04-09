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
    /// PauseState class.
    /// The State that is used when the GameState is paused.
    /// </summary>
    public class PauseState : State
    {
        // list of pause menu components
        private List<Component> _components;

        // the paused game state
        private GameState _gameState;

        /// <summary>
        /// PauseState constructor - pauses the current game state to load the pause menu.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        /// <param name="gameState"></param>
        public PauseState(Game1 game, GraphicsDevice graphics, GameState gameState) : base(game, graphics)
        {
            // get the paused game state
            _gameState = gameState;

            // load the textures
            var buttonTexture = _content.Load<Texture2D>("HUD/Dialog/DialogueBoxSimple");
            var buttonFont = _content.Load<SpriteFont>("HUD/Font/Menu");

            // create buttons and assign them their click events
            var resumeGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 200),
                Text = "Resume Game",
            };
            resumeGameButton.Click += ResumeGameButton_Click;

            var restartGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 300),
                Text = "Restart (Level " + _gameState.Level + ")",
            };
            restartGameButton.Click += RestartGameButton_Click;

            var mainMenuButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 400),
                Text = "Go to Main Menu",
            };
            mainMenuButton.Click += MainMenuButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(640 - buttonTexture.Width / 2, 500),
                Text = "Quit Game",
            };
            quitGameButton.Click += QuitGameButton_Click;

            // create list of menu components (pause menu buttons)
            _components = new List<Component>()
            {
                resumeGameButton,
                restartGameButton,
                mainMenuButton,
                quitGameButton,
            };

        }

        /// <summary>
        /// ResumeGameButton_Click method - resumes the current game state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResumeGameButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("\nResuming Game...\n");
            _game.ChangeState(_gameState);
        }

        /// <summary>
        /// RestartGameButton_Click method - restarts the game state at the current level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RestartGameButton_Click(object sender, EventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\nRestarting Game...\n");
            _game.ChangeState(new GameState(_game, _graphics, _gameState.Level));
        }

        /// <summary>
        /// MainMenuButton_Click method - creates the main menu state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuButton_Click(object sender, EventArgs e)
        {
            Console.Clear();
            Console.WriteLine("\nReturning to Main Menu...\n");
            _game.ChangeState(new MenuState(_game, _graphics));
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
        /// Update Method for Pause State - Update the pause state.
        /// Updating pause state includes checking for button input.
        /// This method is called multiple times per second.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime, Globals.sprites);
        }

        /// <summary>
        /// Post-Update method for Pause State - Called at the end of Update as a post-check to:
        /// - Empty
        /// </summary>
        /// <param name="gameTime"></param>
        public override void PostUpdate(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draw Method for Pause State - Draw the pause menu entities to the screen.
        /// - draws current game state as the background
        /// - draws pause menu components (menu buttons)
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // draw current game state as the background
            _gameState.Draw(gameTime);

            
            _spriteBatch.Begin();

            // draw pause menu components
            foreach (var component in _components)
                component.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }


    }
}
