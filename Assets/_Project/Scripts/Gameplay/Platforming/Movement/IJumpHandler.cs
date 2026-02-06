using System;
using R3;

namespace _Project.Gameplay.Platforming.Movement
{
    public interface IJumpHandler : IDisposable
    {
        public ReactiveProperty<JumpState> State { get; }
        void JumpPressed();
        void JumpReleased();
        void Tick();
    }
}