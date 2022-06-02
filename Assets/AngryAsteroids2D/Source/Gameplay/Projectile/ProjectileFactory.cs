using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.Physics;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Physics;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Projectile
{
    public class Projectile
    {
        public readonly ProjectileType Type ;
        public readonly GameObject GameObject;
        public readonly Collider2D Collider;
        public readonly PhysicsBody Body;
        public readonly ContactFilter2D ContactFilter;
        public Action<Collider2D, Collider2D> OnCollision;

        public Projectile(ProjectileType projectileType, GameObject gameObject, PhysicsConfig physicsConfig, ContactFilter2D contactFilter2D)
        {
            Type = projectileType;
            GameObject = gameObject;
            Collider = gameObject.GetComponent<Collider2D>();
            Body = new PhysicsBody(gameObject.transform, physicsConfig);
            ContactFilter = contactFilter2D;
        }
    }
    
    public class ProjectileFactory
    {
        readonly SharedGameObjectPool<ProjectileType> _projectilePool;
        readonly Dictionary<ProjectileType, ProjectileDataAsset> _projectileData;
        
        public ProjectileFactory(Dictionary<ProjectileType, ProjectileDataAsset> projectileData)
        {
            _projectilePool = new SharedGameObjectPool<ProjectileType>(250);
            _projectileData = projectileData;
        }

        public Projectile SpawnProjectile(ProjectileType projectileType, Vector3 position, Quaternion rotation, ContactFilter2D collisionFilter)
        {
            var projectileData = _projectileData[projectileType];
            
            var gameObject = _projectilePool.GetOrCreate(projectileType, projectileData.Prefab);
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            
            var projectile = new Projectile(projectileType, gameObject, projectileData.PhysicsConfig, collisionFilter);
            return projectile;
        }

        public void DisableProjectile(int gameObjectId)
        {
            _projectilePool.Deactivate(gameObjectId);
        }
    }
}
