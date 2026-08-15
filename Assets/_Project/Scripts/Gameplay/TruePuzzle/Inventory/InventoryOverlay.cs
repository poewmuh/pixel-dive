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
    [DeclareBoxGroup("Animation")]
    public class InventoryOverlay : BaseOverlay
    {
        [SerializeField, Group("Links")] private RectTransform _inventoryGroup;
        [SerializeField, Group("Links")] private Button _hideShowButton;
        [SerializeField, Group("Links")] private Image _hideShowButtonImage;
        [SerializeField, Group("Sprites")] private Sprite _showSprite;
        [SerializeField, Group("Sprites")] private Sprite _hideSprite;

        [Tooltip("На сколько панель уезжает вниз при скрытии. В единицах опорного разрешения канваса (1920x1080), поэтому одинаково на любом экране.")]
        [SerializeField, Group("Animation")] private float _hideOffset = 150f;
        [SerializeField, Group("Animation")] private float _duration = .5f;

        [Inject] private ISubscriber<InventoryMessageEvent> _inventorySub;

        private float _shownY;
        private bool _isShowing;
        private Tween _tween;

        private void Awake() => _shownY = _inventoryGroup.anchoredPosition.y;

        private void Start()
        {
            _inventoryGroup.gameObject.SetActive(false);
            _inventorySub.Subscribe(FirstShowInventory).AddTo(this);
            _hideShowButton.onClick.AsObservable().Subscribe(_ => Toggle()).AddTo(this);
        }

        private void FirstShowInventory(InventoryMessageEvent msg)
        {
            _inventoryGroup.gameObject.SetActive(true);
            Apply(true, true);
        }

        [Button]
        private void Toggle() => Apply(!_isShowing, false);

        private void Apply(bool showing, bool instant)
        {
            _isShowing = showing;
            _hideShowButtonImage.sprite = showing ? _hideSprite : _showSprite;

            var targetY = showing ? _shownY : _shownY - _hideOffset;

            if (_tween != null) _tween.Kill();

            if (instant)
            {
                _inventoryGroup.anchoredPosition = new Vector2(_inventoryGroup.anchoredPosition.x, targetY);
                return;
            }

            _tween = _inventoryGroup.DOAnchorPosY(targetY, _duration).SetLink(gameObject);
        }
    }
}
