using System;
using System.Collections.Generic;
using UnityEngine;

namespace AngryAsteroids2D.Source.Data.UFO
{
    [Serializable]
    public struct LinearPath
    {
        public Vector3[] Waypoints;
    } 
    
    [CreateAssetMenu(menuName = "AngryAsteroids2D/Data/UFO Data Asset")]
    public class UFODataAsset : ScriptableObject
    {
        public GameObject UFOPrefab;
        public float ProjectileSpeed = 14f;
        public float MovementSpeed = 1f;
        public LinearPath[] PossiblePaths;
        
        
        public Queue<Vector3> GetRandomPath()
        {
            return new Queue<Vector3>(PossiblePaths[UnityEngine.Random.Range(0, PossiblePaths.Length)].Waypoints);
        }
    }
    
}
