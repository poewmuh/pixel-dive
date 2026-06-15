namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public interface IInteractable
    {
        InteractableType InteractableType { get; }
        void OnHoverEnter();
        void OnHoverExit();
        void OnClick();
    }
}