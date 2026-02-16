using _Project.Gameplay.Platforming.Input;
using R3;
using UnityEngine;

namespace _Project.Gameplay.Platforming.PlayerView
{
    public class PlayerViewComponent : MonoBehaviour, IComponent
    {
        [SerializeField] private Transform _viewTransform;
        
        private InputComponent _inputComponent;
        private bool _isFacingRight = true;

        private void Awake()
        {
            _inputComponent = GetComponent<InputComponent>();
        }

        void IComponent.Init()
        {
            if (_inputComponent)
            {
                _inputComponent.MoveState.Subscribe(OnMove).AddTo(this);
            }
        }
        
        private void OnMove(float horizontalInput)
        {
            if (_isFacingRight && horizontalInput < 0)
            {
                _isFacingRight = false;
                _viewTransform.Rotate(0f, -180f, 0f);
            }
            else if (!_isFacingRight && horizontalInput > 0)
            {
                _isFacingRight = true;
                _viewTransform.Rotate(0f, 180f, 0f);
            }
        }
    }
}