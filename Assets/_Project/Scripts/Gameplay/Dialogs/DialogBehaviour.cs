using System;
using _Project.Gameplay.Dialogs.Core;
using UnityEngine;

namespace _Project.Gameplay.Dialogs
{
    public class DialogBehaviour : MonoBehaviour
    {
        private event Action _onDialogFinished;
        
        [SerializeField] private EmotionsSpriteConfig _emotionsSpriteConfig;
        [field: SerializeField] public DialogMessageView DialogMessageView { get; private set; }

        private RuntimeDialogGraph _currentGraph;
        private RuntimeBaseNode _currentNode;

        public void StartDialog(RuntimeDialogGraph graph, Action onDialogFinished)
        {
            _currentGraph = graph;
            _onDialogFinished = onDialogFinished;
            var startNode = _currentGraph.Nodes.Find(x => x.NodeId == _currentGraph.EntryNodeId);
            if (startNode == null)
            {
                FinishDialogs();
                return;
            }

            ExecuteNodeByType(startNode);
        }

        private void ExecuteNodeByType(RuntimeBaseNode node)
        {
            _currentNode = node;
            
            if (node is RuntimeDialogeNode dialogeNode)
            {
                var sprite = _emotionsSpriteConfig.GetSpriteForEmotion(dialogeNode.Emotion);
                DialogMessageView.gameObject.SetActive(true);
                DialogMessageView.Setup(dialogeNode.ActorName, dialogeNode.Message, sprite);
            }
        }

        public void SkipDialog()
        {
            if (_currentNode == null) return;
            if (DialogMessageView.IsTyping)
            {
                DialogMessageView.SkipTypewriter();
                return;
            }
            if (string.IsNullOrEmpty(_currentNode.NextNodeId))
            {
                FinishDialogs();
                return;
            }
            
            var nextNode = _currentGraph.Nodes.Find(x => x.NodeId == _currentNode.NextNodeId);
            ExecuteNodeByType(nextNode);
        }

        public void CloseAllDialogs()
        {
            FinishDialogs();
        }

        private void FinishDialogs()
        {
            _currentNode = null;
            _currentGraph = null;
            DialogMessageView.gameObject.SetActive(false);
            _onDialogFinished?.Invoke();
        }
    }
}
