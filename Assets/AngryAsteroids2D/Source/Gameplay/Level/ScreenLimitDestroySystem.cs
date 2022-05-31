using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Level
{
    public class ScreenLimitWatchEntity
    {
        public readonly Transform Transform;
        public readonly Action<int> DestroyCallback; //int => GameObjectId

        public ScreenLimitWatchEntity(Transform transform, Action<int> destroyCallback)
        {
            Transform = transform;
            DestroyCallback = destroyCallback;
        }
    }

    public class ScreenLimitDestroySystem : GameSystem<ScreenLimitWatchEntity>
    {
        Camera _camera;
        
        public ScreenLimitDestroySystem(Camera camera, int maxEntities) : base(maxEntities)
        {
            _camera = camera;
        }

        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                var entity = GetEntityData(i);
                if (IsOutOfTheScreen(entity.Transform.position))
                {
                    entity.DestroyCallback(entity.Transform.gameObject.GetInstanceID());
                }
            }
        }

        bool IsOutOfTheScreen(Vector3 position)
        {
            var viewportPosition = _camera.WorldToViewportPoint(position);
            return viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1;
        }
    } 
}
  
