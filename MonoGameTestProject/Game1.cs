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
    /// Game Class - main program of the MonoGame project.
    /// Needs to include these methods necessary for the game to run:
    ///  - Constructor & Class members
    ///  - Initialize method
    ///  - LoadContent & UnloadContent method
    ///  - Update method
    ///  - Draw method
    /// </summary>
    public class Game1 : Game
    {
        #region Members

        // Graphics manager (required in all MonoGame projects)
        GraphicsDeviceManager graphics;

        // Game screen camera object
        private Camera _camera;

        // List of sprites that have collision detection
        private List<Sprite> _spritesCollidable;

        // List of sprites that shouldn't be checked for collision (ex. background)
        private List<Sprite> _spritesNonCollidable;

        // Player object
        // note: _player is also instantiated to be Globals.player for global access
        private Player _player;

        // Game window height
        public static int screenHeight;

        // Game window width
        public static int screenWidth;

        #endregion Members


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
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 48) },
            };

            // Load NPC Cat animations
            var npcCatAnimations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Cat/SpriteSheet"), 2) },
            };

            // Base NPC Attributes
            var baseNpcAttributes = new Attributes()
            {
                Speed = 0.6f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 0,
                Magic = 0,
            };

            // Load Enemy Monster animations
            var enemyOctopusAnimations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 48) },
            };

            // Base Enemy Attributes
            var baseEnemyAttributes = new Attributes()
            {
                Speed = 0.6f,
                Health = 100,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };

            // Load Boss Demon Cyclops animations
            var bossDemonCyclopAnimations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Walk"), 6) },
            };

            // Base Boss Attributes
            var baseBossAttributes = new Attributes()
            {
                Speed = 0.5f,
                Health = 400,
                Mana = 0,
                Stamina = 0,
                Attack = 25,
                Magic = 0,
            };


            // Instantiate list of sprites which wil be updated/drawn
            _spritesNonCollidable = new List<Sprite>()
            {
                new Sprite(Globals.content.Load<Texture2D>("Test_Background")),
            };

            _spritesCollidable = new List<Sprite>()
            {
                new Npc(npcVillagerAnimations) { Position = new Vector2(-100, -50), BaseAttributes = baseNpcAttributes },
                new Npc(npcCatAnimations) { Position = new Vector2(50, -75), IsStationary = true, BaseAttributes = baseNpcAttributes },
                new Enemy(enemyOctopusAnimations) { Position = new Vector2(150, 100), BaseAttributes = baseEnemyAttributes },
                new Enemy(enemyOctopusAnimations) { Position = new Vector2(-100, 200), BaseAttributes = baseEnemyAttributes },
                new Boss(bossDemonCyclopAnimations) { Position = new Vector2(300, 0), BaseAttributes = baseEnemyAttributes },
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
                // walk
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>(path + "Walk"),
                                                frameCount: 4,
                                                spritesheetColumns: 4,
                                                frameLocation: 0) },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 48) },
                
                // idle
                {"IdleDown",  new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 0) },
                {"IdleUp",    new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 16) },
                {"IdleLeft",  new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 32) },
                {"IdleRight", new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 48) },

                // attack
                {"AttackDown",  new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 0) {FrameSpeed = 0.35f } },
                {"AttackUp",    new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 16) {FrameSpeed = 0.35f } },
                {"AttackLeft",  new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 32) {FrameSpeed = 0.35f } },
                {"AttackRight", new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 48) {FrameSpeed = 0.35f } },

                // jump
                {"JumpDown",  new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 0) {FrameSpeed = 0.25f } },
                {"JumpUp",    new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 16) {FrameSpeed = 0.25f } },
                {"JumpLeft",  new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 32) {FrameSpeed = 0.25f } },
                {"JumpRight", new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 48) {FrameSpeed = 0.25f } },

                // other actions
                {"Item",        new Animation(Globals.content.Load<Texture2D>(path + "Item"), 1) {FrameSpeed = 0.75f } },
                {"Special1",    new Animation(Globals.content.Load<Texture2D>(path + "Special1"), 1) {FrameSpeed = 0.75f } },
                {"Special2",    new Animation(Globals.content.Load<Texture2D>(path + "Special2"), 1) {FrameSpeed = 0.75f } },
                {"Dead",        new Animation(Globals.content.Load<Texture2D>(path + "Dead"), 1) },
            };
            
            // Base Attributes
            var playerAttributes = new Attributes()
            {
                Speed = 1.5f,
                Health = 100,
                Mana = 100,
                Stamina = 50,
                Attack = 20,
                Magic = 40,
            };

            // Initialize player
            Globals.player = new Player(
                animations: playerAnimations,
                spawnPosition: new Vector2(0, 0),
                baseAttributes: playerAttributes
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

            foreach (var sprite in _spritesNonCollidable)
                sprite.Update(gameTime, _spritesNonCollidable);
            
            foreach (var sprite in _spritesCollidable)
                sprite.Update(gameTime, _spritesCollidable);

            _camera.Follow(_player);

            PostUpdate(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Post-Update method - Called at the end of Update as a post-check to:
        /// - Add new children sprites to the list of _sprites and clear
        /// - Remove all "IsRemoved" sprites
        /// - Sort sprites by its current Y position to create a 2.5D illusion effect
        /// </summary>
        /// <param name="gameTime"></param>
        protected void PostUpdate(GameTime gameTime)
        {

            // 2. Add Children to the list of "_sprites" and clear
            int count = _spritesCollidable.Count;
            for (int i = 0; i < count; i++)
            {
                foreach (var child in _spritesCollidable[i].Children)
                    _spritesCollidable.Add(child);

                _spritesCollidable[i].Children.Clear();
            }

            // 3. Remove all "IsRemoved" sprites
            for (int i = 0; i < _spritesCollidable.Count; i++)
            {
                if (_spritesCollidable[i].IsRemoved)
                {
                    _spritesCollidable.RemoveAt(i);
                    i--;
                }
            }

            // 4. Sort sprites by its current Y Position to create a 2.5D illusion effect
            _spritesCollidable.Sort((spriteA, spriteB) => spriteA.Position.Y.CompareTo(spriteB.Position.Y));
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

        #endregion Methods
    }
}
