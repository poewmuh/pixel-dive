using R3;
using VContainer;

namespace _Project.Application.Input
{
    public class InputSwitcher : IInputSwitcher
    {
        [Inject] private IInputSystem _inputSystem;

        public ReactiveProperty<InputMapMode> InputMap { get; } = new(Input.InputMapMode.Platforming);
        public ReactiveProperty<InputBlockMask> Blockers { get; } = new(InputBlockMask.None);
        
        void IInputSwitcher.SetLevelMode(InputMapMode mode)
        {
            InputMap.Value = mode;
            if (Blockers.Value is InputBlockMask.None)
                ApplyMode(mode);
        }

        void IInputSwitcher.EnableInput(InputBlockMask source, bool enable)
        {
            if (enable)
            {
                Blockers.Value |= source;
            }
            else
            {
                Blockers.Value &= ~source;
            }
            
            if (Blockers.Value != InputBlockMask.None)
                _inputSystem.EnableUIInput();
            else
                ApplyMode(InputMap.Value);
        }
        
        private void ApplyMode(InputMapMode mode)
        {
            switch (mode)
            {
                case InputMapMode.Platforming:
                    _inputSystem.EnablePlatformingInput();
                    break;
                case InputMapMode.Puzzle:
                    _inputSystem.EnablePuzzleInput();
                    break;
            }
        }
    }
    
    [System.Flags]
    public enum InputBlockMask
    {
        None        = 0,
        Windows     = 1 << 0,
        Dialogs     = 1 << 1,
    }
    
    public enum InputMapMode
    {
        Platforming,
        Puzzle
    }
}