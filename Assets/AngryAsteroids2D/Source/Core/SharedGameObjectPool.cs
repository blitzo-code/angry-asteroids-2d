using AngryAsteroids2D.Source.Utils;
using UnityEngine;

namespace AngryAsteroids2D.Source.Core
{
    internal struct PoolReference<TIdentifierType>
    {
        public TIdentifierType Id;
        public GameObject GameObject;
    }
    
    public class SharedGameObjectPool<TIdentifierType> 
    {
        readonly PoolReference<TIdentifierType>[] _pool;
        
        int _poolCount;
        int _activeObjects;
        
        public SharedGameObjectPool(int poolCapacity)
        {
            _pool = new PoolReference<TIdentifierType>[poolCapacity];
        }

        //O(n)
        public GameObject GetOrCreate(TIdentifierType referenceId, GameObject gameObject)
        {
            //Search for already instantiated, but inactive game objects
            for (var i = 0; i < _poolCount; i++)
            {
                if (_pool[i].Id.Equals(referenceId) && !_pool[i].GameObject.activeSelf)
                {
                    return _pool[i].GameObject;
                }
            }
            
            //Check if the pool is already full
            if (_poolCount == _pool.Length)
            {
                LogUtils.LogPoolIsFullError();
                return null;
            }
            
            //Create one if there is no object available for getting
            var startingIndex = Mathf.Max(0, _poolCount);
            for (var i = startingIndex; i < _pool.Length; i++)
            {
                _pool[i].Id = referenceId;
                _pool[i].GameObject = GameObject.Instantiate(gameObject, Vector3.one * 1000, gameObject.transform.rotation);
                _pool[i].GameObject.SetActive(false);
                _poolCount++;

                return _pool[i].GameObject;
            }

            return null;
        }

        //O(n)
        public void Deactivate(int gameObjectId)
        {
            var foundObject = FindGameObjectById(gameObjectId);
            if (!foundObject)
            {
                LogUtils.CouldNotFindObjectOnPoolError(gameObjectId.ToString());
            }
            foundObject.SetActive(false);
        }

        GameObject FindGameObjectById(int gameObjectId)
        {
            for (var i = 0; i < _poolCount; i++)
            {
                if (_pool[i].GameObject.GetInstanceID() == gameObjectId)
                {
                    return _pool[i].GameObject;
                }
            }
            return null;
        }
        
    }
}
