using System;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Gameplay.Platforming.Stage
{
    public abstract class LevelDirector : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onStageStart;
        [SerializeField] private UnityEvent _onStageComplete;
        [SerializeField] private UnityEvent _onRunTrueLevel;

        protected Blackboard blackboard;
        protected FSMOwner fsmOwner;
        
        public bool IsActive { get; protected set; }

        private void Awake()
        {
            fsmOwner = GetComponent<FSMOwner>();
            blackboard = GetComponent<Blackboard>();
        }

        public virtual void StartStage()
        {
            IsActive = true;
            _onStageStart?.Invoke();
            fsmOwner.SendEvent("OnStageStart");
        }

        public virtual void CompleteStage()
        {
            IsActive = false;
            _onStageComplete?.Invoke();
            fsmOwner.SendEvent("OnStageFinish");
        }

        public virtual void RunTrueLevel()
        {
            IsActive = false;
            _onRunTrueLevel?.Invoke();
        }
    }
}
