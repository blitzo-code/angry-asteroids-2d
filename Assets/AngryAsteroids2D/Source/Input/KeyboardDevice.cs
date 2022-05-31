using UnityEngine;

namespace AngryAsteroids2D.Source.Input
{
    public class KeyboardDevice : IInputDevice 
    {
        /*
         If we would want to have key-remapping,
         I would go by setting mapping data while initializing or updating the keyboard device 
        */
        
        const KeyCode THROTTLE_KEY = KeyCode.UpArrow;
        const KeyCode FIRE_KEY = KeyCode.Space;
        const KeyCode NEGATIVE_TURN = KeyCode.RightArrow;
        const KeyCode POSITIVE_TURN = KeyCode.LeftArrow;
        
        InputData _inputData;
        
        public InputData ReadInput()
        {
            _inputData.ThrottleButton = UnityEngine.Input.GetKey(THROTTLE_KEY);
            _inputData.FireButton = UnityEngine.Input.GetKeyDown(FIRE_KEY);
            _inputData.TurnDirection = GetTurnDirection();
            
            return _inputData;
        }

        float GetTurnDirection()
        {
            float turnDirection = 0;
            if (UnityEngine.Input.GetKey(POSITIVE_TURN))
            {
                turnDirection++;
            }

            if (UnityEngine.Input.GetKey(NEGATIVE_TURN))
            {
                turnDirection--;
            }
            
            return turnDirection;
        }
    }
}
