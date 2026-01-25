using _Project.Application.GUI.DialogeSystem;
using _Project.Gameplay.Dialogs.Core;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace _Project.Gameplay.Platforming.Dialogs
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogTrigger : MonoBehaviour
    {
        [SerializeField] private RuntimeDialogGraph _dialogNode;
        [SerializeField] private bool _disableOnExecute = true;
        [SerializeField] private UnityEvent _onDialogEnded;

        [Inject] private IDialogeSystem _dialogeSystem;
        
        private RuntimeVariableStorage _variableStorage;
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _variableStorage = new RuntimeVariableStorage();
            _variableStorage.SetVariable("interactCount", 0);
            
            _collider.OnTriggerEnter2DAsObservable().Subscribe(OnEnter).AddTo(this);
        }

        private void OnEnter(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _dialogeSystem.DialogStart(_dialogNode, () => _onDialogEnded.Invoke(), _variableStorage);
                if (_disableOnExecute)
                {
                    gameObject.SetActive(false);
                }

                _variableStorage.IncrementInt("interactCount");
            }
        }
    }
}
