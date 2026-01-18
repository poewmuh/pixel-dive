using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Stage
{
    public abstract class LevelDirector : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onStageStart;
        [SerializeField] private UnityEvent _onStageComplete;
        
        public bool IsActive { get; protected set; }

        public virtual void StartStage()
        {
            IsActive = true;
            _onStageStart?.Invoke();
        }

        public virtual void CompleteStage()
        {
            IsActive = false;
            _onStageComplete?.Invoke();
        }
    }
}
