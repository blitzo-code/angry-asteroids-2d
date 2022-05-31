using System.Collections.Generic;
using System.Numerics;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Physics;
using AngryAsteroids2D.Source.Utils;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace AngryAsteroids2D.Source.Gameplay.Level
{
    public class ScreenBorderEntityData
    {
        public readonly Transform Transform;
        public readonly PhysicsBody Body;
        public readonly Collider2D Collider;
        public bool IsLocked;

        public ScreenBorderEntityData(Transform transform, PhysicsBody body, Collider2D collider, bool startLocked = false)
        {
            Transform = transform;
            Body = body;
            Collider = collider;
            IsLocked = startLocked;
        }
    }
    
    public class ScreenBorderPortalSystem : GameSystem<ScreenBorderEntityData>
    {
        static Vector2[] CORNERS =
        {
            Vector2.zero,
            Vector2.right,
            Vector2.up,
            Vector2.one
        };
        
        readonly Camera _camera;
        readonly Vector2 _screenCenterWorldPosition;
        
        public ScreenBorderPortalSystem(Camera camera, int maxEntities) : base(maxEntities)
        {
            _camera = camera;
            _screenCenterWorldPosition = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
        }
 
        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                var entityData = GetEntityData(i);
                if (!entityData.Transform.gameObject.activeSelf)
                {
                    continue;
                }

                if (entityData.IsLocked)
                {
                    Debug.DrawLine(Vector3.zero, entityData.Transform.position);   
                }
                
                var pos = entityData.Transform.position;
                if (entityData.IsLocked && IsOnScreen(pos, entityData.Body.Velocity, entityData.Collider))
                {
                    SetTeleportLock(entityData, false);
                    continue;
                }
                
                if (!entityData.IsLocked && !IsOnScreen(pos, entityData.Body.Velocity, entityData.Collider))
                {
                    TeleportToOtherSideOfTheScreen(entityData);
                }
            }
        }

        void SetTeleportLock(ScreenBorderEntityData entityData, bool isLocked)
        {
            entityData.IsLocked = isLocked;
        }
        
        void TeleportToOtherSideOfTheScreen(ScreenBorderEntityData entityData)
        {
            Vector2 position = entityData.Transform.position;
            
            Vector2 viewportPoint = _camera.WorldToViewportPoint(position);
            Vector2 newPosition = position;

            if (viewportPoint.x < 0 || viewportPoint.x > 1)
            {
                newPosition.x *= -1;
            }

            if (viewportPoint.y < 0 || viewportPoint.y > 1)
            {
                newPosition.y *= -1;
            }

            // var centerDirection = (_screenCenterWorldPosition - newPosition).normalized;
            // var dotProduct = Vector2.Dot(centerDirection, entity.Body.Velocity);
            // if (IsNearCorner(newPosition, entity.Body.Velocity, entity.Collider) && dotProduct <= 0)
            // {
            //     newPosition.x *= -1;
            // }
            //
            // newPosition.x = Mathf.Clamp(newPosition.x, )
            
            entityData.IsLocked = true;
            entityData.Transform.position = newPosition;
        }

        bool IsNearCorner(Vector3 worldPosition, Vector3 velocity, Collider2D collider)
        {
            Vector2 closestCornerViewportPosition = GetClosestCorner(worldPosition);
            Vector2 closestCornerWorldPosition = _camera.ViewportToWorldPoint(closestCornerViewportPosition);
            
            Vector2 checkPoint = worldPosition + GetToleranceVector(worldPosition, velocity, collider);
            var sqrDistance = (closestCornerWorldPosition - checkPoint).sqrMagnitude;
            
            return sqrDistance < (collider.bounds.size + collider.bounds.extents * 0.5f).sqrMagnitude;
        }

        Vector2 GetClosestCorner(Vector2 worldPosition)
        {
            Vector2 viewportPosition = _camera.WorldToViewportPoint(worldPosition);
            
            var minSqrDistance = float.MaxValue;
            var corner = Vector3.zero;
            
            for (var i = 0; i < CORNERS.Length; i++)
            {
                var distance = (viewportPosition - CORNERS[i]).sqrMagnitude;
                if (distance < minSqrDistance)
                {
                    minSqrDistance = distance;
                    corner = CORNERS[i];
                }
            }
            return corner;
        }
        
        /*
         * Viewport coordinates goes from 0,1 on x and y axis. I used that info to cheaply check
         * if the object is on the viewport
         *
         * Also, I've sum the velocity reversed vector to the position in order to add a little bit of tolerance to the check.
         * This was decided after some gameplay tests and this tolerance gives a smoother sensation when teleporting
         * entities
         *
         * DotProduct
         * 1 = Same direction
         * 0 = Perpendicular
         * -1 = Opposite Direction
         */
        
        bool IsOnScreen(Vector3 tPosition, Vector3 velocity, Collider2D collider)
        {
            var toleranceVector = GetToleranceVector(tPosition, velocity, collider);
            
            var viewportPoint = _camera.WorldToViewportPoint(tPosition /*+ toleranceVector */);
            var min = 0;
            var max = 1;
            
            return viewportPoint.x >= min && viewportPoint.x <= max && viewportPoint.y >= min && viewportPoint.y <= max;
        }

        Vector3 GetToleranceVector(Vector2 tPosition, Vector2 velocity, Collider2D collider)
        {
            var size = collider.bounds.size;
            var normalizedVelocity = velocity.normalized;
            
            var screenCenterPoint  = _screenCenterWorldPosition;
            var screenCenterVector = (screenCenterPoint - tPosition).normalized;

            var alignment = Mathf.Min(1, Mathf.Sign(Vector2.Dot(screenCenterVector, normalizedVelocity)));
            
            var checkDirection = (normalizedVelocity * alignment).normalized;
            return new Vector3(size.x * checkDirection.x, size.y * checkDirection.y);
        }
    }
}
