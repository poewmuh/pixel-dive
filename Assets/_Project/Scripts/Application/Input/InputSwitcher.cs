using VContainer;

namespace _Project.Application.Input
{
    public class InputSwitcher : IInputSwitcher
    {
        [Inject] private IInputSystem _inputSystem;
        
        private bool _dialogActive;

        void IInputSwitcher.EnableInput(InputBlockMask source, bool enable)
        {
            switch(source)
            {
                case InputBlockMask.Platforming:
                    if (enable) _inputSystem.EnablePlayerInput();
                    else _inputSystem.EnableUIInput();
                    break;
                case InputBlockMask.Windows:
                    if (enable) _inputSystem.EnableUIInput();
                    else if (_dialogActive)
                    {
                        _inputSystem.EnableDialogsInput();
                    }
                    else
                    {
                        _inputSystem.EnablePlayerInput();
                    }
                    break;
                case InputBlockMask.Dialogs:
                    if (enable)
                    {
                        _inputSystem.EnableDialogsInput();
                        _dialogActive = true;
                    }
                    else
                    {
                        _inputSystem.EnablePlayerInput();
                        _dialogActive = false;
                    }
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
        Platforming = 1 << 2,
    }
}