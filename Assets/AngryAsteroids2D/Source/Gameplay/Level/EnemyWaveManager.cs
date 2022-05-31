using System.Linq;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Gameplay.Asteroid;
using Random = UnityEngine.Random;

namespace AngryAsteroids2D.Source.Gameplay.Level
{
    public class EnemyWaveManager
    {
        readonly AsteroidManager _asteroidManager;
        readonly WaveData[] _easyWaves;
        readonly WaveData[] _mediumWaves;
        readonly WaveData[] _hardWaves;

        int _completedWaves;
        
        public EnemyWaveManager(AsteroidManager asteroidManager, WaveData[] levelWaves)
        {
            _easyWaves = levelWaves.Where(x => x.WaveType == WaveType.Easy).ToArray();
            _mediumWaves = levelWaves.Where(x => x.WaveType == WaveType.Medium).ToArray();
            _hardWaves = levelWaves.Where(x => x.WaveType == WaveType.Hard).ToArray();

            _asteroidManager = asteroidManager;
        }
        
        public void StartWave()
        {
            var waveData = GetRandomWave();
            var asteroidsData = waveData.Asteroids;
            
            for (var i = 0; i < asteroidsData.Length; i++)
            {
                var spawnData = asteroidsData[i];
                SpawnAsteroid(spawnData);
            }
        }

        void SpawnAsteroid(AsteroidSpawnData asteroidSpawnData)
        {
            var asteroid = _asteroidManager.CreateAsteroid(asteroidSpawnData.AsteroidType, asteroidSpawnData.SpawnPoint);
            asteroid.Body.SetVelocity(asteroidSpawnData.MoveDirection * UnityEngine.Random.Range(0.5f, 4f));
        }
        
        WaveData GetRandomWave()
        {
            if (_completedWaves < 4)
            {
                return _easyWaves[Random.Range(0, _easyWaves.Length)];
            }

            if (_completedWaves < 8)
            {
                return _mediumWaves[Random.Range(0, _mediumWaves.Length)];
            }

            return _hardWaves[Random.Range(0, _hardWaves.Length)];
        }
    }
}
