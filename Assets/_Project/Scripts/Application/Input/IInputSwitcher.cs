namespace _Project.Application.Input
{
    public interface IInputSwitcher
    {
        void EnableInput(InputBlockMask source, bool enable);
    }
}