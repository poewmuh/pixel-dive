using R3;
using VContainer;

namespace _Project.Application.Input
{
    public class InputSwitcher : IInputSwitcher
    {
        [Inject] private IInputSystem _inputSystem;

        public ReactiveProperty<InputBlockMask> Blockers { get; } = new(InputBlockMask.None);

        void IInputSwitcher.EnableInput(InputBlockMask source, bool enable)
        {
            if (enable)
            {
                Blockers.Value |= source;
                _inputSystem.EnableUIInput();
            }
            else
            {
                Blockers.Value &= ~source;
                if (Blockers.Value is InputBlockMask.None)
                {
                    _inputSystem.EnablePlayerInput();
                }
            }
        }
    }
    
    [System.Flags]
    public enum InputBlockMask
    {
        None        = 0,
        Windows     = 1 << 0,
        Dialogs     = 1 << 1,
        TruePuzzle  = 1 << 2
    }
}