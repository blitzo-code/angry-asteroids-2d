using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace AngryAsteroids2D.Source.Gameplay.Level
{
    public class ScreenBorderEntityData
    {
        public readonly Transform Transform;
        public readonly PhysicsBody Body;
        public bool IsLocked;

        public ScreenBorderEntityData(Transform transform, PhysicsBody body, bool startLocked = false)
        {
            Transform = transform;
            Body = body;
            IsLocked = startLocked;
        }
    }
    
    public class ScreenBorderPortalSystem : GameSystem<ScreenBorderEntityData>
    {
        readonly Camera _camera;
        readonly Collider2D _safeZone;
        public ScreenBorderPortalSystem(Camera camera, Collider2D safeZone, int maxEntities) : base(maxEntities)
        {
            _camera = camera;
            _safeZone = safeZone;
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
                
                var pos = (Vector2)entityData.Transform.position;
                
                if (entityData.IsLocked && IsOnScreen(pos))
                {
                    SetTeleportLock(entityData, false);
                    continue;
                }
                
                if (!entityData.IsLocked && !IsOnScreen(pos))
                {
                    TeleportToOtherSideOfTheScreen(entityData);
                    SetTeleportLock(entityData, true);
                    continue;
                }
                
                //Safety net if the entity gets too far away of the screen
                if (!IsInsideSafeZone(pos))
                {
                    //Set the movement direction to the center of the screen
                    var screenCenter = (Vector2)_camera.ViewportToWorldPoint(Vector3.one * 0.5f);
                    var currentVelocity = entityData.Body.Velocity;
                    var fixedDirection = (screenCenter - pos).normalized;
                    
                    fixedDirection.x *=  Mathf.Max(1, Mathf.Abs(currentVelocity.x));
                    fixedDirection.y *= Mathf.Max(1, Mathf.Abs(currentVelocity.y));
                    
                    entityData.Body.SetVelocity(fixedDirection);
                }
            }
        }

        bool IsInsideSafeZone(Vector3 pos)
        {
            var bounds = _safeZone.bounds;
            var min = bounds.min;
            var max = bounds.max;

            return pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y;
        }

        void SetTeleportLock(ScreenBorderEntityData entityData, bool isLocked)
        {
            entityData.IsLocked = isLocked;
        }
        
        /*
         * This works because the camera is at the origin
         * So, doing the *=-1 mirrors the position of the object.
         *
         * Notice that we're changing the worldPosition, not the viewport position.
         */
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
 
            entityData.IsLocked = true;
            entityData.Transform.position = newPosition;
        }

        /*
         * Viewport coordinates goes from 0,1 on x and y axis. I used that info to cheaply check
         * if the object is on the viewport
         */
        bool IsOnScreen(Vector3 tPosition)
        {
            var viewportPoint = _camera.WorldToViewportPoint(tPosition);
            var min = 0;
            var max = 1;
            
            return viewportPoint.x >= min && viewportPoint.x <= max && viewportPoint.y >= min && viewportPoint.y <= max;
        }
    }
}
