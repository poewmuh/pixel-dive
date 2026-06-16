using _Project.Gameplay.TruePuzzle.Interactable;
using _Project.GUI;
using AYellowpaper.SerializedCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Cursor
{
    public class CursorOverlay : BaseOverlay
    {
        [SerializeField] private SerializedDictionary<CursorState, Sprite> _cursorSprites;
        [SerializeField] private Image _cursorImage;
        
        [Inject] private ICursorSystem _cursorSystem;

        private RectTransform _cursorTransform;
        private bool _isVisible;
        private Vector3 _lastMousePosition;

        private void Awake()
        {
            _cursorTransform = _cursorImage.rectTransform;
        }

        private void Start()
        {
            _cursorSystem.CurrentState.Subscribe(OnCursorStateChanged).AddTo(this);
        }
        
        private void OnCursorStateChanged(CursorState state)
        {
            _isVisible = state is not CursorState.Disabled;
            _cursorImage.enabled = _isVisible;
            
            if (_cursorSprites.TryGetValue(state, out var sprite))
            {
                _cursorImage.sprite = sprite;
            }
        }

        private void LateUpdate()
        {
            if (!_isVisible) return;
            
            var mousePosition = Input.mousePosition;
            if (_lastMousePosition == mousePosition) return;
            
            _lastMousePosition = mousePosition;
            _cursorTransform.position = mousePosition;
        }
    }
}
