using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Gameplay.Player;
using AngryAsteroids2D.Source.Gameplay.Projectile;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.UFO
{
    public class UFOTargetEntity
    {
        public readonly Transform UFOTransform;
        public readonly Transform TargetTransform;
        public readonly float ProjectileSpeed;
        public readonly float TimeBetweenShots;
        
        public float Timer;
        
        public UFOTargetEntity(Transform ufoTransform, Transform targetTransform, float projectileSpeed, float timeBetweenShots)
        {
            UFOTransform = ufoTransform;
            TargetTransform = targetTransform;
            TimeBetweenShots = timeBetweenShots;
            ProjectileSpeed = projectileSpeed;
        }
    }
    
    public class UFOTargetSystem : GameSystem<UFOTargetEntity>
    {
        readonly ProjectileManager _projectileManager;
        readonly PlayerManager _playerManager;
        readonly ContactFilter2D _ufoProjectileFilter;
        
        public UFOTargetSystem(int maxEntities) : base(maxEntities)
        {
            _projectileManager = GameManager.GetInstance().GameManagers.ProjectileManager;
            _playerManager = GameManager.GetInstance().GameManagers.PlayerManager;
            _ufoProjectileFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Player")
            };
        }
        
        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                LockAndShoot(GetEntityData(i));
            }
        }

        void LockAndShoot(UFOTargetEntity entity)
        {
            if (!entity.TargetTransform.gameObject.activeSelf)
            {
                entity.Timer = 0;
                return;
            }
            
            entity.Timer += Time.deltaTime;
            if (entity.Timer > entity.TimeBetweenShots)
            {
                Shoot(entity);
                entity.Timer = 0;
            }
        }

        void Shoot(UFOTargetEntity entity)
        {
            var ufoTransform = entity.UFOTransform;
            
            var shootDirection = (entity.TargetTransform.position - ufoTransform.position).normalized;
            var rotationAngle = Vector2.SignedAngle(Vector2.up, shootDirection);
            var rotation = Quaternion.Euler(0, 0, rotationAngle);
            
            var projectile = _projectileManager.CreateProjectile(ProjectileType.MissileProjectile, ufoTransform.position, rotation, _ufoProjectileFilter, OnProjectileHit);
            projectile.Body.SetVelocity(shootDirection * entity.ProjectileSpeed);
        }

        void OnProjectileHit(Collider2D self, Collider2D other)
        {
            _playerManager.KillPlayer();
        }
        
    }
}
