using System;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public interface IInteractable
    {
        void OnHoverEnter();
        void OnHoverExit();
        void OnClick();
    }
}