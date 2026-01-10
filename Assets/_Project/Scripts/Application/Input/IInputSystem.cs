using R3;

namespace _Project.Application.Input
{
    public interface IInputSystem
    {
        Subject<InputActionData> OnJump { get; }
        Subject<InputVectorActionData> OnMove { get; }
        Subject<InputActionData> OnPause { get; }
        Subject<InputActionData> OnUIClick { get; }

        void EnablePlayerInput();
        void EnableDialogsInput();
        void EnableUIInput();
    }
}
