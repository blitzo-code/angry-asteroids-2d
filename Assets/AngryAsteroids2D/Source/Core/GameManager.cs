using AngryAsteroids2D.Source.Data.Asteroids;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Data.Player;
using AngryAsteroids2D.Source.Data.UFO;
using AngryAsteroids2D.Source.Gameplay.Asteroid;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Gameplay.Player;
using AngryAsteroids2D.Source.Gameplay.Projectile;
using AngryAsteroids2D.Source.Gameplay.Spaceship;
using AngryAsteroids2D.Source.Gameplay.UFO;
using AngryAsteroids2D.Source.Physics;
using AngryAsteroids2D.Source.UI;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AngryAsteroids2D.Source.Core
{
    public struct GameSystems
    {
        public ScreenBorderPortalSystem ScreenBorderPortalSystem;
        public CollisionMessageSystem CollisionMessageSystem;
        public PhysicsSystem PhysicsSystem;
        public ScreenLimitDestroySystem ScreenLimitDestroySystem;
        public UFOTargetSystem UFOTargetSystem;
        public PathFollowSystem PathFollowSystem;
        public PlayerRespawnSystem PlayerRespawnSystem;
    }

    public struct GameManagers
    {
        public PlayerManager PlayerManager;
        public AsteroidManager AsteroidManager;
        public UFOManager UFOManager;
        public EnemyWaveManager EnemyWaveManager;
        public ProjectileManager ProjectileManager;
        public PlayerDataManager PlayerDataManager;
    }
    
    public class GameManager : MonoBehaviour
    {
        /*
         * I've taken the shortcut of centralizing stuff on this script, but for sure
         * it's not the best way to do it.
         *  
         * For this kind of game, we won't need a loading screen/process.
         * But for games that are heavier to load, we could create a loading screen that would
         * load assets and initialize systems async
         *
         * The advantages of doing the loading approach is that you have full control about the initialization order
         * of each system, instead of initializing purely via Awake or Start methods located on MonoBehaviors
         * in the scene
         *
         * Also, for the asset management, we could use unity's addressable system, which offers good enough
         * functionalities for asset loading
         */
        
        [Header("Prefab databases")]
        [SerializeField] AsteroidDatabase asteroidDatabase;
        [SerializeField] ProjectileDatabase projectileDatabase;
        [SerializeField] UFODataAsset ufoDataAsset;
        
        [Header("Game configs")] 
        [SerializeField] LevelConfigAsset levelConfigAsset;
        
        [Header("Player")] 
        [SerializeField] SpaceshipCore spaceshipPrefab;
        [SerializeField] PlayerDataAsset playerDataAsset;

        [Header("HUD")] 
        [SerializeField] GameplayHUD gameplayHUD;

        [Header("LevelComponents")] 
        [SerializeField] Collider2D safeZone;
        
        static GameManager instance;
        
        /* Components */ 
        Camera _camera;
        
        /* Systems */
        GameSystems _gameSystems;
        
        /* Managers */
        GameManagers _gameManagers;
        
        /*
         * GameSystems should be turned into a dependency management system.
         * Systems could be initialized, loaded, unloaded and accessed with ease if we would
         * do this
         */
        public GameSystems GameSystems => _gameSystems;
        
        /*
         * Managers could initialized and retrieved by a dependency system as well
         */
        public GameManagers GameManagers => _gameManagers;
        
        void Awake()
        {
            if (instance)
            {
                LogUtils.LogDuplicatedSingletonError(GetType().Name, gameObject);
                return;
            }
            instance = this;
        }

        void Start()
        {
            BootGame();
        }

        void BootGame()
        {
            _camera = FindObjectOfType<Camera>();
            _gameManagers = CreateGameManagers();
            _gameSystems = CreateGameSystems();
            
            InitializeUI();
            InitializeGameManagers();
        }

        GameSystems CreateGameSystems()
        {
            var physicsSystem = new PhysicsSystem(250);
            var collisionMessageSystem = new CollisionMessageSystem(20, 250);
            var screenBorderPortalSystem = new ScreenBorderPortalSystem(_camera, safeZone, 250);
            var screenLimitDestroySystem = new ScreenLimitDestroySystem(_camera, 250);
            var ufoTargetSystem = new UFOTargetSystem( 2);
            var pathFollowSystem = new PathFollowSystem(2);
            var playerRespawnSystem = new PlayerRespawnSystem(2);
            
           return new GameSystems
            {
                ScreenBorderPortalSystem = screenBorderPortalSystem,
                CollisionMessageSystem = collisionMessageSystem,
                PhysicsSystem = physicsSystem,
                ScreenLimitDestroySystem = screenLimitDestroySystem,
                UFOTargetSystem = ufoTargetSystem,
                PathFollowSystem = pathFollowSystem,
                PlayerRespawnSystem = playerRespawnSystem
            };
        }

        GameManagers CreateGameManagers()
        {
            return new GameManagers
            {
                PlayerManager = new PlayerManager(),
                AsteroidManager = new AsteroidManager(asteroidDatabase),
                UFOManager = new UFOManager(ufoDataAsset),
                EnemyWaveManager = new EnemyWaveManager(levelConfigAsset.LevelWaves),
                ProjectileManager = new ProjectileManager(projectileDatabase),
                PlayerDataManager= new PlayerDataManager(_camera, playerDataAsset)
            };
        }

        void InitializeUI()
        {
            gameplayHUD.LinkEvents(_gameManagers.EnemyWaveManager, _gameManagers.PlayerDataManager);
        }

        void InitializeGameManagers()
        {
            _gameManagers.PlayerManager.Initialize(_camera, spaceshipPrefab); 
            _gameManagers.EnemyWaveManager.Initialize();
            _gameManagers.PlayerDataManager.Initialize(TriggerGameOver);
        }
        
        void Update()
        {
            //Could loop through the systems as well
            GameSystems.PhysicsSystem.Tick();
            GameSystems.CollisionMessageSystem.Tick();
            GameSystems.ScreenBorderPortalSystem.Tick();
            GameSystems.ScreenLimitDestroySystem.Tick();
            GameSystems.PathFollowSystem.Tick();
            GameSystems.UFOTargetSystem.Tick();
            GameSystems.PlayerRespawnSystem.Tick();
        }

        void TriggerGameOver()
        {
            //Shortcut to reset the game...
            SceneManager.LoadScene("GameplayScene");
        }
        
        public static GameManager GetInstance()
        {
            return instance;
        }
    }
}

