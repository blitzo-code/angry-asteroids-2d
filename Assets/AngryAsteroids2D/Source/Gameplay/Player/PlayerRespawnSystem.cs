using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Player
{
    public class PlayerRespawnEntity
    {
        public readonly Vector3 SpawnPosition;
        public readonly LayerMask LayerMask;
        public readonly float RespawnTime;

        public float Timer;
        
        public PlayerRespawnEntity(Vector3 spawnPosition, LayerMask layerMask, float respawnTime)
        {
            SpawnPosition = spawnPosition;
            LayerMask = layerMask;
            RespawnTime = respawnTime;
        }
    }
    
    public class PlayerRespawnSystem : GameSystem<PlayerRespawnEntity>
    {
        readonly PlayerManager _playerManager;
        readonly float _checkRadius;
        
        public PlayerRespawnSystem(int maxEntities) : base(maxEntities)
        {
            _playerManager = GameManager.GetInstance().GameManagers.PlayerManager;
            _checkRadius = 6f;
        }

        public override void Tick()
        {
            for (var i = 0; i < GetEntityCount(); i++)
            {
                CheckForRespawn(GetEntityData(i));
            }
        }

        void CheckForRespawn(PlayerRespawnEntity entity)
        {
            entity.Timer += Time.deltaTime;
            if (entity.Timer > entity.RespawnTime)
            {
                var hit = UnityEngine.Physics2D.OverlapCircle(entity.SpawnPosition, _checkRadius, entity.LayerMask);
                if (hit == null)
                {
                    _playerManager.RespawnPlayer();
                }
            }
        }
    }
}
