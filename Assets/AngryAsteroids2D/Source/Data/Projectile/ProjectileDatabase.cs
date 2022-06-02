using System;
using System.Collections.Generic;
using AngryAsteroids2D.Source.Data.Physics;
using UnityEngine;

public enum ProjectileType
{
    Undefined = 0,
    MissileProjectile = 1
} 

[Serializable]
public struct ProjectileDataAsset
{
    public ProjectileType ProjectileType;
    public GameObject Prefab;
    public PhysicsConfig PhysicsConfig;
}

[CreateAssetMenu(menuName = "AngryAsteroids2D/Data/Projectile Database")]
public class ProjectileDatabase : ScriptableObject
{
    public ProjectileDataAsset[] Data;

    public Dictionary<ProjectileType, ProjectileDataAsset> GetProjectileDataDictionary()
    {
        var dictionary = new Dictionary<ProjectileType, ProjectileDataAsset>();
        for (var i = 0; i < Data.Length; i++)
        {
            var reference = Data[i];
            if (reference.ProjectileType == ProjectileType.Undefined)
            {
                continue;
            }
            dictionary.Add(reference.ProjectileType, reference);
        }
        return dictionary;
    }
}
