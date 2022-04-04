﻿#region Includes
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
using Jusgabon.Source.Models;
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

        private List<(string key, Vector2 spawnPosition)> _spawnPositions;

        public struct SpriteProperties
        {
            public Dictionary<string, Animation> animations;
            public Attributes baseAttributes;
            public override int GetHashCode()
            {
                var hashCode = 43270662;
                hashCode = hashCode * -1521134295 + animations.GetHashCode();
                hashCode = hashCode * -1521134295 + baseAttributes.GetHashCode();
                return hashCode;
            }
            public override bool Equals(object obj) 
            {
                return obj is SpriteProperties other && (animations == other.animations && baseAttributes == other.baseAttributes);
            }
        }


        // content libaries
        private Dictionary<string, SpriteProperties> _dictNpcs;

        private Dictionary<string, SpriteProperties> _dictAnimals;

        private Dictionary<string, SpriteProperties> _dictEnemies;

        private Dictionary<string, SpriteProperties> _dictWeapons;

        private Dictionary<string, SpriteProperties> _dictBosses;


        // List of sprites that have collision detection
        private List<Sprite> _spritesCollidable;

        // Player object
        // note: _player is also instantiated to be Globals.player for global access
        private Player _player { get => Globals.player; set => Globals.player = value; }

        // Game window height
        public static int screenHeight;

        // Game window width
        public static int screenWidth;

        // Tiled map import variables
        private TileMapManager tileMapManager;
        private TmxMap map;
        private Texture2D tileset;

        // Hud import
        Hud hud;

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

            // Instantiate Hud
            hud = new Hud();

            // Instantiate camera
            _camera = new Camera();

            // Load sprites
            LoadContentSprites();
        }

        protected virtual void LoadContentSprites()
        {
            // Load weapon library
            LoadLibraryWeapons();
            // Instantiate npc dictionary library
            LoadLibraryNpcs();
            // Load npc animals library
            LoadLibraryAnimals();
            // Load enemy library
            LoadLibraryEnemies();
            // Load boss libary
            LoadLibraryBosses();


            // Initialize list of sprites
            _spritesCollidable = new List<Sprite>() { };

            // Get spawn positions from tiled map
            _spawnPositions = tileMapManager.LoadSpawnPositions();

            foreach (var item in _spawnPositions)
            {
                switch (item.key)
                {
                    case "BlueX": // Instantiate and Load Player
                        LoadContentPlayer(item.spawnPosition);
                        _spritesCollidable.Add(_player);
                        break;

                    case "GreenX": // Load Villager
                        _spritesCollidable.Add(new Npc(_dictNpcs["Villager"].animations, item.spawnPosition, _dictNpcs["Villager"].baseAttributes));
                        break;

                    case "YellowX": // Load Dog
                        _spritesCollidable.Add(new Npc(_dictAnimals["Dog"].animations, item.spawnPosition, _dictAnimals["Dog"].baseAttributes) { IsStationary = true });
                        break;

                    case "OrangeX": // Nothing
                        break;

                    case "RedX": // Load Enemy1
                        _spritesCollidable.Add(new Enemy(_dictEnemies["Octopus"].animations, item.spawnPosition, _dictEnemies["Octopus"].baseAttributes));
                        break;

                    case "PurpleX": // Load Enemy2
                        _spritesCollidable.Add(new Enemy(_dictEnemies["Cyclope"].animations, item.spawnPosition, _dictEnemies["Cyclope"].baseAttributes) { GoldGiven = 15 });
                        break;

                    case "BlackX": // Load Old Woman Npc
                        _spritesCollidable.Add(new Npc(_dictNpcs["OldWoman"].animations, item.spawnPosition, _dictNpcs["OldWoman"].baseAttributes));
                        break;

                    case "WhiteX": // Load Boss
                        _spritesCollidable.Add(new Boss(_dictBosses["DemonCyclop"].animations, item.spawnPosition, _dictBosses["DemonCyclop"].baseAttributes));
                        break;

                    default:
                        break;

                }
            }

            // set enemy FollowTarget to player
            foreach (var sprite in _spritesCollidable)
            {
                if (sprite is Enemy)
                    ((Enemy)sprite).SetFollowTarget(_player, 10f); // 10f to be close enough to collide with player hitbox
            }

        }

        protected void LoadLibraryAnimals()
        {
            _dictAnimals = new Dictionary<string, SpriteProperties>();

            var AnimalProperties = new SpriteProperties();

            // Load & set basic animal Attributes
            var baseAnimalAttributes = new Attributes()
            {
                Speed = 0.8f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 0,
                Magic = 0,
            };
            AnimalProperties.baseAttributes = baseAnimalAttributes;

            // Cat
            AnimalProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Cat/SpriteSheet"), 2) },
            };
            _dictAnimals.Add("Cat", AnimalProperties);

            // Dog
            AnimalProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Dog/SpriteSheet"), 2) },
            };
            _dictAnimals.Add("Dog", AnimalProperties);

            // Dog2
            AnimalProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Dog2/SpriteSheet"), 2) },
            };
            _dictAnimals.Add("Dog2", AnimalProperties);

            // Frog
            AnimalProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Frog/SpriteSheet"), 2) },
            };
            _dictAnimals.Add("Frog", AnimalProperties);

            // Racoon
            AnimalProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Animals/Racoon/SpriteSheet"), 2) },
            };
            _dictAnimals.Add("Racoon", AnimalProperties);


        }

        protected void LoadLibraryBosses()
        {
            _dictBosses = new Dictionary<string, SpriteProperties>();

            var BossProperties = new SpriteProperties();
            

            // Load basic Boss Attributes
            var baseAttributes = new Attributes()
            {
                Speed = 0.5f,
                Health = 400,
                Mana = 0,
                Stamina = 0,
                Attack = 25,
                Magic = 0,
            };
            BossProperties.baseAttributes = baseAttributes;

            // Boss Demon Cyclop
            BossProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Walk"), 6) },
                {"Hit", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Hit"), 3) },
            };
            _dictBosses.Add("DemonCyclop", BossProperties);


        }

        protected void LoadLibraryEnemies()
        {

            _dictEnemies = new Dictionary<string, SpriteProperties>();

            var EnemyProperties = new SpriteProperties();

            // Base attributes
            var baseAttributes = new Attributes()
            {
                Speed = 0.6f,
                Health = 100,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };
            EnemyProperties.baseAttributes = baseAttributes;

            // Octopus (green)
            EnemyProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus/Octopus"), 4, 4, 48) },
            };
            EnemyProperties.baseAttributes = new Attributes()
            {
                Speed = 0.6f,
                Health = 100,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };
            _dictEnemies.Add("Octopus", EnemyProperties);


            // Octopus 2 (red)
            EnemyProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus2/Octopus2"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus2/Octopus2"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus2/Octopus2"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Octopus2/Octopus2"), 4, 4, 48) },
            };
            EnemyProperties.baseAttributes = new Attributes()
            {
                Speed = 0.4f,
                Health = 120,
                Mana = 0,
                Stamina = 0,
                Attack = 15,
                Magic = 0,
            };
            _dictEnemies.Add("Octopus2", EnemyProperties);

            // Cyclope (red)
            EnemyProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope/Cyclopes"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope/Cyclopes"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope/Cyclopes"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope/Cyclopes"), 4, 4, 48) },
            };
            EnemyProperties.baseAttributes = new Attributes()
            {
                Speed = 0.4f,
                Health = 120,
                Mana = 0,
                Stamina = 0,
                Attack = 20,
                Magic = 0,
            };
            _dictEnemies.Add("Cyclope", EnemyProperties);

            // Cyclope2 (green)
            EnemyProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope2/SpriteSheet"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope2/SpriteSheet"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope2/SpriteSheet"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Monsters/Cyclope2/SpriteSheet"), 4, 4, 48) },
            };
            EnemyProperties.baseAttributes = new Attributes()
            {
                Speed = 0.6f,
                Health = 100,
                Mana = 0,
                Stamina = 0,
                Attack = 15,
                Magic = 0,
            };
            _dictEnemies.Add("Cyclope2", EnemyProperties);


        }

        protected void LoadLibraryNpcs()
        {
            _dictNpcs = new Dictionary<string, SpriteProperties>();

            var NpcProperties = new SpriteProperties();

            // Load & set basic NPC Attributes
            var baseNpcAttributes = new Attributes()
            {
                Speed = 0.6f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 0,
                Magic = 0,
            };
            NpcProperties.baseAttributes = baseNpcAttributes;

            // Villager
            NpcProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager/SeparateAnim/Walk"), 4, 4, 48) },
            };
            _dictNpcs.Add("Villager", NpcProperties);

            // Villager2
            NpcProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager2/SeparateAnim/Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager2/SeparateAnim/Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager2/SeparateAnim/Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager2/SeparateAnim/Walk"), 4, 4, 48) },
            };
            _dictNpcs.Add("Villager2", NpcProperties);

            // Villager3
            NpcProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager3/SeparateAnim/Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager3/SeparateAnim/Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager3/SeparateAnim/Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager3/SeparateAnim/Walk"), 4, 4, 48) },
            };
            _dictNpcs.Add("Villager3", NpcProperties);

            // Villager4
            NpcProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager4/SeparateAnim/Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager4/SeparateAnim/Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager4/SeparateAnim/Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Villager4/SeparateAnim/Walk"), 4, 4, 48) },
            };
            _dictNpcs.Add("Villager4", NpcProperties);

            // Woman
            NpcProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Woman/SeparateAnim/Walk"), 4, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Woman/SeparateAnim/Walk"), 4, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Woman/SeparateAnim/Walk"), 4, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/Woman/SeparateAnim/Walk"), 4, 4, 48) },
            };
            _dictNpcs.Add("Woman", NpcProperties);

            // Old Woman (0 speed - no walking movement)
            NpcProperties.animations = new Dictionary<string, Animation>()
            {
                {"WalkDown",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/OldWoman/SpriteSheet"), 2, 4, 0)  },
                {"WalkUp",      new Animation(Globals.content.Load<Texture2D>("Actor/Characters/OldWoman/SpriteSheet"), 2, 4, 16) },
                {"WalkLeft",    new Animation(Globals.content.Load<Texture2D>("Actor/Characters/OldWoman/SpriteSheet"), 2, 4, 32) },
                {"WalkRight",   new Animation(Globals.content.Load<Texture2D>("Actor/Characters/OldWoman/SpriteSheet"), 2, 4, 48) },
            };
            var OldWomanAttributes = new Attributes()
            {
                Speed = 0f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 0,
                Magic = 0,
            };
            NpcProperties.baseAttributes = OldWomanAttributes;
            _dictNpcs.Add("OldWoman", NpcProperties);



        }

        protected void LoadLibraryWeapons()
        {
            _dictWeapons = new Dictionary<string, SpriteProperties>();

            SpriteProperties WeaponProperties = new SpriteProperties();

            // Lance
            WeaponProperties.animations = new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Lance/Sprite"), 1) },
                {"SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Lance/SpriteInHand"), 1) }
            };
            WeaponProperties.baseAttributes = new Attributes()
            {
                Speed = 0f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };
            _dictWeapons.Add("Lance", WeaponProperties);

            // Big Sword
            WeaponProperties.animations = new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/BigSword/Sprite"), 1) },
                {"SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/BigSword/SpriteInHand"), 1) }
            };
            WeaponProperties.baseAttributes = new Attributes()
            {
                Speed = -0.5f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 20,
                Magic = 0,
            };
            _dictWeapons.Add("BigSword", WeaponProperties);

            // Sai
            WeaponProperties.animations = new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Sai/Sprite"), 1) },
                {"SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/Sai/SpriteInHand"), 1) }
            };
            WeaponProperties.baseAttributes = new Attributes()
            {
                Speed = 0.25f,
                Health = 0,
                Mana = 0,
                Stamina = 0,
                Attack = 10,
                Magic = 0,
            };
            _dictWeapons.Add("Sai", WeaponProperties);

        }

        /// <summary>
        /// Load Player-specific Content.
        /// Includes player animations and ...
        /// </summary>
        protected void LoadContentPlayer(Vector2 spawnPosition)
        {
            // Content path for Player
            var path = "Actor/Characters/BlueNinja/SeparateAnim/";

            // Player animations
            var playerAnimations = new Dictionary<string, Animation>()
            {
                // walk
                {"WalkDown"   , new Animation(texture: Globals.content.Load<Texture2D>(path + "Walk"),
                                              frameCount: 4,
                                              spritesheetColumns: 4,
                                              frameLocation: 0)                                         },
                {"WalkUp"     , new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 16) },
                {"WalkLeft"   , new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 32) },
                {"WalkRight"  , new Animation(Globals.content.Load<Texture2D>(path + "Walk"), 4, 4, 48) },
                
                // idle
                {"IdleDown"   , new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 0)  },
                {"IdleUp"     , new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 16) },
                {"IdleLeft"   , new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 32) },
                {"IdleRight"  , new Animation(Globals.content.Load<Texture2D>(path + "Idle"), 1, 4, 48) },

                // attack
                {"AttackDown" , new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 0)  },
                {"AttackUp"   , new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 16) },
                {"AttackLeft" , new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 32) },
                {"AttackRight", new Animation(Globals.content.Load<Texture2D>(path + "Attack"), 1, 4, 48) },

                // jump
                {"JumpDown"   , new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 0)  },
                {"JumpUp"     , new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 16) },
                {"JumpLeft"   , new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 32) },
                {"JumpRight"  , new Animation(Globals.content.Load<Texture2D>(path + "Jump"), 1, 4, 48) },

                // other actions
                {"Item"       , new Animation(Globals.content.Load<Texture2D>(path + "Item"), 1)     },
                {"Special1"   , new Animation(Globals.content.Load<Texture2D>(path + "Special1"), 1) },
                {"Special2"   , new Animation(Globals.content.Load<Texture2D>(path + "Special2"), 1) },
                {"Dead"       , new Animation(Globals.content.Load<Texture2D>(path + "Dead"), 1)     },
            };
            
            // Base Attributes
            var playerAttributes = new Attributes()
            {
                Speed = 1.5f,
                Health = 100,
                Mana = 100,
                Stamina = 100,
                Attack = 20,
                Magic = 40,
            };

            // Initialize player
            Globals.player = new Player(
                animations: playerAnimations,
                spawnPosition: spawnPosition,
                baseAttributes: playerAttributes
                );
            _player = Globals.player;

            // Add a couple starting weapons to player weapon inventory
            _player.PickUp(new Weapon(_dictWeapons["Lance"].animations, _dictWeapons["Lance"].baseAttributes));
            _player.PickUp(new Weapon(_dictWeapons["BigSword"].animations, _dictWeapons["BigSword"].baseAttributes));
            _player.PickUp(new Weapon(_dictWeapons["Sai"].animations, _dictWeapons["Sai"].baseAttributes));
        }

        /// <summary>
        /// UnloadContent method - Unload game-specific content from the Content project.
        /// This method is only called one per game.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non-ContentManager content

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

            // Update all sprites
            foreach (var sprite in _spritesCollidable)
                sprite.Update(gameTime, _spritesCollidable);

            // Update Hud
            hud.Update(_player);
            

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

            // Begin Spritebatch
            Globals.spriteBatch.Begin(transformMatrix: _camera.Transform);

            // Draw tiled map
            tileMapManager.Draw(gameTime, Globals.spriteBatch);

            // Draw all the sprites
            foreach (var sprite in _spritesCollidable)
                sprite.Draw(gameTime, Globals.spriteBatch);

            // Draw Hud
            hud.Draw(Globals.spriteBatch);

            // End Spritebatch
            Globals.spriteBatch.End();
        }


        #endregion Methods

    }
}
