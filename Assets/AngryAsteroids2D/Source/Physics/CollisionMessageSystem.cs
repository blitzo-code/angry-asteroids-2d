using System;
using AngryAsteroids2D.Source.Core;
using UnityEngine;

namespace AngryAsteroids2D.Source.Physics
{
    public class CollisionListenerEntity
    {
        public readonly Collider2D Collider;
        public readonly ContactFilter2D ContactFilter2D;
        public readonly Action<Collider2D, Collider2D> CollisionCallback;
        
        public CollisionListenerEntity(Collider2D collider, ContactFilter2D contactFilter, Action<Collider2D, Collider2D> collisionCallback)
        {
            Collider = collider;
            ContactFilter2D = contactFilter;
            CollisionCallback = collisionCallback;
        }
    }

    public class CollisionMessageSystem : GameSystem<CollisionListenerEntity>
    {
        readonly Collider2D[] _collisionBuffer;
        
        public CollisionMessageSystem(int collisionBufferSize, int maxEntities) : base(maxEntities)
        {
            _collisionBuffer = new Collider2D[collisionBufferSize];
        }

        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                RunCollisionCheck(GetEntityData(i));
            }
        }

        void RunCollisionCheck(CollisionListenerEntity entityData)
        {
            var hits = entityData.Collider.OverlapCollider(entityData.ContactFilter2D, _collisionBuffer);
            if (hits > 0)
            {
                for (var i = 0; i < hits; i++)
                {
                    entityData.CollisionCallback(entityData.Collider, _collisionBuffer[i]);
                }
            }
        }
    }
}
