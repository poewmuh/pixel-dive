using System;
using System.Collections.Generic;
using _Project.Gameplay.TruePuzzle.Interactable;
using DG.Tweening;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private InventoySpritesConfig _spritesConfig;
        [SerializeField] private InventoryItem _inventoryPrefab;
        [SerializeField] private List<InventorySlot> _slots;
        [SerializeField] private DragDropController _dragDropController;
        
        [Inject] private ISubscriber<OnInteractionCompleteEvent> _onInteractionCompleteSub;
        [Inject] private Camera _camera;
        [Inject] private IObjectResolver _objectResolver;

        private void Start()
        {
            _onInteractionCompleteSub.Subscribe(OnInteractionComplete).AddTo(this);
        }

        private void OnInteractionComplete(OnInteractionCompleteEvent msg)
        {
            var interactableScreenPos = _camera.WorldToScreenPoint(msg.InteractableTransform.position);
            var freeSlot = _slots.Find(slot => slot.IsFree);
            var intenrotyItem = _objectResolver.Instantiate(_inventoryPrefab, freeSlot.transform);
            intenrotyItem.Setup(msg.InteractableType, _spritesConfig.GetInventoryItemSprite(msg.InteractableType), _dragDropController, freeSlot);
            intenrotyItem.transform.position = interactableScreenPos;
            intenrotyItem.transform.DOLocalMove(Vector3.zero, 1f);
            freeSlot.SetItem(intenrotyItem);
        }
    }
}