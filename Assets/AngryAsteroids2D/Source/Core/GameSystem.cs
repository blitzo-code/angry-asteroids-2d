using AngryAsteroids2D.Source.Utils;
using UnityEngine;

namespace AngryAsteroids2D.Source.Core
{
    public abstract class GameSystem<T>
    {
        struct Entity<TEntity>
        {
            public int EntityId;
            public TEntity EntityData;
        }
        
        readonly Entity<T>[] _entities;
        int _entityCount;
        
        protected GameSystem(int maxEntities)
        {
            _entities = new Entity<T>[maxEntities];
        }

        //O(1)
        public void AddEntity(int entityId, T entityData)
        {
            var insertionIndex = Mathf.Min(_entityCount, _entities.Length - 1);

            var entity = _entities[insertionIndex];
            
            if (entity.EntityId == 0) 
            {
                _entities[insertionIndex].EntityId = entityId;
                _entities[insertionIndex].EntityData = entityData;
                _entityCount++;
                return;
            }
            
            LogUtils.LogCouldNotInsertEntityError(entityId.ToString() , GetType().Name);
        }

        //O(n)
        public void RemoveEntity(int entityId)
        {
            var index = GetIndexById(entityId);
            if (index < 0)
            {
                LogUtils.LogCouldNotFindEntityError(entityId.ToString(), GetType().Name);
                return;
            }
            
            _entities[index] = default;
            
            //If it's not the last element, swap with the last element
            if (index < _entityCount - 1)
            {
                (_entities[index], _entities[_entityCount - 1]) = (_entities[_entityCount - 1], _entities[index]);
            }
            _entityCount--;
        }

        protected T GetEntityData(int entityIndex)
        {
            return _entities[entityIndex].EntityData;
        }
        
        protected int GetEntityCount()
        {
            return _entityCount;
        }
        
        
        int GetIndexById(int entityId)
        {
            for (var i = 0; i < _entityCount; i++)
            {
                if (_entities[i].EntityId == entityId)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public abstract void Tick();
    }
}
