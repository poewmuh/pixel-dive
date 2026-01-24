using System;
using _Project.Gameplay.Dialogs.Core;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Project.Gameplay.Dialogs
{
    public class DialogBehaviour : MonoBehaviour
    {
        private event Action _onDialogFinished;
        
        [SerializeField] private UnityEvent _onDialogStartedEvent;
        [SerializeField] private UnityEvent _onNextDialogEvent;
        [SerializeField] private UnityEvent _onDialogFinishedEvent;
        
        [SerializeField] private EmotionsSpriteConfig _emotionsSpriteConfig;
        [field: SerializeField] public DialogMessageView DialogMessageView { get; private set; }

        private RuntimeDialogGraph _currentGraph;
        private RuntimeBaseNode _currentNode;
        private RuntimeVariableStorage _variables;

        public void StartDialog(RuntimeDialogGraph graph, Action onDialogFinished, RuntimeVariableStorage variables)
        {
            _currentGraph = graph;
            _onDialogFinished = onDialogFinished;
            _variables = variables ?? new RuntimeVariableStorage();
            var startNode = _currentGraph.Nodes.Find(x => x.NodeId == _currentGraph.EntryNodeId);
            if (startNode == null)
            {
                FinishDialogs();
                return;
            }

            DialogMessageView.gameObject.SetActive(true);
            ExecuteNodeByType(startNode);
            _onDialogStartedEvent?.Invoke();
        }

        private void ExecuteNodeByType(RuntimeBaseNode node)
        {
            _currentNode = node;
            
            switch (node)
            {
                case RuntimeDialogeNode dialogeNode:
                    ExecuteDialogNode(dialogeNode);
                    return;
                    
                case RuntimeConditionNode conditionNode:
                    ExecuteConditionNode(conditionNode);
                    return;
                    
                case RuntimeRandomNode randomNode:
                    ExecuteRandomNode(randomNode);
                    return;
            }
            
            Debug.LogError($"Unknown node type: {node.GetType()}");
            FinishDialogs();
        }
        
        private void ExecuteDialogNode(RuntimeDialogeNode dialogeNode)
        {
            var sprite = _emotionsSpriteConfig.GetSpriteForEmotion(dialogeNode.Emotion);
            DialogMessageView.Setup(dialogeNode.ActorName, dialogeNode.Message, sprite);
        }
        
        private void ExecuteConditionNode(RuntimeConditionNode conditionNode)
        {
            var result = conditionNode.Evaluate(_variables);
            var nextNodeId = result ? conditionNode.TrueNodeId : conditionNode.FalseNodeId;
            
            if (string.IsNullOrEmpty(nextNodeId))
            {
                FinishDialogs();
                return;
            }
            
            var nextNode = _currentGraph.Nodes.Find(x => x.NodeId == nextNodeId);
            ExecuteNodeByType(nextNode);
        }
        
        private void ExecuteRandomNode(RuntimeRandomNode randomNode)
        {
            if (randomNode.NextNodeIds.Count == 0)
            {
                FinishDialogs();
                return;
            }

            var randomIndex = Random.Range(0, randomNode.NextNodeIds.Count);
            var nextNode = _currentGraph.Nodes.Find(x => x.NodeId == randomNode.NextNodeIds[randomIndex]);
            if (nextNode == null)
            {
                Debug.LogError($"Random node not found: {randomNode.NextNodeIds[randomIndex]} index: {randomIndex}");
            }
            ExecuteNodeByType(nextNode);
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
            
            _onNextDialogEvent?.Invoke();
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
            _onDialogFinishedEvent?.Invoke();
        }
    }
}
