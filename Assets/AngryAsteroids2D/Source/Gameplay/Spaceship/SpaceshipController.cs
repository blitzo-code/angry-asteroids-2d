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
        public float FireRate;
    }

    public class SpaceshipController
    {
        readonly SpaceshipConfig _config;
        readonly Transform _transform;
        readonly PhysicsBody _body;

        float lastFireTime = float.MinValue;
        float projectileSpeed = 24f;
        
        public SpaceshipController(Transform transform, PhysicsBody body, SpaceshipConfig config)
        {
            _transform = transform;
            _config = config;
            _body = body;
        }

        public void Accelerate()
        {
            var propulsionPower = _transform.up * _config.PropulsionPower; 
            _body.AddContinuousForce(propulsionPower);
        }

        public void TurnBody(float turnDirection)
        {
            var rotationAngle = _config.TurnSpeed * turnDirection;
            _transform.Rotate(Vector3.forward, rotationAngle);
        }

        public void FireAction()
        {
            var projectile = GameManager.GetInstance().GameManagers.ProjectileManager.CreateProjectile(ProjectileType.MissileProjectile, _transform.position + _transform.up, _transform.rotation);
            projectile.Body.SetVelocity(_transform.up * projectileSpeed);
        }
    }
}
