using _Project.Application.Input;
using _Project.Gameplay.Platforming.Input;
using R3;
using R3.Triggers;
using TMPro;
using UnityEngine;
using Utils;
using VContainer;

namespace _Project.Gameplay.Platforming.Interaction
{
    public class PlayerInteractionComponent : MonoBehaviour, IComponent
    {
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private GameObject _hintTextObject;
        [SerializeField] private TextMeshPro _interactSpriteText;

        [Inject] private IInputSystem _inputSystem;

        private IInteractable _interactable;

        void IComponent.Init()
        {
            _collider2D.OnTriggerEnter2DAsObservable().Subscribe(OnEnter).AddTo(this);
            _collider2D.OnTriggerExit2DAsObservable().Subscribe(OnExit).AddTo(this);

            if (TryGetComponent(out InputComponent input))
            {
                input.InteractState.Subscribe(OnInteractClicked).AddTo(this);
            }

            _inputSystem.CurrentControlScheme.Subscribe(OnSchemeChange).AddTo(this);
        }

        private void OnEnter(Collider2D collider)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;
            
            _hintTextObject.SetActive(true);
            _interactable = collider.GetComponentInParent<IInteractable>();
        }

        private void OnSchemeChange(string scheme)
        {
            if (scheme == null) return;
            
            _interactSpriteText.text = GetInteractKey();
        }
        
        private string GetInteractKey()
        {
            foreach (var interactBinding in _inputSystem.PlatformingInteractAction.bindings)
            {
                if (!interactBinding.groups.Contains(_inputSystem.CurrentControlScheme.Value)) continue;
                var binding = InputUtils.PrettifyBinding(interactBinding.effectivePath);
                var spriteIndex = _interactSpriteText.spriteAsset.GetSpriteIndexFromName(binding);
                return $"<sprite={spriteIndex}>";
            }

            return "";
        }

        private void OnExit(Collider2D collider)
        {
            if (collider.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;

            _interactable = null;
            _hintTextObject.SetActive(false);
        }

        private void OnInteractClicked(Unit _)
        {
            if (_interactable == null) return;
            
            _interactable.Interact();
        }
    }
}
