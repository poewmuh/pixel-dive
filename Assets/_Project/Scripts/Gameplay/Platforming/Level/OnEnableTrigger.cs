using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Level
{
    public class OnEnableTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onEnableAction;
        
        private void OnEnable()
        {
            _onEnableAction?.Invoke();
        }
    }
}