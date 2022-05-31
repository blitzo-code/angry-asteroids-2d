using UnityEngine;

namespace AngryAsteroids2D.Source.Data.Physics
{
    [CreateAssetMenu(menuName = "AngryAsteroids2D/Data/Physics Config")]
    public class PhysicsConfig : ScriptableObject
    {
        public float Drag;
        public float MaxVelocity;
    }
}
