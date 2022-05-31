using AngryAsteroids2D.Source.Data.Level;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

namespace AngryAsteroids2D.Source.Gameplay.Asteroid
{
    public class AsteroidDestroyActionResolver
    {
        readonly AsteroidManager _asteroidManager;
        
        public AsteroidDestroyActionResolver(AsteroidManager asteroidManager)
        {
            _asteroidManager = asteroidManager;
        }
        
        public void TriggerDestructionAction(Asteroid asteroid)
        {
            switch (asteroid.Type)
            {
                case AsteroidType.Big:
                    OnBigAsteroidDestroyed(asteroid);
                    break;
                case AsteroidType.Medium:
                    OnMediumAsteroidDestroyed(asteroid);
                    break;
                case AsteroidType.Small:
                    OnSmallAsteroidDestroyed(asteroid);
                    break;
            }
        }

        void OnBigAsteroidDestroyed(Asteroid asteroid)
        {
            for (var i = 0; i < 2; i++)
            {
                var spawnPosition = asteroid.GameObject.transform.position +  Vector3.Scale(Random.insideUnitCircle,asteroid.Collider.bounds.extents);
                var createdAsteroid = _asteroidManager.CreateAsteroid(AsteroidType.Medium, spawnPosition);
                
                createdAsteroid.Body.SetVelocity(Random.insideUnitCircle * 6f);
            }
        }
        
        void OnMediumAsteroidDestroyed(Asteroid asteroid)
        {
            for (var i = 0; i < 2; i++)
            {
                var spawnPosition = asteroid.GameObject.transform.position +  Vector3.Scale(Random.insideUnitCircle,asteroid.Collider.bounds.extents);
                var createdAsteroid = _asteroidManager.CreateAsteroid(AsteroidType.Small, spawnPosition);
                
                createdAsteroid.Body.SetVelocity(Random.insideUnitCircle * 12f);
            }
        }

        void OnSmallAsteroidDestroyed(Asteroid asteroid)
        {
        }
    }
}
