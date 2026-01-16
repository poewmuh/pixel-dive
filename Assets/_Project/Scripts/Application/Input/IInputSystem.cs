using R3;
using UnityEngine.InputSystem;

namespace _Project.Application.Input
{
    public interface IInputSystem
    {
        Subject<InputActionData> OnJump { get; }
        Subject<InputVectorActionData> OnMove { get; }
        Subject<InputActionData> OnPause { get; }
        Subject<InputActionData> OnUIClick { get; }
        Subject<InputActionData> OnInteract { get; }
        
        ReactiveProperty<string> CurrentControlScheme { get; }
        InputAction PlatformingInteractAction { get; }

        void EnablePlayerInput();
        void EnableDialogsInput();
        void EnableUIInput();
    }
}
