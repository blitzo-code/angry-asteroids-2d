using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.UFO
{
    public class PathFollowEntity
    {
        public readonly Transform Transform;
        public readonly float Speed;
        public readonly Queue<Vector3> Waypoints;
        public readonly Action<int> OnPathCompleted;

        public PathFollowEntity(Transform transform, float speed, Queue<Vector3> waypoints, Action<int> onPathCompleted)
        {
            Transform = transform;
            Speed = speed;
            Waypoints = waypoints;
            OnPathCompleted = onPathCompleted;
        }
    }
    
    public class PathFollowSystem : GameSystem<PathFollowEntity>
    {
        const float proximityCheckThreshold = 0.0001f;
        
        public PathFollowSystem(int maxEntities) : base(maxEntities)
        {
        }

        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                FollowPath(GetEntityData(i));
            }
        }

        void FollowPath(PathFollowEntity entity)
        {
            var entityTransform = entity.Transform;
            var nextWaypoint = entity.Waypoints.Peek();
            var currentPosition =entityTransform.position;
            
            if ((currentPosition - nextWaypoint).sqrMagnitude <= proximityCheckThreshold)
            {
                entity.Waypoints.Dequeue();
                if (entity.Waypoints.Count == 0)
                {
                    entity.OnPathCompleted?.Invoke(entityTransform.gameObject.GetInstanceID());
                    return;
                }
            }

            var step = entity.Speed * Time.deltaTime;
            entityTransform.position = Vector3.MoveTowards(entityTransform.position, nextWaypoint, step);
        }
    }
}
