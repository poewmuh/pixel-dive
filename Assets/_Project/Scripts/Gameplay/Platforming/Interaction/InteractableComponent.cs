using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Interaction
{
    public class InteractableComponent : MonoBehaviour, IInteractable
    {
        [SerializeField] private UnityEvent _onInteract;
        
        void IInteractable.Interact()
        {
            _onInteract?.Invoke();
        }
    }
}