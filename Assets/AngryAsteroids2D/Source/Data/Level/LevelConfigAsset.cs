using System;
using UnityEngine;

namespace AngryAsteroids2D.Source.Data.Level
{
    public enum WaveType
    {
        Easy,
        Medium,
        Hard
    }
    
    public enum AsteroidType
    {
        Undefined = 0,
        Small = 1,
        Medium = 2,
        Big = 3,
        Lava = 4
    }
    
    [Serializable]
    public struct AsteroidSpawnData
    {
        public Vector3 SpawnPoint;
        public Vector3 MoveDirection;
        public AsteroidType AsteroidType;
        
    }

    [Serializable]
    public struct WaveData
    {
        public WaveType WaveType;
        public AsteroidSpawnData[] Asteroids;
    }
    
    [CreateAssetMenu(menuName = "AngryAsteroids2D/Data/Level Config Asset")]
    public class LevelConfigAsset : ScriptableObject
    {
        public WaveData[] LevelWaves;
    }
}
