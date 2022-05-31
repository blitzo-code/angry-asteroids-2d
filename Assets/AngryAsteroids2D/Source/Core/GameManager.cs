using AngryAsteroids2D.Source.Data.Asteroids;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Gameplay.Asteroid;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Gameplay.Projectile;
using AngryAsteroids2D.Source.Gameplay.Spaceship;
using AngryAsteroids2D.Source.Input;
using AngryAsteroids2D.Source.Physics;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;

namespace AngryAsteroids2D.Source.Core
{
    public struct GameSystems
    {
        public ScreenBorderPortalSystem ScreenBorderPortalSystem;
        public CollisionMessageSystem CollisionMessageSystem;
        public PhysicsSystem PhysicsSystem;
        public ScreenLimitDestroySystem ScreenLimitDestroySystem;
    }

    public struct GameManagers
    {
        public AsteroidManager AsteroidManager;
        public EnemyWaveManager EnemyWaveManager;
        public ProjectileManager ProjectileManager;
    }
    
    public class GameManager : MonoBehaviour
    {
        /*
         * For this kind of game, we won't need a loading screen/process.
         * But for games that are heavier to load, we could create a loading screen that would
         * load assets and initialize systems async
         *
         * The advantages of doing that is that you have full control about the initialization order
         * of each system, instead of initializing purely via Awake or Start methods located on MonoBehaviors
         * in the scene
         *
         * Also, for the asset management, we could use unity's addressable system, which offers good enough
         * functionalities
         */
        
        [Header("Prefab databases")]
        [SerializeField] AsteroidDatabase asteroidDatabase;
        [SerializeField] ProjectileDatabase projectileDatabase;
        
        [Header("Game configs")] 
        [SerializeField] LevelConfigAsset levelConfigAsset;
        
        [Header("Player")] 
        [SerializeField] SpaceshipCore spaceshipPrefab;
        
        static GameManager INSTANCE;
        
        /* Components */ 
        Camera _camera;
        
        /* Systems */
        GameSystems _gameSystems;
        
        /* Managers */
        GameManagers _gameManagers;
        
        /* MonoBehaviours */ 
        SpaceshipCore _spaceshipCore;

        public GameSystems GameSystems => _gameSystems;
        public GameManagers GameManagers => _gameManagers;
        
        void Awake()
        {
            if (INSTANCE)
            {
                HierarchyUtils.LogDuplicatedSingletonError(GetType().Name, gameObject);
                return;
            }
            INSTANCE = this;
        }

        void Start()
        {
            BootGame();
        }

        void BootGame()
        {
            _camera = FindObjectOfType<Camera>();
            
            InitializeGameSystems();
            InitializeGameManagers();
            InitializePlayerSystems();
            
            _gameManagers.EnemyWaveManager.StartWave();
        }

        void InitializeGameSystems()
        {
            var physicsSystem = new PhysicsSystem(250);
            var collisionMessageSystem = new CollisionMessageSystem(20, 250);
            var screenBorderPortalSystem = new ScreenBorderPortalSystem(_camera, 250);
            var screenLimitDestroySystem = new ScreenLimitDestroySystem(_camera, 250);
            
            _gameSystems = new GameSystems
            {
                ScreenBorderPortalSystem = screenBorderPortalSystem,
                CollisionMessageSystem = collisionMessageSystem,
                PhysicsSystem = physicsSystem,
                ScreenLimitDestroySystem = screenLimitDestroySystem
            };
        }

        void InitializeGameManagers()
        {
            var asteroidManager = new AsteroidManager(asteroidDatabase);
            var enemyWaveManager = new EnemyWaveManager(asteroidManager, levelConfigAsset.LevelWaves);
            var projectileManager = new ProjectileManager(projectileDatabase);
            
            _gameManagers = new GameManagers
            {
                AsteroidManager = asteroidManager,
                EnemyWaveManager = enemyWaveManager,
                ProjectileManager = projectileManager
            };
        }
        
        void Update()
        {
            //Could loop through the systems as well
            GameSystems.PhysicsSystem.Tick();
            GameSystems.CollisionMessageSystem.Tick();
            GameSystems.ScreenBorderPortalSystem.Tick();
            GameSystems.ScreenLimitDestroySystem.Tick();
        }

        void InitializePlayerSystems()
        {
            _spaceshipCore = SpawnPlayer();
            _spaceshipCore.Initialize(InputDeviceType.Keyboard);
}
        
        SpaceshipCore SpawnPlayer()
        {
            var spawnPosition = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -_camera.depth));
            return Instantiate(spaceshipPrefab, spawnPosition, spaceshipPrefab.transform.rotation);
        }

        public static GameManager GetInstance()
        {
            return INSTANCE;
        }
    }
}

