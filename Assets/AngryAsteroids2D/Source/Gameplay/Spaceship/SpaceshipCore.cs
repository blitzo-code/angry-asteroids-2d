using System;
using AngryAsteroids2D.Source.Core;
using AngryAsteroids2D.Source.Data.Physics;
using AngryAsteroids2D.Source.Gameplay.Level;
using AngryAsteroids2D.Source.Input;
using AngryAsteroids2D.Source.Physics;
using UnityEngine;

namespace AngryAsteroids2D.Source.Gameplay.Spaceship
{
    public class SpaceshipCore : MonoBehaviour
    {
        [SerializeField] SpaceshipConfig spaceshipConfig;
        [SerializeField] PhysicsConfig physicsConfig;
        [SerializeField] Collider2D collider;
        [SerializeField] ContactFilter2D contactFilter2D;
        
        InputManager _inputManager;
        SpaceshipController _spaceshipController;
        PhysicsBody _body;
        
        bool _isInitialized;

        public PhysicsBody Body => _body;
        public Collider2D Collider => collider;
        
        public void Initialize(InputDeviceType inputDeviceType)
        {
            CreateSpaceshipComponents(inputDeviceType);
            ConnectInput();
            LinkSystems();
            
            _isInitialized = true;
        }

        void LinkSystems()
        {
            var entityId = GetInstanceID();
            
            var screenBorderEntity = new ScreenBorderEntityData(transform, _body, collider);
            GameManager.GetInstance().GameSystems.ScreenBorderPortalSystem.AddEntity(entityId, screenBorderEntity);

            var physicsEntity = new PhysicsEntity(gameObject,_body);
            GameManager.GetInstance().GameSystems.PhysicsSystem.AddEntity(entityId, physicsEntity);

            var collisionListenerEntity =
                new CollisionListenerEntity(collider, contactFilter2D, OnAsteroidHitSpaceship);
            GameManager.GetInstance().GameSystems.CollisionMessageSystem.AddEntity(entityId, collisionListenerEntity);
        }
        
        void UnlinkSystems()
        {
            var entityId = GetInstanceID();
            
            GameManager.GetInstance().GameSystems.ScreenBorderPortalSystem.RemoveEntity(entityId);
            GameManager.GetInstance().GameSystems.PhysicsSystem.RemoveEntity(entityId);
            GameManager.GetInstance().GameSystems.CollisionMessageSystem.RemoveEntity(entityId);
        }
        
        void CreateSpaceshipComponents(InputDeviceType inputDeviceType)
        {
            var t = transform;
            
            _body = new PhysicsBody(t, physicsConfig);
            _spaceshipController = new SpaceshipController(t, _body, spaceshipConfig);
            _inputManager = new InputManager(inputDeviceType);
        }
        
        void ConnectInput()
        {
            _inputManager.Throttle += _spaceshipController.Accelerate;
            _inputManager.RotateShip += _spaceshipController.TurnBody;
            _inputManager.FireAction += _spaceshipController.FireAction;
        }

        void OnAsteroidHitSpaceship(Collider2D self, Collider2D asteroid)
        {
            UnlinkSystems();
            Destroy(gameObject);
        }
        
        void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            _inputManager.ReadInput();
        }
    }
}
