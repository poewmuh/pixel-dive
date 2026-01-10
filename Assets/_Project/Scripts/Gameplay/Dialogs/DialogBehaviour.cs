using System;
using _Project.Gameplay.Dialogs.Core;
using UnityEngine;

namespace _Project.Gameplay.Dialogs
{
    public class DialogBehaviour : MonoBehaviour
    {
        public event Action OnDialogTextCharWrote;
        
        private event Action _onDialogFinished;
        
        [SerializeField] private EmotionsSpriteConfig _emotionsSpriteConfig;
        [SerializeField] private DialogMessageView _dialogMessageView;

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
                _dialogMessageView.gameObject.SetActive(true);
                _dialogMessageView.Setup(dialogeNode.ActorName, dialogeNode.Message, sprite);
            }
        }

        public void SkipDialog()
        {
            if (_currentNode == null) return;
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
            _dialogMessageView.gameObject.SetActive(false);
            _onDialogFinished?.Invoke();
        }
    }
}
