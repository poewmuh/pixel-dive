using R3;

namespace _Project.Application.Input
{
    public interface IInputSwitcher
    {
        public ReactiveProperty<InputBlockMask> Blockers { get; }
        void EnableInput(InputBlockMask source, bool enable);
    }
}