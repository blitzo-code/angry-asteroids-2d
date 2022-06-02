using System;
using AngryAsteroids2D.Source.Data.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Physics
{
    public class PhysicsBody
    {
        readonly Transform _transform;
        readonly PhysicsConfig _config;
        
        Vector3 _velocity;

        public Vector3 Velocity => _velocity;
        public Transform Transform => _transform;
        
        public PhysicsBody(Transform transform, PhysicsConfig config)
        {
            _transform = transform;
            _config = config;
        }
        
        public void AddContinuousForce(Vector3 force)
        {
            _velocity += force * Time.deltaTime;
        }

        public void RunPhysicsLoop()
        {
            ApplyDrag();
            UpdatePosition();
        }

        void ApplyDrag()
        {
            var dragForce = -1 * _velocity.normalized * _config.Drag;
            AddContinuousForce(dragForce);
        }
        
        void UpdatePosition()
        {
            _velocity = Vector3.ClampMagnitude(_velocity, _config.MaxVelocity);
            _transform.position += _velocity * Time.deltaTime;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _velocity = velocity;
            _velocity = Vector3.ClampMagnitude(_velocity, _config.MaxVelocity);
        }
    }
}
