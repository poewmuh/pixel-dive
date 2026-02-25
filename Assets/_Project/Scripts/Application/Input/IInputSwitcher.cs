using R3;

namespace _Project.Application.Input
{
    public interface IInputSwitcher
    {
        ReactiveProperty<InputMapMode> InputMap { get; }
        void SetLevelMode(InputMapMode mode);
        ReactiveProperty<InputBlockMask> Blockers { get; }
        void EnableInput(InputBlockMask source, bool enable);
    }
}