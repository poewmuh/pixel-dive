using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Gameplay.Dialogs.Core.Editor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Localization;

namespace _Project.Gameplay.Dialogs.Core.Editor
{
    [ScriptedImporter(1, DialogGraph.AssetExtension)]
    public class DialogGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var editorGraph = GraphDatabase.LoadGraphForImporter<DialogGraph>(ctx.assetPath);
            var runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogGraph>();

            var nodeIdMap = new Dictionary<INode, string>();

            foreach (var node in editorGraph.GetNodes())
            {
                nodeIdMap[node] = Guid.NewGuid().ToString();
            }

            var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNode != null)
            {
                var entryPort = startNode.GetOutputPort(0).firstConnectedPort;
                if (entryPort != null)
                {
                    runtimeGraph.EntryNodeId = nodeIdMap[entryPort.GetNode()];
                }
            }

            FillRuntimeGraph(runtimeGraph, editorGraph, nodeIdMap);
            
            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }

        private void FillRuntimeGraph(RuntimeDialogGraph runtimeGraph, DialogGraph editorGraph, Dictionary<INode, string> nodeIdMap)
        {
            foreach (var node in editorGraph.GetNodes())
            {
                switch (node)
                {
                    case StartNode:
                        continue;
                    case DialogeNode dialogNode:
                    {
                        var runtimeNode = new RuntimeDialogeNode
                        {
                            NodeId = nodeIdMap[node]
                        };    
                    
                        FillDialogNode(dialogNode, runtimeNode, nodeIdMap);
                        runtimeGraph.Nodes.Add(runtimeNode);
                        break;
                    }
                    case ConditionNode conditionNode:
                    {
                        var runtimeNode = new RuntimeConditionNode
                        {
                            NodeId = nodeIdMap[node]
                        };
                        
                        FillConditionNode(conditionNode, runtimeNode, nodeIdMap);
                        runtimeGraph.Nodes.Add(runtimeNode);
                        break;
                    }
                    case RandomNode randomNode:
                    {
                        var runtimeNode = new RuntimeRandomNode
                        {
                            NodeId = nodeIdMap[node]
                        };
                        
                        FillRandomNode(randomNode, runtimeNode, nodeIdMap);
                        runtimeGraph.Nodes.Add(runtimeNode);
                        break;
                    }
                }
            }
        }
        
        private void FillDialogNode(DialogeNode node, RuntimeDialogeNode runtimeNode, Dictionary<INode, string> nodeIdMap)
        {
            runtimeNode.ActorName = GetPortValue<LocalizedString>(node.GetInputPortByName(DialogeNode.IN_PORT_ACTOR_NAME_NAME))
                .GetLocalizedString();
            runtimeNode.Message = GetPortValue<LocalizedString>(node.GetInputPortByName(DialogeNode.IN_PORT_DIALOGUE_NAME))
                .GetLocalizedString();
            runtimeNode.Emotion = GetPortValue<EmotionType>(node.GetInputPortByName(DialogeNode.IN_PORT_ACTOR_SPRITE_NAME));

            var nextNodePort = node.GetOutputPortByName(DialogeNode.OUTPUT_PORT_DEFAULT_NAME)?.firstConnectedPort;
            if (nextNodePort != null)
            {
                runtimeNode.NextNodeId = nodeIdMap[nextNodePort.GetNode()];
            }
        }
        
        private void FillConditionNode(ConditionNode node, RuntimeConditionNode runtimeNode, Dictionary<INode, string> nodeIdMap)
        {
            runtimeNode.VariableName = GetPortValue<string>(node.GetInputPortByName(ConditionNode.IN_PORT_VARIABLE_NAME));
            runtimeNode.ComparisonType = GetPortValue<ComparisonType>(node.GetInputPortByName(ConditionNode.IN_PORT_COMPARISON_TYPE));
            runtimeNode.CompareValue = GetPortValue<int>(node.GetInputPortByName(ConditionNode.IN_PORT_COMPARE_VALUE));

            var truePort = node.GetOutputPortByName(ConditionNode.OUTPUT_PORT_TRUE)?.firstConnectedPort;
            if (truePort != null)
            {
                runtimeNode.TrueNodeId = nodeIdMap[truePort.GetNode()];
            }
            
            var falsePort = node.GetOutputPortByName(ConditionNode.OUTPUT_PORT_FALSE)?.firstConnectedPort;
            if (falsePort != null)
            {
                runtimeNode.FalseNodeId = nodeIdMap[falsePort.GetNode()];
            }
        }
        
        private void FillRandomNode(RandomNode node, RuntimeRandomNode runtimeNode, Dictionary<INode, string> nodeIdMap)
        {
            var allOutputPorts = new List<IPort>();
            node.GetOutputPortByName(DialogeNode.OUTPUT_PORT_DEFAULT_NAME).GetConnectedPorts(allOutputPorts);
            foreach (var nextNodePort in allOutputPorts)
            {
                runtimeNode.NextNodeIds.Add(nodeIdMap[nextNodePort.GetNode()]);
            }
        }
        
        private T GetPortValue<T>(IPort port)
        {
            if (port == null) return default;

            if (port.isConnected)
            {
                if (port.firstConnectedPort.GetNode() is IVariableNode variableNode)
                {
                    variableNode.variable.TryGetDefaultValue(out T value);
                    return value;
                }
            }
            
            port.TryGetValue(out T fallbackValue);
            return fallbackValue;
        }
    }
}
