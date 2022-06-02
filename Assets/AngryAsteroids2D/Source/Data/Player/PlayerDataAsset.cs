using System;
using System.Collections.Generic;
using System.Linq;
using AngryAsteroids2D.Source.Data.Level;
using UnityEngine;

namespace AngryAsteroids2D.Source.Data.Player
{
    [Serializable]
    public struct AsteroidScoreValue
    {
        public AsteroidType AsteroidType;
        public int ScoreValue;
    }
    
    [CreateAssetMenu(menuName = "AngryAsteroids2D/Data/Player Data Asset")]
    public class PlayerDataAsset : ScriptableObject
    {
        public int InitialPlayerLives;
        public int UFOScoreValue = 150;
        public AsteroidScoreValue[] AsteroidScoreValues;

        public Dictionary<AsteroidType, int> GetAsteroidScoreTable()
        {
           return AsteroidScoreValues.ToDictionary(x => x.AsteroidType, x => x.ScoreValue);
        }
    }
}
