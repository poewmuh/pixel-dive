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
