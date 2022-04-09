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

        // the current game state
        private State _currentState;

        // the next game state to use
        private State _nextState;

        // content libaries
        private Dictionary<string, SpriteProperties> _dictAnimals { get => Globals.libraryAnimals; set => Globals.libraryAnimals = value; }
        private Dictionary<string, SpriteProperties> _dictBosses { get => Globals.libraryBosses; set => Globals.libraryBosses = value; }
        private Dictionary<string, SpriteProperties> _dictEnemies { get => Globals.libraryEnemies; set => Globals.libraryEnemies = value; }
        private Dictionary<string, SpriteProperties> _dictNpcs { get => Globals.libraryNpcs; set => Globals.libraryNpcs = value; }
        private Dictionary<string, SpriteProperties> _dictWeapons { get => Globals.libraryWeapons; set => Globals.libraryWeapons = value; }

        // spell content libary does not need attributes
        private Dictionary<string, Dictionary<string, Animation>> _dictSpells { get => Globals.librarySpells; set => Globals.librarySpells = value; }

        /// <summary>
        /// Structure for initializing default SpriteProperties for sprites in content library.
        /// </summary>
        public struct SpriteProperties
        {
            // animation dictionary
            public Dictionary<string, Animation> animations;

            // sprite attributes
            public Attributes baseAttributes;

            // override methods to improve performance of structure
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
        }

        /// <summary>
        /// ChangeState method - prepare the next state.
        /// Update method will go to the next state accordingly.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(State state)
        {
            _nextState = state;
        }

        /// <summary>
        /// Initialize method - Initialize the game upon startup.
        /// This method is called after the constructor but before the main game loop.
        /// Method is used to query any required services and load any non-graphic related content.
        /// </summary>
        protected override void Initialize()
        {
            // Set Game Window size
            screenWidth = 1280;
            screenHeight = 720;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();

            // make mouse visible
            IsMouseVisible = true;

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

            // Load all content libraries (dictionaries with their animations/attributes)
            LoadLibraryAnimals();
            LoadLibraryBosses();
            LoadLibraryEnemies();
            LoadLibraryNpcs();
            LoadLibraryWeapons();
            LoadLibrarySpells();


            _currentState = new MenuState(this, graphics.GraphicsDevice);

            base.LoadContent();
        }

        /// <summary>
        /// LoadLibraryAnimals method to load and populate content libary of future animal sprites.
        /// </summary>
        protected void LoadLibraryAnimals()
        {
            // initialize
            _dictAnimals = new Dictionary<string, SpriteProperties>();
            var AnimalProperties = new SpriteProperties();

            // initialize & set basic animal Attributes
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

        /// <summary>
        /// LoadLibraryBosses method to load and populate content libary of future boss sprites.
        /// </summary>
        protected void LoadLibraryBosses()
        {
            // initialize
            _dictBosses = new Dictionary<string, SpriteProperties>();
            var BossProperties = new SpriteProperties();
            

            // intialize & set basic Boss Attributes
            var baseAttributes = new Attributes()
            {
                Speed = 0.3f,
                Health = 400,
                Mana = 0,
                Stamina = 0,
                Attack = 25,
                Magic = 25,
            };
            BossProperties.baseAttributes = baseAttributes;

            // Boss Demon Cyclop
            BossProperties.animations = new Dictionary<string, Animation>()
            {
                {"Walk", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Walk"), 6) },
                {"Hit", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Hit"), 3) },
                {"Idle", new Animation(Globals.content.Load<Texture2D>("Actor/Boss/DemonCyclop/Idle"), 5) },
            };
            _dictBosses.Add("DemonCyclop", BossProperties);


        }

        /// <summary>
        /// LoadLibraryEnemies method to load and populate content libary of future enemy sprites.
        /// </summary>
        protected void LoadLibraryEnemies()
        {
            // initialize
            _dictEnemies = new Dictionary<string, SpriteProperties>();
            var EnemyProperties = new SpriteProperties();

            // intialize & set basic Enemy attributes
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

        /// <summary>
        /// LoadLibraryNpcs method to load and populate content libary of future npc sprites.
        /// </summary>
        protected void LoadLibraryNpcs()
        {
            // initialize
            _dictNpcs = new Dictionary<string, SpriteProperties>();
            var NpcProperties = new SpriteProperties();

            // initialize & set basic NPC Attributes
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

        /// <summary>
        /// LoadLibrarySpells method to load and populate content libary of future spell sprites.
        /// Spells are special sprites in that they do not need attributes.
        /// The player/enemy object using creating the spell will set its attributes like magic & speed.
        /// </summary>
        protected void LoadLibrarySpells()
        {
            // intialize
            _dictSpells = new Dictionary<string, Dictionary<string, Animation>>();


            // IceSpike
            _dictSpells.Add("IceSpikeProjectile", new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("FX/Projectile/IceSpike"), 8) }
            });

            // Shuriken
            _dictSpells.Add("ShurikenProjectile", new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("FX/Projectile/Shuriken"), 2) }
            });

            // Fireball
            _dictSpells.Add("FireballProjectile", new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("FX/Projectile/Fireball"), 4) }
            });

            // Ice Elemental            
            _dictSpells.Add("IceElemental", new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("FX/Elemental/Ice/SpriteSheetB"), 9) },
                {"Cast", new Animation(Globals.content.Load<Texture2D>("FX/Elemental/Ice/SpriteSheetFlake"), 9) },
            });

            // Fire Elemental
            _dictSpells.Add("FlameElemental", new Dictionary<string, Animation>()
            {
                {"Sprite", new Animation(Globals.content.Load<Texture2D>("FX/Elemental/Flam/SpriteSheet"), 5) },
                {"Cast", new Animation(Globals.content.Load<Texture2D>("FX/Smoke/Smoke/SpriteSheet"), 6) },
            });

        }

        /// <summary>
        /// LoadLibraryWeapons method to load and populate content libary of future weapon sprites.
        /// </summary>
        protected void LoadLibraryWeapons()
        {
            // initialize
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
                Stamina = 50,
                Attack = 10,
                Magic = 0,
            };
            _dictWeapons.Add("Sai", WeaponProperties);

            // MagicWand
            WeaponProperties.animations = new Dictionary<string, Animation>()
            {
                { "Sprite", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/MagicWand/Sprite"), 1) },
                { "SpriteInHand", new Animation(Globals.content.Load<Texture2D>("Items/Weapons/MagicWand/SpriteInHand"), 1) }
            };
            WeaponProperties.baseAttributes = new Attributes()
            {
                Speed = 0f,
                Health = 0,
                Mana = 50,
                Stamina = 25,
                Attack = 0,
                Magic = 20,
            };
            _dictWeapons.Add("MagicWand", WeaponProperties);


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
            if(_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }

            _currentState.Update(gameTime);
            _currentState.PostUpdate(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw Method - Called on a regular interval to draw the game entities to the screen.
        /// This method is called multiple times per second.
        /// IMPORTANT: The order of which draw methods are called is important in determining draw order.
        /// WARNING: Do not include update logic in Draw method; Use the Update method instead.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _currentState.Draw(gameTime);

            base.Draw(gameTime);
        }


        #endregion Methods

    }
}
