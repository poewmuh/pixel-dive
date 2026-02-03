using NodeCanvas.DialogueTrees;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Dialogs
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueTreeController _dialogGraph;
        [SerializeField] private bool _disableOnExecute = true;
        [SerializeField] private UnityEvent _onDialogEnded;
        
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            
            _collider.OnTriggerEnter2DAsObservable().Subscribe(OnEnter).AddTo(this);
        }

        private void OnEnter(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _dialogGraph.StartDialogue(_ => _onDialogEnded?.Invoke());
                if (_disableOnExecute)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
