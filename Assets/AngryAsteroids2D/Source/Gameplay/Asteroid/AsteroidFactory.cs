using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.Asteroids;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Data.Physics;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Asteroid
{
    public class Asteroid
    {
        public readonly AsteroidType Type;
        public readonly GameObject GameObject;
        public readonly PhysicsBody Body;
        public readonly Collider2D Collider;
        public readonly ContactFilter2D ContactFilter2D;

        public Asteroid(AsteroidType type, GameObject gameObject, PhysicsConfig physicsConfig, ContactFilter2D contactFilter2D)
        {
            Type = type;
            GameObject = gameObject;
            Body = new PhysicsBody(gameObject.transform, physicsConfig);
            Collider = gameObject.GetComponent<Collider2D>();
            ContactFilter2D = contactFilter2D;
        }
    }
    
    public class AsteroidFactory
    {
        readonly SharedGameObjectPool<AsteroidType> _asteroidPool;
        readonly Dictionary<AsteroidType, AsteroidDataAsset> _asteroidsData;
        readonly ContactFilter2D _collisionFilter;
        
        public AsteroidFactory(AsteroidDatabase asteroidDatabase)
        {
            _asteroidPool = new SharedGameObjectPool<AsteroidType>(250);
            _asteroidsData = asteroidDatabase.GetAsteroidDataDictionary();
            _collisionFilter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Player")
            };
        }

        public Asteroid SpawnAsteroid(AsteroidType asteroidType, Vector3 position)
        {
            var asteroidData = _asteroidsData[asteroidType];
            var gameObject = _asteroidPool.GetOrCreate(asteroidType, asteroidData.Prefab);
            gameObject.transform.position = position;

            var asteroid = new Asteroid(asteroidType, gameObject, asteroidData.PhysicsConfig, _collisionFilter);
            return asteroid;
        }

        public void DisableAsteroid(int gameObjectId)
        {
            _asteroidPool.Deactivate(gameObjectId);
        }
    }
}
