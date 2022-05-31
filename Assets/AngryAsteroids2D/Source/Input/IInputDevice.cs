namespace AngryAsteroids2D.Source.Input
{
    public struct InputData
    {
        public float TurnDirection;
        public bool ThrottleButton;
        public bool FireButton;
    }

    public interface IInputDevice
    {
        public InputData ReadInput();
    }
}
