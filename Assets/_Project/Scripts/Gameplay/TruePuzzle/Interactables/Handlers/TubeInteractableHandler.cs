using _Project.Application.GUI.WindowsSystem;
using _Project.Gameplay.TruePuzzle.TubeGame;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class TubeInteractableHandler : InteractableHandler
    {
        private const string IsCompletedVariableName = "IsCompleted";
        
        [Inject] private IWindowSystem _windowSystem;
        
        [SerializeField] private UnityEvent _onPuzzleSolve;
        
        protected override void Init()
        {
            _blackboard?.SetVariableValue(IsCompletedVariableName, false);
        }

        public void GameStart()
        {
            ShowGameWindow().Forget();
        }

        private async UniTaskVoid ShowGameWindow()
        {
            try
            {
                var window = await _windowSystem.Show<TubeGameWindow>();
                window.OnComplete.Subscribe(OnGameComplete).AddTo(this);
            }
            catch (System.Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }

        private void OnGameComplete(Unit _)
        {
            _windowSystem.Close<TubeGameWindow>();
            _onPuzzleSolve?.Invoke();
        }
    }
}
