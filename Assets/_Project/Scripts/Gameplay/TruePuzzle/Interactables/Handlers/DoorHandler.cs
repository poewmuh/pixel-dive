namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class DoorHandler : InteractableHandler
    {
        private bool _isDoorOpen;

        protected override void Init()
        {
            UpdateDoorStatus();
        }

        private void UpdateDoorStatus()
        {
            _blackboard.SetVariableValue("IsDoorOpen", _isDoorOpen);
        }
    }
}