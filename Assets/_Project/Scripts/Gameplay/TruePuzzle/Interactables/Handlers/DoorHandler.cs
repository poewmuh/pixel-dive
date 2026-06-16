namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class DoorHandler : InteractableHandler
    {
        private const string IsDoorOpenVariableName = "IsDoorOpen";
        
        private bool _isDoorOpen;

        protected override void Init()
        {
            UpdateDoorStatus();
        }

        private void UpdateDoorStatus()
        {
            _blackboard?.SetVariableValue(IsDoorOpenVariableName, _isDoorOpen);
        }
    }
}
