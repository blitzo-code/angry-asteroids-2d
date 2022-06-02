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
        
        InputManager _inputManager;
        SpaceshipController _spaceshipController;
        PhysicsBody _body;
        
        bool _isEnabled;
        
        public void DoFirstTimeSetup(InputDeviceType inputDeviceType)
        {
            CreateSpaceshipComponents(inputDeviceType);
            ConnectInput();
        }
        
        public void Enable()
        {
            LinkSystems();
            
            _body.SetVelocity(Vector3.zero);
            gameObject.SetActive(true);
            
            _isEnabled = true;
        }

        public void Disable()
        {
            UnlinkSystems();
            
            gameObject.SetActive(false);

            _isEnabled = false;
        }
        
        void LinkSystems()
        {
            var entityId = GetInstanceID();
            
            var screenBorderEntity = new ScreenBorderEntityData(transform, _body, collider);
            GameManager.GetInstance().GameSystems.ScreenBorderPortalSystem.AddEntity(entityId, screenBorderEntity);

            var physicsEntity = new PhysicsEntity(gameObject,_body);
            GameManager.GetInstance().GameSystems.PhysicsSystem.AddEntity(entityId, physicsEntity);
        }
        
        void UnlinkSystems()
        {
            var entityId = GetInstanceID();
            
            GameManager.GetInstance().GameSystems.ScreenBorderPortalSystem.RemoveEntity(entityId);
            GameManager.GetInstance().GameSystems.PhysicsSystem.RemoveEntity(entityId);
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

        void Update()
        {
            if (!_isEnabled)
            {
                return;
            }
            _inputManager.ReadInput();
        }
    }
}
