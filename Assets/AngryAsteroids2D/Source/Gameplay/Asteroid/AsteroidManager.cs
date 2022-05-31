using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.Asteroids;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Asteroid
{
    public class AsteroidManager
    {
        readonly AsteroidFactory _asteroidFactory;
        readonly AsteroidDestroyActionResolver _asteroidDestroyActionResolver;
        readonly List<Asteroid> _asteroids;
        
        public AsteroidManager(AsteroidDatabase asteroidDatabase)
        {
            _asteroidFactory = new AsteroidFactory(asteroidDatabase);
            _asteroidDestroyActionResolver = new AsteroidDestroyActionResolver(this);
            _asteroids = new List<Asteroid>();
        }

        public Asteroid CreateAsteroid(AsteroidType asteroidType, Vector3 position)
        {
            var asteroid = _asteroidFactory.SpawnAsteroid(asteroidType, position);
            InitializeAsteroid(asteroid);
            
            return asteroid;
        }
        
        public void DestroyAsteroid(int gameObjectId)
        {
            int removeIndex = -1;
            for (var i = 0; i < _asteroids.Count; i++)
            {
                var asteroid = _asteroids[i];
                if (asteroid.GameObject.GetInstanceID() == gameObjectId)
                {
                    removeIndex = i;
                    break;
                }
            }
            
            if (removeIndex == -1)
            {
                return;
            }
            
            DisposeAsteroid(removeIndex);
        }
        
        void InitializeAsteroid(Asteroid asteroid)
        {
            _asteroids.Add(asteroid);
            ConnectAsteroidSystems(asteroid);
        }

        void DisposeAsteroid(int removeIndex)
        {
            var asteroid = _asteroids[removeIndex];
            DisposeAsteroidSystems(asteroid);
            
            _asteroids.RemoveAt(removeIndex);
            _asteroidDestroyActionResolver.TriggerDestructionAction(asteroid);
            _asteroidFactory.DisableAsteroid(asteroid.GameObject.GetInstanceID());
        }
        
        void ConnectAsteroidSystems(Asteroid asteroid)
        {
            var gameObjectId = asteroid.GameObject.GetInstanceID();
            
            var screenBorderEntityData = new ScreenBorderEntityData
            (
                asteroid.GameObject.transform,
                asteroid.Body,
                asteroid.GameObject.GetComponent<Collider2D>(),
                true
            );
            GameManager.GetInstance().GameSystems.ScreenBorderPortalSystem.AddEntity(gameObjectId, screenBorderEntityData);
            
            var physicsEntity = new PhysicsEntity(
                asteroid.GameObject, 
                asteroid.Body
            );
            GameManager.GetInstance().GameSystems.PhysicsSystem.AddEntity(gameObjectId, physicsEntity);
        }

        void DisposeAsteroidSystems(Asteroid asteroid)
        {
            GameManager.GetInstance().GameSystems.ScreenBorderPortalSystem.RemoveEntity(asteroid.GameObject.GetInstanceID());
            GameManager.GetInstance().GameSystems.PhysicsSystem.RemoveEntity(asteroid.GameObject.GetInstanceID());
        }
    }
}
