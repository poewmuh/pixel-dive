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

        private void Start()
        {
            _cursorSystem.CurrentState.Subscribe(OnCursorStateChanged).AddTo(this);
        }
        
        private void OnCursorStateChanged(CursorState state)
        {
            _cursorImage.enabled = state is not CursorState.Disabled;
            if (_cursorSprites.TryGetValue(state, out var sprite))
            {
                _cursorImage.sprite = sprite;
            }
        }

        private void LateUpdate()
        {
            _cursorImage.transform.position = Input.mousePosition;
        }
    }
}