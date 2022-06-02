using System;
using System.Collections.Generic;
using System.Linq;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Gameplay.Asteroid;
using AngryAsteroids2D.Source.Gameplay.UFO;
using Random = UnityEngine.Random;

namespace AngryAsteroids2D.Source.Gameplay.Level
{
    public class EnemyWaveManager
    {
        readonly WaveData[] _easyWaves;
        readonly WaveData[] _mediumWaves;

        WaveData _lastChosenWave;
        AsteroidManager _asteroidManager;
        UFOManager _ufoManager;
        int _completedWaves;

        public event Action<int> WaveChanged;
        
        public EnemyWaveManager(List<WaveData> levelWaves)
        {
            _easyWaves = levelWaves.Where(x => x.WaveType == WaveType.Easy).ToArray();
            _mediumWaves = levelWaves.Where(x => x.WaveType == WaveType.Medium).ToArray();
        }

        ~EnemyWaveManager()
        {
            if (_asteroidManager != null)
            {
                _asteroidManager.AsteroidDestroyed -= OnAsteroidDestroyed;
            }
        }

        public void Initialize()
        {
            _asteroidManager = GameManager.GetInstance().GameManagers.AsteroidManager;
            _ufoManager = GameManager.GetInstance().GameManagers.UFOManager;
            _asteroidManager.AsteroidDestroyed += OnAsteroidDestroyed;
            
            StartWave();
        }
        
        void StartWave()
        {
            _ufoManager.SpawnUFO();
            
            var waveData = GetNextWave();
            
            var asteroidsData = waveData.Asteroids;
            
            for (var i = 0; i < asteroidsData.Count; i++)
            {
                var spawnData = asteroidsData[i];
                SpawnAsteroid(spawnData);
            }
            
            WaveChanged?.Invoke(_completedWaves);
        }

        void SpawnAsteroid(AsteroidSpawnData asteroidSpawnData)
        {
            var asteroid = _asteroidManager.CreateAsteroid(asteroidSpawnData.AsteroidType, asteroidSpawnData.SpawnPoint);
            asteroid.Body.SetVelocity(asteroidSpawnData.MoveDirection * GetAsteroidSpawnSpeed(asteroid.Type));
        }

        float GetAsteroidSpawnSpeed(AsteroidType asteroidType)
        {
            switch (asteroidType)
            {
                case AsteroidType.Small:
                    return Random.Range(8, 10);
                case AsteroidType.Medium:
                    return Random.Range(5, 8);
                case AsteroidType.Big:
                    return Random.Range(3, 6);
                default:
                    return Random.Range(2, 6);
            }
        }
        
        void OnAsteroidDestroyed(AsteroidType asteroidType)
        {
            if (_asteroidManager.AsteroidCount == 0)
            {
                _completedWaves++;
                StartWave();
            }
        }

        WaveData GetNextWave()
        {
            var waveData = GetRandomWave();
            while (waveData.Asteroids == _lastChosenWave.Asteroids)
            {
                waveData = GetRandomWave();
            }
            _lastChosenWave = waveData;
            return waveData;
        }
        
        WaveData GetRandomWave()
        {
            if (_completedWaves < 2)
            {
                return _easyWaves[Random.Range(0, _easyWaves.Length)];
            }

            return _mediumWaves[Random.Range(0, _mediumWaves.Length)];
            //No hard waves this time;
        }
    }
}
