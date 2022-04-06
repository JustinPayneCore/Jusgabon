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
        // note: _player is also instantiated to be Globals.player for global access
        private Player _player;

        // Game window height
        public static int screenHeight;

        // Game window width
        public static int screenWidth;

        private TmxMap map;

        private Texture2D tileset;

        private int tileWidth;

        private int tileHeight;

        private int tilesetTilesWide;

        private int tilesetTilesHigh;

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
            
            map = new TmxMap("Content/Level 3.tmx");
            tileset = Globals.content.Load<Texture2D>("Backgrounds/Tilesets/" + map.Tilesets[0].Name.ToString());
            /*tileset = Content.Load<Texture2D>(map.Tilesets[0].Name.ToString());*/
            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;
            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;
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
                //new Sprite(Globals.content.Load<Texture2D>("Test_Background")),
                new Sprite(Globals.content.Load<Texture2D>("Level 3")),
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
        /// - Check collision between all current sprites
        /// - Add new children sprites to the list of _sprites and clear
        /// - Remove all "IsRemoved" sprites
        /// </summary>
        /// <param name="gameTime"></param>
        protected void PostUpdate(GameTime gameTime)
        {
            // 1. Check Collision between all current "Sprites"
            foreach (var spriteA in _spritesCollidable)
            {
                foreach (var spriteB in _spritesCollidable)
                {
                    if (spriteA == spriteB)
                        continue;

                    if (spriteA.IsTouching(spriteB))
                    {
                        spriteA.OnCollide(spriteB);
                    }
                }
            }

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
            Globals.spriteBatch.Begin(transformMatrix: _camera.Transform);
            //Globals.spriteBatch.Begin();
            for (int i = 0; i < map.Layers[0].Tiles.Count; i++)
            {
                int gid = map.Layers[0].Tiles[i].Gid;
                if (gid == 0)
                {

                }
                else
                {
                    int tileFrame = gid - 1;
                    int column = tileFrame % tilesetTilesWide;
                    int row = (int)Math.Floor((double)tileFrame / (double)tilesetTilesWide);

                    float x = (i % map.Width) * map.TileWidth;
                    float y = (float)Math.Floor(i / (double)map.Width) * map.TileHeight;

                    Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                    Globals.spriteBatch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                }
            }
            
            // TODO: Add your drawing code
            //Globals.spriteBatch.Begin(transformMatrix: _camera.Transform);

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
