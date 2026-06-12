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
            var interactableScreenPos = _camera.WorldToScreenPoint(msg.InteractableTransform.position);
            var freeSlot = _slots.Find(slot => slot.IsFree);
            var intenrotyItem = _objectResolver.Instantiate(_inventoryPrefab, freeSlot.transform);
            intenrotyItem.Setup(msg.InteractableType, _spritesConfig.GetInventoryItemSprite(msg.InteractableType), _dragDropController, freeSlot);
            intenrotyItem.transform.position = interactableScreenPos;
            intenrotyItem.transform.DOLocalMove(Vector3.zero, 1f);
            freeSlot.SetItem(intenrotyItem);
        }

        private void OnFinalKeyAnimation(FinalKeyEvent msg)
        {
            var firstSlot = _slots.Find(slot => slot.Item.InteractableType == InteractableType.GreenKey);
            var secondSlot = _slots.Find(slot => slot.Item.InteractableType == InteractableType.TubeKey);
            
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

        private void CreateDiamondKey(Vector3 mergePoint)
        {
            var mergePointWorld = _camera.ScreenToWorldPoint(mergePoint);
            var diamondKey = _objectResolver.Instantiate(_diamondKey, mergePointWorld, Quaternion.identity);
            OnInteractionComplete(new OnInteractionCompleteEvent(InteractableType.DiamondKey, diamondKey.transform));
        }
    }
    
    public class FinalKeyEvent { }
}