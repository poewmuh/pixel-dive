using System;
using _Project.Gameplay.TruePuzzle.Interactable;
using _Project.GUI;
using DG.Tweening;
using MessagePipe;
using R3;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Inventory
{
    [DeclareBoxGroup("Links")]
    [DeclareBoxGroup("Sprites")]
    public class InventoryOverlay : BaseOverlay
    {
        [SerializeField, Group("Links")] private Transform _inventoryGroup;
        [SerializeField, Group("Links")] private Button _hideShowButton;
        [SerializeField, Group("Links")] private Image _hideShowButtonImage;
        [SerializeField, Group("Sprites")] private Sprite _showSprite;
        [SerializeField, Group("Sprites")] private Sprite _hideSprite;

        [Inject] private ISubscriber<InventoryMessageEvent> _inventorySub;
        [SerializeField] private float _hiddenValue = -300;
        [SerializeField] private float _showingValue = -200;

        private bool _isShowing;

        private void Start()
        {
            _inventoryGroup.gameObject.SetActive(false);
            _inventorySub.Subscribe(FirstShowInventory).AddTo(this);
            _hideShowButton.onClick.AsObservable().Subscribe(OnHideShow).AddTo(this);
        }

        private void FirstShowInventory(InventoryMessageEvent msg)
        {
            _inventoryGroup.gameObject.SetActive(true);
            _isShowing = true;
        }

        [Button]
        private void OnHideShow(Unit _)
        {
            _isShowing = !_isShowing;
            _hideShowButtonImage.sprite = _isShowing ? _hideSprite : _showSprite;
            var endValue = _isShowing ? _showingValue : _hiddenValue;
            _inventoryGroup.DOLocalMoveY(endValue, .5f);
        }
    }
}
