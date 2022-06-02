using System;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Spaceship
{
    [Serializable]
    public class SpaceshipConfig
    {
        public float PropulsionPower;
        public float TurnSpeed;
        public float ProjectileSpeed = 24f;
    }

    public class SpaceshipController
    {
        readonly SpaceshipConfig _config;
        readonly Transform _transform;
        readonly PhysicsBody _body;
        readonly ContactFilter2D _projectileContactFilter;
        
        public SpaceshipController(Transform transform, PhysicsBody body, SpaceshipConfig config)
        {
            _transform = transform;
            _config = config;
            _body = body;
            _projectileContactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Asteroids", "UFO")
            };
        }

        public void Accelerate()
        {
            var propulsionPower = _transform.up * _config.PropulsionPower; 
            _body.AddContinuousForce(propulsionPower);
        }

        public void TurnBody(float turnDirection)
        {
            var rotationSpeed = _config.TurnSpeed * turnDirection ;
            _transform.RotateAround(_transform.position, Vector3.forward, rotationSpeed* Time.deltaTime);
        }

        public void FireAction()
        {
            var projectile = GameManager.GetInstance().GameManagers.ProjectileManager.CreateProjectile(ProjectileType.MissileProjectile, _transform.position + _transform.up, _transform.rotation, _projectileContactFilter, OnProjectileHit);
            projectile.Body.SetVelocity(_transform.up * _config.ProjectileSpeed);
        }

        void OnProjectileHit(Collider2D self, Collider2D other)
        {
            //If the game grew a lot on collision layers, we probably would have to rework this
            
            var collisionLayer = 1 << other.gameObject.layer;
            if ((LayerMask.GetMask("UFO") & collisionLayer) > 0)
            {
                GameManager.GetInstance().GameManagers.UFOManager.KillUFO(other.gameObject.GetInstanceID());
                GameManager.GetInstance().GameManagers.ProjectileManager.DestroyProjectile(self.gameObject.GetInstanceID());
                return;
            }
            
            if ((LayerMask.GetMask("Asteroids") & collisionLayer) > 0)
            {
                GameManager.GetInstance().GameManagers.AsteroidManager.DestroyAsteroid(other.gameObject.GetInstanceID());
                GameManager.GetInstance().GameManagers.ProjectileManager.DestroyProjectile(self.gameObject.GetInstanceID());
                return;
            }
        }
    }
}
