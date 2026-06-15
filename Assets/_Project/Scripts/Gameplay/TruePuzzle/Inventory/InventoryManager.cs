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
        [SerializeField] private GameObject _diamondKey;
        
        [Inject] private ISubscriber<OnInteractionCompleteEvent> _onInteractionCompleteSub;
        [Inject] private ISubscriber<FinalKeyEvent> _onFinalKeySub;
        [Inject] private Camera _camera;
        [Inject] private IObjectResolver _objectResolver;

        private void Start()
        {
            _onInteractionCompleteSub.Subscribe(OnInteractionComplete).AddTo(this);
            _onFinalKeySub.Subscribe(OnFinalKeyAnimation).AddTo(this);
        }

        private void OnInteractionComplete(OnInteractionCompleteEvent msg)
        {
            var freeSlot = _slots.Find(slot => slot.IsFree);
            if (!freeSlot)
            {
                Debug.LogError($"[InventoryManager] No free slot for {msg.InteractableType}.");
                return;
            }

            var interactableScreenPosition = _camera.WorldToScreenPoint(msg.InteractableTransform.position);
            var inventoryItem = _objectResolver.Instantiate(_inventoryPrefab, freeSlot.transform);
            inventoryItem.Setup(
                msg.InteractableType,
                _spritesConfig.GetInventoryItemSprite(msg.InteractableType),
                _dragDropController,
                freeSlot);
            inventoryItem.transform.position = interactableScreenPosition;
            inventoryItem.transform.DOLocalMove(Vector3.zero, 1f);
        }

        private void OnFinalKeyAnimation(FinalKeyEvent _)
        {
            var firstSlot = FindSlotWith(InteractableType.GreenKey);
            var secondSlot = FindSlotWith(InteractableType.TubeKey);
            if (!firstSlot || !secondSlot)
            {
                Debug.LogError("[InventoryManager] Cannot create final key: required keys are missing.");
                return;
            }
            
            var sequence = DOTween.Sequence();
            var mergePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            sequence.Append(firstSlot.Item.transform.DOMove(mergePosition, 1f))
                .Join(secondSlot.Item.transform.DOMove(mergePosition, 1f))
                .AppendInterval(1)
                .OnComplete(() =>
                {
                    Destroy(firstSlot.Item.gameObject);
                    Destroy(secondSlot.Item.gameObject);
                    firstSlot.RemoveItem();
                    secondSlot.RemoveItem();
                    CreateDiamondKey(mergePosition);
                });
        }

        private InventorySlot FindSlotWith(InteractableType interactableType)
        {
            return _slots.Find(slot =>
                slot.Item && slot.Item.InteractableType == interactableType);
        }

        private void CreateDiamondKey(Vector3 mergePoint)
        {
            var mergePointWorld = _camera.ScreenToWorldPoint(mergePoint);
            var diamondKey = _objectResolver.Instantiate(_diamondKey, mergePointWorld, Quaternion.identity);
            OnInteractionComplete(new OnInteractionCompleteEvent(InteractableType.DiamondKey, diamondKey.transform));
        }
    }
    
    public class FinalKeyEvent { }
}
