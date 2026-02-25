using R3;
using UnityEngine.InputSystem;

namespace _Project.Application.Input
{
    public interface IInputSystem
    {
        Subject<InputActionData> OnJump { get; }
        Subject<InputVectorActionData> OnMove { get; }
        Subject<InputActionData> OnPause { get; }
        Subject<InputActionData> OnSkipClicked { get; }
        Subject<InputActionData> OnInteract { get; }
        Subject<InputVectorActionData> OnUIMouseClick { get; }
        Subject<InputVectorActionData> OnPuzzleMouseClick { get; }
        
        ReactiveProperty<string> CurrentControlScheme { get; }
        InputAction PlatformingInteractAction { get; }

        void EnablePlatformingInput();
        void EnablePuzzleInput();
        void EnableUIInput();
    }
}
