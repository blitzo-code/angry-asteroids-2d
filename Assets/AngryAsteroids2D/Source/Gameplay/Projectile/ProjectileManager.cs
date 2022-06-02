using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Physics;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Projectile
{
    public class ProjectileManager
    {
        readonly ProjectileFactory _projectileFactory;
        readonly List<Projectile> _projectiles;
        
        public ProjectileManager(ProjectileDatabase projectileDatabase)
        {
            _projectileFactory = new ProjectileFactory(projectileDatabase.GetProjectileDataDictionary());
            _projectiles = new List<Projectile>();
        }

        public Projectile CreateProjectile(ProjectileType projectileType, Vector3 position, Quaternion rotation, ContactFilter2D filter2D, Action<Collider2D,Collider2D> onProjectileHitCallback)
        {
            var projectile = _projectileFactory.SpawnProjectile(projectileType, position, rotation, filter2D);
            projectile.OnCollision = onProjectileHitCallback;
            
            InitializeProjectile(projectile);
            return projectile;
        }

        public void DestroyProjectile(int gameObjectId)
        {
            int removeIndex = -1;
            for (var i = 0; i < _projectiles.Count; i++)
            {
                var projectile = _projectiles[i];
                if (projectile.GameObject.GetInstanceID() == gameObjectId)
                {
                    removeIndex = i;
                    break;
                }
            }

            if (removeIndex == -1)
            {
                return;
            }
            
            DisposeProjectile(removeIndex);
        }
        
        void InitializeProjectile(Projectile projectile)
        {
            _projectiles.Add(projectile);
            ConnectProjectileSystems(projectile);
        }

        void DisposeProjectile(int removeIndex)
        {
            var projectile = _projectiles[removeIndex];
            DisposeAsteroidSystems(projectile);
            
            _projectiles.RemoveAt(removeIndex);
            _projectileFactory.DisableProjectile(projectile.GameObject.GetInstanceID());
        }
        
        void ConnectProjectileSystems(Projectile projectile)
        {
            var entityId = projectile.GameObject.GetInstanceID();
            
            var physicsEntity = new PhysicsEntity(projectile.GameObject, projectile.Body);
            GameManager.GetInstance().GameSystems.PhysicsSystem.AddEntity(entityId, physicsEntity);

            var collisionListenerEntity = new CollisionListenerEntity(projectile.Collider, projectile.ContactFilter, projectile.OnCollision);
            GameManager.GetInstance().GameSystems.CollisionMessageSystem.AddEntity(entityId, collisionListenerEntity);
            
            var screenLimitWatchEntity = new ScreenLimitWatchEntity(projectile.GameObject.transform, DestroyProjectile);
            GameManager.GetInstance().GameSystems.ScreenLimitDestroySystem.AddEntity(entityId, screenLimitWatchEntity);
            
            projectile.GameObject.SetActive(true);
        }

        void DisposeAsteroidSystems(Projectile projectile)
        {
            var projectileId = projectile.GameObject.GetInstanceID();
           
            GameManager.GetInstance().GameSystems.CollisionMessageSystem.RemoveEntity(projectileId);
            GameManager.GetInstance().GameSystems.ScreenLimitDestroySystem.RemoveEntity(projectileId);
            GameManager.GetInstance().GameSystems.PhysicsSystem.RemoveEntity(projectileId);
        }
        
    }
}
