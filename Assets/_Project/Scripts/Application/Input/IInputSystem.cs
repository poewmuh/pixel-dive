using R3;

namespace _Project.Application.Input
{
    public interface IInputSystem
    {
        Subject<InputActionData> OnJump { get; }
        Subject<InputVectorActionData> OnMove { get; }
        Subject<InputActionData> OnPause { get; }

        void EnablePlayerInput(bool isEnabled);
    }
}
