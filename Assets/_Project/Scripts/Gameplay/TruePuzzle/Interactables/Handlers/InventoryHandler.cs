using MessagePipe;
using NodeCanvas.Framework;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class InventoryHandler : InteractableHandler
    {
        [SerializeField] private Blackboard[] _needPortalShowBlackboards;
        [Inject] private IPublisher<InventoryMessageEvent> _inventoryMessagePub;
        
        public void ShowInventory()
        {
            foreach (var blackboard in _needPortalShowBlackboards)
            {
                blackboard.SetVariableValue("IsInventoryShow", true);
            }
            
            _inventoryMessagePub.Publish(new InventoryMessageEvent());
        }
    }

    public class InventoryMessageEvent
    {
        
    }
}