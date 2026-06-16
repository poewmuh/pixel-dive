using MessagePipe;
using NodeCanvas.Framework;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Interactable
{
    public class InventoryHandler : InteractableHandler
    {
        private const string IsInventoryShowVariableName = "IsInventoryShow";
        
        [SerializeField] private Blackboard[] _needPortalShowBlackboards;
        
        [Inject] private IPublisher<InventoryMessageEvent> _inventoryMessagePub;
        
        public void ShowInventory()
        {
            foreach (var blackboard in _needPortalShowBlackboards)
            {
                if (blackboard != null)
                {
                    blackboard.SetVariableValue(IsInventoryShowVariableName, true);
                }
            }
            
            _inventoryMessagePub.Publish(new InventoryMessageEvent());
        }
    }

    public class InventoryMessageEvent
    {
        
    }
}
