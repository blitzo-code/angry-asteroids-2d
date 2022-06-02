using System;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Gameplay.Spaceship;
using AngryAsteroids2D.Source.Input;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Player
{
    public class PlayerManager
    {
        SpaceshipCore _spaceshipPrefab;
        SpaceshipCore _spaceshipCore;
        Vector3 _spawnPosition;

        public event Action PlayerKilled;

        public Transform PlayerTransform => _spaceshipCore.transform;
        
        public void Initialize(Camera camera, SpaceshipCore spaceshipPrefab)
        {
            _spaceshipPrefab = spaceshipPrefab;
            _spawnPosition = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, -camera.depth));
            
            _spaceshipCore = GameObject.Instantiate(_spaceshipPrefab, _spawnPosition, _spaceshipPrefab.transform.rotation);
            _spaceshipCore.DoFirstTimeSetup(InputDeviceType.Keyboard);
            _spaceshipCore.Enable();
        }

        public void KillPlayer()
        {
            _spaceshipCore.Disable();    
            PlayerKilled?.Invoke();
        }
        
        public void RespawnPlayer()
        {
            _spaceshipCore.Enable();
            _spaceshipCore.transform.position = _spawnPosition;
            _spaceshipCore.transform.rotation = _spaceshipPrefab.transform.rotation;
            
            GameManager.GetInstance().GameSystems.PlayerRespawnSystem.RemoveEntity(_spaceshipCore.gameObject.GetInstanceID());
        }
        
        
    }
}
