using System;
using _Project.Application.GUI.DialogeSystem;
using cherrydev;
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
        [SerializeField] private DialogNodeGraph _dialogNode;
        [SerializeField] private bool _disableOnExecute = true;

        [Inject] private IDialogeSystem _dialogeSystem;
        
        public UnityEvent OnDialogEnded;
        
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
                _dialogeSystem.DialogeStart(_dialogNode, OnDialogEnded);
                if (_disableOnExecute)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
