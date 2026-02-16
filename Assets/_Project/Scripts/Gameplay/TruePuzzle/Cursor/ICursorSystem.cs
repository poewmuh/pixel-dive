using R3;

namespace _Project.Gameplay.TruePuzzle.Cursor
{
    public interface ICursorSystem
    {
        public ReactiveProperty<CursorState> CurrentState { get; }
    }

    public enum CursorState
    {
        Default = 0,
        DefaultPressed = 1,
        Interactive = 2,
        InteractivePressed = 3,
        Disabled = 99
    }
}
