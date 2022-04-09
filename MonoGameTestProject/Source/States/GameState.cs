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
    /// GameState class.
    /// The Main Game State that loads the level and plays the game.
    /// </summary>
    public class GameState : State
    {
        #region Members

        // Game screen camera object
        private Camera _camera;

        // timer before display death screen
        private float _deathScreenTimer = 0f;

        // content libaries
        private Dictionary<string, Game1.SpriteProperties> _dictAnimals { get => Globals.libraryAnimals; }
        private Dictionary<string, Game1.SpriteProperties> _dictBosses { get => Globals.libraryBosses; }
        private Dictionary<string, Game1.SpriteProperties> _dictEnemies { get => Globals.libraryEnemies; }
        private Dictionary<string, Game1.SpriteProperties> _dictNpcs { get => Globals.libraryNpcs; }
        private Dictionary<string, Game1.SpriteProperties> _dictWeapons { get => Globals.libraryWeapons; }

        // spell content libary does not need attributes
        private Dictionary<string, Dictionary<string, Animation>> _dictSpells { get => Globals.librarySpells; }

        // Player
        private Player _player { get => Globals.player; set => Globals.player = value; }

        // list of sprite spawn positions pulled from the map
        private List<(string key, Vector2 spawnPosition)> _spawnPositions;

        // List of sprites that have collision detection
        private List<Sprite> _sprites { get => Globals.sprites; set => Globals.sprites = value; }

        // Tiled map import variables
        private TileMapManager tileMapManager;
        private TmxMap map;
        private Texture2D tileset;

        // check if death screen should be displayed
        public bool IsDeathScreen = false;

        // Game Level
        public int Level { get; set; }

        // Hud import
        Hud hud;

        #endregion Members

        /// <summary>
        /// Game State Constructor - Starts game State and loads the game level.
        /// </summary>
        public GameState(Game1 game, GraphicsDevice graphics, int level): base(game, graphics)
        {
            Level = level;
            
            LoadLevel();
        }

        /// <summary>
        /// LoadLevel Method for Game State - Load the tile map file to load the game state.
        /// This method is called only once per new game state within the constructor.
        /// </summary>
        private void LoadLevel()
        {
            // get Tiled map file path
            switch (Level)
            {
                case 1:
                    map = new TmxMap("Content/Level1.tmx");
                    break;
                case 2:
                    map = new TmxMap("Content/Level1.tmx");
                    break;
                default:
                    map = new TmxMap("Content/Level1.tmx");
                    break;
            }
            tileset = Globals.content.Load<Texture2D>("Backgrounds/Tilesets/" + map.Tilesets[0].Name.ToString());
            int tileWidth = map.Tilesets[0].TileWidth;
            int tileHeight = map.Tilesets[0].TileHeight;
            int tilesetTilesWide = tileset.Width / tileWidth;
            // Creates the tiled map using the TiledMapMananger class
            tileMapManager = new TileMapManager(map, tileset, tilesetTilesWide, tileWidth, tileHeight);

            // Instantiate camera
            _camera = new Camera();

            // Load sprites
            LoadSprites();

            // Instantiate Hud
            // hud must be instantiated after load sprites so hud has existing sprite properties to be read
            hud = new Hud();
        }

        /// <summary>
        /// LoadContentSprites method to intialize sprites into game state.
        /// </summary>
        private void LoadSprites()
        {
            LoadPlayer();

            // Load prefabricated sprites for level
            var npcVillager = new Npc(_dictNpcs["Villager"].animations, _dictNpcs["Villager"].baseAttributes);
            var npcVillager2 = new Npc(_dictNpcs["Villager2"].animations, _dictNpcs["Villager2"].baseAttributes);
            var npcVillager3 = new Npc(_dictNpcs["Villager3"].animations, _dictNpcs["Villager3"].baseAttributes);
            var npcVillager4 = new Npc(_dictNpcs["Villager4"].animations, _dictNpcs["Villager4"].baseAttributes);
            var npcWoman = new Npc(_dictNpcs["Woman"].animations, _dictNpcs["Woman"].baseAttributes);
            var npcDog = new Npc(_dictAnimals["Dog"].animations, _dictAnimals["Dog"].baseAttributes) { IsStationary = true };
            var enemyOcotopus = new Enemy(_dictEnemies["Octopus"].animations, _dictEnemies["Octopus"].baseAttributes);
            var enemyOcotopus2 = new Enemy(_dictEnemies["Octopus2"].animations, _dictEnemies["Octopus2"].baseAttributes);
            var enemyCyclope = new Enemy(_dictEnemies["Cyclope"].animations, _dictEnemies["Cyclope"].baseAttributes) { GoldGiven = 15 };
            var enemyCyclope2 = new Enemy(_dictEnemies["Cyclope2"].animations, _dictEnemies["Cyclope2"].baseAttributes) { GoldGiven = 15 };
            var npcOldWoman = new Npc(_dictNpcs["OldWoman"].animations, _dictNpcs["OldWoman"].baseAttributes);
            var bossDemonCyclop = new Boss(_dictBosses["DemonCyclop"].animations, _dictBosses["DemonCyclop"].baseAttributes);

            // Initialize list of sprites
            _sprites = new List<Sprite>() { };

            // Initialize random for randomly initializing different sprites of same type
            var random = new Random();

            // Get spawn positions from tiled map
            _spawnPositions = tileMapManager.LoadSpawnPositions();

            // Add each sprite after setting their spawn position
            foreach (var item in _spawnPositions)
            {
                switch (item.key)
                {
                    case "BlueX": // Instantiate and Load Player
                        _player.SpawnPosition = item.spawnPosition;
                        _sprites.Add(_player);
                        break;

                    case "GreenX": // Load Villager
                        var randVillager = random.Next(0, 5);
                        Npc villager;
                        switch (randVillager)   // random villager
                        {
                            case 0:
                                villager = npcVillager.Clone() as Npc;
                                break;
                            case 1:
                                villager = npcVillager2.Clone() as Npc;
                                break;
                            case 2:
                                villager = npcVillager3.Clone() as Npc;
                                break;
                            case 3:
                                villager = npcVillager4.Clone() as Npc;
                                break;
                            default:
                                villager = npcWoman.Clone() as Npc;
                                break;
                        }
                        villager.SpawnPosition = item.spawnPosition;
                        _sprites.Add(villager);
                        break;

                    case "YellowX": // Load Dog
                        var dogClone = npcDog.Clone() as Npc;
                        dogClone.SpawnPosition = item.spawnPosition;
                        _sprites.Add(dogClone);
                        break;

                    case "OrangeX": // Nothing
                        break;

                    case "RedX": // Load Enemy1
                        var randOcotpus = random.Next(0, 2);
                        Enemy octopus;
                        switch (randOcotpus) // random enemy1
                        {
                            case 0:
                                octopus = enemyOcotopus.Clone() as Enemy;
                                break;
                            default:
                                octopus = enemyOcotopus2.Clone() as Enemy;
                                break;
                        }
                        octopus.SpawnPosition = item.spawnPosition;
                        _sprites.Add(octopus);
                        break;

                    case "PurpleX": // Load Enemy2
                        var randCyclope = random.Next(0, 2);
                        Enemy cyclope;
                        switch (randCyclope) // random enemy2
                        {
                            case 0:
                                cyclope = enemyCyclope.Clone() as Enemy;
                                break;
                            default:
                                cyclope = enemyCyclope2.Clone() as Enemy;
                                break;
                        }
                        cyclope.SpawnPosition = item.spawnPosition;
                        _sprites.Add(cyclope);
                        break;

                    case "BlackX": // Load Old Woman Npc
                        var oldWomanClone = npcOldWoman.Clone() as Npc;
                        oldWomanClone.SpawnPosition = item.spawnPosition;
                        _sprites.Add(oldWomanClone);
                        break;

                    case "WhiteX": // Load Boss
                        var demonCyclopClone = bossDemonCyclop.Clone() as Boss;
                        demonCyclopClone.SpawnPosition = item.spawnPosition;
                        demonCyclopClone.SetPhase1Threshold(350);
                        demonCyclopClone.SetPhase2Threshold(200);
                        demonCyclopClone.SetSpecial1(
                            spell: new ProjectileAimed(_dictSpells["FireballProjectile"]),
                            magic: 0,
                            speed: 1f,
                            lifespan: 3f);
                        demonCyclopClone.SetSpecial2(
                            spell: new AoeSurroundSelf(_dictSpells["FlameElemental"], 1.25f, 5),
                            magic: 10,
                            speed: 0f,
                            lifespan: 0.5f);

                        _sprites.Add(demonCyclopClone);
                        break;

                    default:
                        break;

                }
            }

            // set enemy FollowTarget to player
            foreach (var sprite in _sprites)
            {
                if (sprite is Enemy)
                    ((Enemy)sprite).SetFollowTarget(_player, 10f); // 10f to be close enough to collide with player hitbox
            }
        }

        /// <summary>
        /// Load Player-specific Content.
        /// This loads player content and initializes player properties:
        /// - content path
        /// - set player animations
        /// - set base attributes
        /// - initialize player
        /// - give player some starting weapons
        /// - set the special actions/skills for player
        /// </summary>
        private void LoadPlayer()
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
                Magic = 10,
            };

            // Initialize player
            _player = new Player(
                animations: playerAnimations,
                baseAttributes: playerAttributes
                );

            // Add a couple starting weapons to player weapon inventory
            _player.PickUp(new Weapon(_dictWeapons["Lance"].animations, _dictWeapons["Lance"].baseAttributes, "Lance"));
            _player.PickUp(new Weapon(_dictWeapons["BigSword"].animations, _dictWeapons["BigSword"].baseAttributes, "BigSword"));
            _player.PickUp(new Weapon(_dictWeapons["MagicWand"].animations, _dictWeapons["MagicWand"].baseAttributes, "MagicWand"));
            _player.PickUp(new Weapon(_dictWeapons["Sai"].animations, _dictWeapons["Sai"].baseAttributes, "Sai"));

            // Set player specials
            _player.SetSpecial1(new Projectile(_dictSpells["IceSpikeProjectile"]), 10, 2f, 0.5f);
            _player.SetSpecial2(new AoeLine(_dictSpells["IceElemental"], 0.5f, 2, 3), 40, 0f, 2f);

        }

        /// <summary>
        /// Update Method for Game State - Called on a regular interval to update the game state.
        /// Updating game state includes taking player input, checking for collision, playing audio, animating entities, etc...
        /// This method is called multiple times per second.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                _game.ChangeState(new PauseState(_game, _graphics, this));

            // Update all sprites
            foreach (var sprite in _sprites)
                sprite.Update(gameTime, _sprites);

            // update camera position to follow player
            _camera.Follow(_player);

            // Update Hud
            hud.Update();


            PostUpdate(gameTime);
        }

        /// <summary>
        /// Post-Update method for Game State - Called at the end of Update as a post-check to:
        /// - Add new children sprites to the list of _sprites and clear
        /// - Remove all "IsRemoved" sprites
        /// - Sort sprites by its current Y position to create a 2.5D illusion effect
        /// - Check if player is dead and display the death screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void PostUpdate(GameTime gameTime)
        {
            // Add Children to the list of "_sprites" and clear
            int count = _sprites.Count;
            for (int i = 0; i < count; i++)
            {
                foreach (var child in _sprites[i].Children)
                    _sprites.Add(child);

                _sprites[i].Children.Clear();
            }

            // Remove all "IsRemoved" sprites
            for (int i = 0; i < _sprites.Count; i++)
            {
                if (_sprites[i].IsRemoved)
                {
                    _sprites.RemoveAt(i);
                    i--;
                }
            }

            // Sort sprites by its current Y Position to create a 2.5D illusion effect
            _sprites.Sort((spriteA, spriteB) => spriteA.Position.Y.CompareTo(spriteB.Position.Y));

            // display the death screen if player is dead
            if (_player.IsDead == true)
            {
                _deathScreenTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (IsDeathScreen == true || _deathScreenTimer > 3f)
                {
                    IsDeathScreen = true;
                    _game.ChangeState(new PauseState(_game, _graphics, this));
                }
            }
        }

        /// <summary>
        /// Draw Method for Game State - Called on a regular interval to draw the game entities to the screen.
        /// This method is called multiple times per second.
        /// IMPORTANT: The order of which draw methods are called is important in determining draw order.
        /// WARNING: Do not include update logic in Draw method; Use the Update method instead.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Begin Spritebatch
            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            // Draw tiled map
            tileMapManager.Draw(gameTime, _spriteBatch);

            // Draw all the sprites
            foreach (var sprite in _sprites)
                sprite.Draw(gameTime, _spriteBatch);

            // Draw Hud
            hud.Draw(_spriteBatch);

            // End Spritebatch
            _spriteBatch.End();
        }

    }
}
