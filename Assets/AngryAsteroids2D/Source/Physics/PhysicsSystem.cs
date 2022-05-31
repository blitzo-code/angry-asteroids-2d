using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Utils;
using UnityEngine;

namespace AngryAsteroids2D.Source.Physics
{
    public class PhysicsEntity
    {
        public readonly GameObject GameObject;
        public readonly PhysicsBody Body;

        public PhysicsEntity(GameObject gameObject, PhysicsBody body)
        {
            GameObject = gameObject;
            Body = body;
        }
    }
    
    public class PhysicsSystem : GameSystem<PhysicsEntity>
    {
        public PhysicsSystem(int maxEntities) : base(maxEntities)
        {
        }

        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                var entityData = GetEntityData(i);
                if (entityData.GameObject.activeSelf)
                {
                    entityData.Body.RunPhysicsLoop();
                }
            }
        }
    }
}
