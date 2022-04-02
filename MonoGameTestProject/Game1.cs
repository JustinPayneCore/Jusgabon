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

        // Player object
        // note: _player is also instantiated to be Globals.player for global access
        private Player _player;

        // Game window height
        public static int screenHeight;

        // Game window width
        public static int screenWidth;

        // Tiled map import variables
        private TileMapManager tileMapManager;
        private TmxMap map;
        private Texture2D tileset;

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

            // Creates the tiled map using the TiledMapMananger class
            // Tiled map file path
            map = new TmxMap("Content/Level1.tmx");

            tileset = Globals.content.Load<Texture2D>("Backgrounds/Tilesets/" + map.Tilesets[0].Name.ToString());
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;
            int tilesetTilesWide = tileset.Width / tileWidth;
            tileMapManager = new TileMapManager(map, tileset, tilesetTilesWide, tileWidth, tileHeight);

            // TODO: use Globals.Content to load your game content

            // Instantiate camera
            _camera = new Camera();

            // Instantiate Player
            LoadPlayer();

            // Load NPC Villager animations
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

            // Load basic NPC Attributes
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

            // Load basic Enemy Attributes
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

            // Load basic Boss Attributes
            var baseBossAttributes = new Attributes()
            {
                Speed = 0.5f,
                Health = 400,
                Mana = 0,
                Stamina = 0,
                Attack = 25,
                Magic = 0,
            };

            // Load weapon Lance animations & attributes
            var weaponLanceAnimations = new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Lance/Sprite"), 1) },
                {"SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Lance/SpriteInHand"), 1) }
            };
            var weaponLanceAttributes = new Attributes()
            {
                Speed = 0f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };

            // Load weapon Big Sword animations & attributes
            var weaponBigSwordAnimations = new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/BigSword/Sprite"), 1) },
                {"SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/BigSword/SpriteInHand"), 1) }
            };
            var weaponBigSwordAttributes = new Attributes()
            {
                Speed = -0.5f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 20,
                Magic = 0,
            };

            // Load weapon Sai animations & attributes
            var weaponSaiAnimations = new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Sai/Sprite"), 1) },
                {"SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Sai/SpriteInHand"), 1) }
            };
            var weaponSaiAttributes = new Attributes()
            {
                Speed = 0.25f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };


            // Instantiate list of sprites which will be updated/drawn
            _spritesCollidable = new List<Sprite>()
            {
                new Npc(npcVillagerAnimations) { Position = new Vector2(200, 215), BaseAttributes = baseNpcAttributes },
                new Npc(npcCatAnimations) { Position = new Vector2(350, 215), IsStationary = true, BaseAttributes = baseNpcAttributes },
                new Enemy(enemyOctopusAnimations) { Position = new Vector2(700, 550), BaseAttributes = baseEnemyAttributes },
                new Enemy(enemyOctopusAnimations) { Position = new Vector2(200, 500), BaseAttributes = baseEnemyAttributes },
                new Boss(bossDemonCyclopAnimations) { Position = new Vector2(1180, 1100), BaseAttributes = baseEnemyAttributes },
                _player,
            };

            // Add weapons to player weapon inventory
            _player.PickUp(new Weapon(weaponLanceAnimations, weaponLanceAttributes));
            _player.PickUp(new Weapon(weaponBigSwordAnimations, weaponBigSwordAttributes));
            _player.PickUp(new Weapon(weaponSaiAnimations, weaponSaiAttributes));
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
                {"AttackDown",  new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 0) },
                {"AttackUp",    new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 16) },
                {"AttackLeft",  new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 32) },
                {"AttackRight", new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 48) },

                // jump
                {"JumpDown",  new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 0) },
                {"JumpUp",    new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 16) },
                {"JumpLeft",  new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 32) },
                {"JumpRight", new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 48) },

                // other actions
                {"Item",        new Animation(Globals.content.Load<Texture2D>(path + "Item"), 1) },
                {"Special1",    new Animation(Globals.content.Load<Texture2D>(path + "Special1"), 1) },
                {"Special2",    new Animation(Globals.content.Load<Texture2D>(path + "Special2"), 1) },
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
                spawnPosition: new Vector2(300, 300),
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

            // Update all sprites
            foreach (var sprite in _spritesCollidable)
                sprite.Update(gameTime, _spritesCollidable);

            // update camera position to follow player
            _camera.Follow(_player);


            PostUpdate(gameTime);
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

            // Add Children to the list of "_sprites" and clear
            int count = _spritesCollidable.Count;
            for (int i = 0; i < count; i++)
            {
                foreach (var child in _spritesCollidable[i].Children)
                    _spritesCollidable.Add(child);

                _spritesCollidable[i].Children.Clear();
            }

            // Remove all "IsRemoved" sprites
            for (int i = 0; i < _spritesCollidable.Count; i++)
            {
                if (_spritesCollidable[i].IsRemoved)
                {
                    _spritesCollidable.RemoveAt(i);
                    i--;
                }
            }

            // Sort sprites by its current Y Position to create a 2.5D illusion effect
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
           
            // Draw the tiled map
            tileMapManager.Draw(gameTime, Globals.spriteBatch);

            // Draw all the sprites
            foreach (var sprite in _spritesCollidable)
                sprite.Draw(gameTime, Globals.spriteBatch);

            Globals.spriteBatch.End();
        }

        #endregion Methods
    }
}
