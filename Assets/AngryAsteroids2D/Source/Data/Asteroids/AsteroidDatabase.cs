using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Data.Level;
using AngryAsteroids2D.Source.Data.Physics;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Data.Asteroids
{
    [Serializable]
    public struct AsteroidDataAsset
    {
        public AsteroidType AsteroidType;
        public GameObject Prefab;
        public PhysicsConfig PhysicsConfig;
        public GameObject DestructionVFX;
    }
    
    
    [CreateAssetMenu(menuName = "AngryAsteroids2D/Data/Asteroid Prefab Database")]
    public class AsteroidDatabase : ScriptableObject
    {
        public AsteroidDataAsset[] Data;

        public Dictionary<AsteroidType, AsteroidDataAsset> GetAsteroidDataDictionary()
        {
            var dictionary = new Dictionary<AsteroidType, AsteroidDataAsset>();
            for (var i = 0; i < Data.Length; i++)
            {
                var reference = Data[i];
                if (reference.AsteroidType == AsteroidType.Undefined)
                {
                    continue;
                }
                dictionary.Add(reference.AsteroidType, reference);
            }
            return dictionary;
        }
    }
}
