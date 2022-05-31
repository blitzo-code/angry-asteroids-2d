using System;
using UnityEngine;

namespace AngryAsteroids2D.Source.Input
{
    public enum InputDeviceType
    {
        None = 0,
        Keyboard = 1
    };
    
    public class InputManager
    {
        readonly IInputDevice _inputDevice;
        
        InputData _inputData;
        
        public event Action Throttle = delegate {  };
        public event Action<float> RotateShip = delegate {  };
        public event Action FireAction = delegate {  };
        
        public InputManager(InputDeviceType inputDeviceType)
        {
            _inputDevice = LoadInputDevice(inputDeviceType);
        }

        IInputDevice LoadInputDevice(InputDeviceType inputDeviceType)
        {
            switch (inputDeviceType)
            {
                case InputDeviceType.Keyboard:
                    return new KeyboardDevice();
                default:
                    Debug.LogError($"This device type {inputDeviceType} is not linked to any constructor");
                    return null;
            }
        }
        
        public void ReadInput()
        {
            _inputData = _inputDevice.ReadInput();

            if (_inputData.ThrottleButton)
            {
                Throttle();
            }

            if (_inputData.TurnDirection != 0)
            {
                RotateShip(_inputData.TurnDirection);
            }

            if (_inputData.FireButton)
            {
                FireAction();
            }
        }
    }
}
