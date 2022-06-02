using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Data.Player;
using AngryAsteroids2D.Source.Gameplay.Asteroid;
using AngryAsteroids2D.Source.Gameplay.UFO;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Player
{
    public class PlayerDataManager
    {
        readonly PlayerDataAsset _playerDataAsset;
        readonly Vector3 _spawnPosition;
        readonly Dictionary<AsteroidType, int> _asteroidScoreTable;
        
        AsteroidManager _asteroidManager;
        PlayerManager _playerManager;
        UFOManager _ufoManager;
        Action _gameOverCallback;
        
        int _playerLives;
        int _playerScore;

        public event Action<int> LivesChanged;
        public event Action<int> ScoreChanged;
        
        public PlayerDataManager(Camera camera, PlayerDataAsset playerDataAsset)
        {
            _playerDataAsset = playerDataAsset;
            _asteroidScoreTable = _playerDataAsset.GetAsteroidScoreTable();
            _spawnPosition = camera.ViewportToWorldPoint(Vector3.one * 0.5f);
        }

        ~PlayerDataManager()
        {
            if (_asteroidManager != null)
            {
                _asteroidManager.AsteroidDestroyed -= OnAsteroidDestroyed;
            }

            if (_playerManager != null)
            {
                _playerManager.PlayerKilled += OnPlayerKilled;
            }

            if (_ufoManager != null)
            {
                _ufoManager.UFODestroyed -= OnUFODestroyed;
            }
        }

        public void Initialize(Action gameOverCallback)
        {
            _gameOverCallback = gameOverCallback;
            
            _asteroidManager = GameManager.GetInstance().GameManagers.AsteroidManager;
            _playerManager = GameManager.GetInstance().GameManagers.PlayerManager;
            _ufoManager = GameManager.GetInstance().GameManagers.UFOManager;
            
            _asteroidManager.AsteroidDestroyed += OnAsteroidDestroyed;
            _ufoManager.UFODestroyed += OnUFODestroyed;
            _playerManager.PlayerKilled += OnPlayerKilled;
            
            ResetPlayerData();
        }
        
        void ResetPlayerData()
        {
            _playerLives = _playerDataAsset.InitialPlayerLives;
            _playerScore = 0;
            
            LivesChanged?.Invoke(_playerLives);
            ScoreChanged?.Invoke(_playerScore);
        }
        
        void OnAsteroidDestroyed(AsteroidType asteroidType)
        {
            AddToScore(_asteroidScoreTable[asteroidType]);
        }
        
        void OnUFODestroyed()
        {
            AddToScore(_playerDataAsset.UFOScoreValue);
        }
        
        void OnPlayerKilled()
        {
            RemoveLife();
            
            if (_playerLives > 0)
            {
                TriggerPlayerRespawn();
                return;
            }
            
            TriggerGameOver();
        }

        void TriggerPlayerRespawn()
        {
            var respawnEntity = new PlayerRespawnEntity(_spawnPosition, LayerMask.GetMask("Asteroids", "UFO"), 3f);
            GameManager.GetInstance().GameSystems.PlayerRespawnSystem.AddEntity(_playerManager.PlayerTransform.gameObject.GetInstanceID(), respawnEntity);
        }

        void TriggerGameOver()
        {
            _gameOverCallback();
        }
        
        void RemoveLife()
        {
            _playerLives--;
            LivesChanged?.Invoke(_playerLives);
        }

        void AddToScore(int amount)
        {
            _playerScore += amount;
            ScoreChanged?.Invoke(_playerScore);
        }
    }
}
