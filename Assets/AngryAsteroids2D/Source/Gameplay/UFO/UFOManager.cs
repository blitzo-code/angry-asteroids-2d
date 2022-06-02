using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.UFO;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.UFO
{
    public class UFO
    {
        public readonly Transform Transform;
        public readonly Collider2D Collider;

        public UFO(GameObject gameObject)
        {
            Transform = gameObject.transform;
            Collider = gameObject.GetComponent<Collider2D>();
        }
    }
    
    public class UFOManager
    {
        static readonly float[] timeBetweenShotsPool =
        {
            2.5f,
            3f,
            3.5f,
            4f
        };
        
        readonly UFODataAsset _ufoDataAsset;
        readonly ContactFilter2D _collisionFilter;
        
        UFO _ufo; 
        Transform _target;

        public event Action UFODestroyed;
        
        public UFOManager(UFODataAsset ufoDataAsset)
        {
            _ufoDataAsset = ufoDataAsset;
            _collisionFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Player")
            };
        }

        public void SpawnUFO()
        {
            if (_ufo == null)
            {
                var prefab = GameObject.Instantiate(_ufoDataAsset.UFOPrefab, new Vector3(1000f, 1000f), _ufoDataAsset.UFOPrefab.transform.rotation);
                _ufo = new UFO(prefab);
            }
            
            var timeBetweenShots = timeBetweenShotsPool[UnityEngine.Random.Range(0, timeBetweenShotsPool.Length)];
            var path = _ufoDataAsset.GetRandomPath();
            
            _ufo.Transform.position = path.Peek();
            _ufo.Transform.gameObject.SetActive(true);
            
            ConnectUFOSystems(path, timeBetweenShots);
        }

        void ConnectUFOSystems(Queue<Vector3> path, float timeBetweenShots)
        {
            var entityId = _ufo.Transform.gameObject.GetInstanceID();
            
            var pathFollowEntity = new PathFollowEntity(_ufo.Transform, _ufoDataAsset.MovementSpeed, path, KillUFO);
            GameManager.GetInstance().GameSystems.PathFollowSystem.AddEntity(entityId, pathFollowEntity);
            
            var collisionEntity = new CollisionListenerEntity(_ufo.Collider,_collisionFilter, OnCollidedWithPlayer);           
            GameManager.GetInstance().GameSystems.CollisionMessageSystem.AddEntity(entityId, collisionEntity);

            var targetTransform = GameManager.GetInstance().GameManagers.PlayerManager.PlayerTransform;
            var targetLockEntity = new UFOTargetEntity(_ufo.Transform, targetTransform, _ufoDataAsset.ProjectileSpeed, timeBetweenShots);
            GameManager.GetInstance().GameSystems.UFOTargetSystem.AddEntity(entityId, targetLockEntity);
        }

        void DisposeUFOSystems()
        {
            var entityId = _ufo.Transform.gameObject.GetInstanceID();
            
            GameManager.GetInstance().GameSystems.PathFollowSystem.RemoveEntity(entityId);
            GameManager.GetInstance().GameSystems.CollisionMessageSystem.RemoveEntity(entityId);
            GameManager.GetInstance().GameSystems.UFOTargetSystem.RemoveEntity(entityId);
        }
        
        void OnCollidedWithPlayer(Collider2D self, Collider2D other)
        {
            GameManager.GetInstance().GameManagers.PlayerManager.KillPlayer();
            GameManager.GetInstance().GameManagers.ProjectileManager.DestroyProjectile(self.gameObject.GetInstanceID());
        }
        
        public void KillUFO(int entityId)
        {
            _ufo.Transform.gameObject.SetActive(false);
            
            DisposeUFOSystems();
            
            UFODestroyed?.Invoke();
        }
    }
}
