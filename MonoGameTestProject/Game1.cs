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
#endregion

/// <summary>
/// Top down 2D Zelda-like RPG game built using the MonoGame framework
/// Authors: Team 4 - Jason Cheung, Gabriel Baluyut, Justin Payne
/// Date: Mar 14, 2022
/// Source: https://docs.monogame.net/articles/getting_started/0_getting_started.html
/// </summary>
namespace Jusgabon
{
    /// <summary>
    /// Game Class definition - main class program of the MonoGame project.
    /// This class needs to include these methods necessary for the game to run:
    ///  - Constructor & key variables
    ///  - Initialize method
    ///  - LoadContent & UnloadContent method
    ///  - Update method
    ///  - Draw method
    /// </summary>
    public class Game1 : Game
    {
        #region Fields

        // Graphics manager (required in all MonoGame projects)
        GraphicsDeviceManager graphics;

        // Game screen camera object
        private Camera _camera;

        // List of sprites that have collision detection
        private List<Sprite> _spritesCollidable;

        // List of sprites that shouldn't be checked for collision (ex. background)
        private List<Sprite> _spritesNonCollidable;

        // Player object
        // note: the player is instantiated to also be Globals.player so other classes can get global access to it
        private Player _player;

        // Game window height
        public static int screenHeight;

        // Game window width
        public static int screenWidth;

        #endregion


        #region Methods

        /// <summary>
        /// Game Constructor - Includes bits to tell the project how to start.
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initialize method - Initialize the game upon startup.
        /// This method is called after the constructor but before the main game loop.
        /// Method is used to query any required services and load any non-graphic related content.
        /// </summary>
        protected override void Initialize()
        {
            // Set Game Window size
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            // TODO: Add your initialization logic
            screenHeight = graphics.PreferredBackBufferHeight;
            screenWidth = graphics.PreferredBackBufferWidth;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent Method - Add assets from the running game from the Content project.
        /// This method is called only once per game within the Initialize method.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new content and spriteBatch, which is used to load and draw textures.
            Globals.content = this.Content;
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use Globals.Content to load your game content

            // Instantiate camera
            _camera = new Camera();

            LoadPlayer();

            // NPC Villager animations
            var npcVillagerAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Idle"), 1, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Idle"), 1, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Idle"), 1, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Idle"), 1, 4, 48) },
            };

            // Load NPC Cat animations
            var npcCatAnimations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Cat/SpriteSheet"), 2, false) },
            };

            // Load Boss Demon Cyclops animations
            var bossDemonCyclopAnimations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Walk"), 6, false) },
            };

            // Instantiate list of sprites which wil be updated/drawn
            _spritesNonCollidable = new List<Sprite>()
            {
                new Sprite(Globals.content.Load<Texture2D>("Test_Background")),
            };
            
            _spritesCollidable = new List<Sprite>()
            {
                new Sprite(npcVillagerAnimations) { Position = new Vector2(100, 200) },
                new Sprite(npcCatAnimations) { Position = new Vector2(200, 100) },
                new Sprite(bossDemonCyclopAnimations) { Position = new Vector2(200, 200) },
                _player,
            };

        }

        /// <summary>
        /// Load Player-specific Content.
        /// Includes player animations and ...
        /// </summary>
        private void LoadPlayer()
        {
            // Content path for Player
            var path = "Actor/Characters/BlueNinja/SeparateAnim/";

            // Player animations
            var playerAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 48) },
            };

            // TESTING - boss animations
            var bossAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Boss/GiantFrog/Attack"), 3, false)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Boss/GiantFrog/Charge"), 7, false)  },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Boss/GiantFrog/Hit"), 3, false)  },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Boss/GiantFrog/Jump"), 6, false)  },
            };

            // Set up player
            Globals.player = new Player(
                animations: playerAnimations,
                //animations: bossAnimations, // TESTING
                spawnPosition: new Vector2(100, 100)
                );
            _player = Globals.player;
        }

        /// <summary>
        /// UnloadContent method - Unload game-specific content from the Content project.
        /// This method is only called one per game.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non-ContentManager content




            base.UnloadContent();
        }

        /// <summary>
        /// Update Method - Called on a regular interval to update the game state.
        /// Updating game state includes taking player input, checking for collision, playing audio, animating entities, etc...
        /// This method is called multiple times per second.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic


            foreach (var sprite in _spritesCollidable)
                sprite.Update(gameTime, _spritesCollidable);


            _camera.Follow(_player);



            base.Update(gameTime);
        }

        /// <summary>
        /// Draw Method - Called on a regular interval to draw the game entities to the screen.
        /// This method is called multiple times per second.
        /// (!) Warning (!): Do not include update logic in Draw method; Use the Update method instead.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code
            Globals.spriteBatch.Begin(transformMatrix: _camera.Transform);

            foreach (var sprite in _spritesNonCollidable)
                sprite.Draw(gameTime, Globals.spriteBatch);
            
            foreach (var sprite in _spritesCollidable)
                sprite.Draw(gameTime, Globals.spriteBatch);

            Globals.spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion 
    }
}
