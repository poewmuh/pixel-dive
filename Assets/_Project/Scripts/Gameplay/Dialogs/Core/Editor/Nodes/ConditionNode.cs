using System;
using Unity.GraphToolkit.Editor;

namespace _Project.Gameplay.Dialogs.Core.Editor.Nodes
{
    [Serializable]
    public class ConditionNode : BaseDialogNode
    {
        public const string IN_PORT_VARIABLE_NAME = "VariableName";
        public const string IN_PORT_COMPARISON_TYPE = "ComparisonType";
        public const string IN_PORT_COMPARE_VALUE = "CompareValue";
        public const string OUTPUT_PORT_TRUE = "TruePort";
        public const string OUTPUT_PORT_FALSE = "FalsePort";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(INPUT_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
                
            context.AddInputPort<string>(IN_PORT_VARIABLE_NAME)
                .WithDisplayName("Variable Name")
                .Build();
            
            context.AddInputPort<ComparisonType>(IN_PORT_COMPARISON_TYPE)
                .WithDisplayName("Comparison")
                .Build();
            
            context.AddInputPort<int>(IN_PORT_COMPARE_VALUE)
                .WithDisplayName("Value")
                .Build();
            
            context.AddOutputPort(OUTPUT_PORT_TRUE)
                .WithDisplayName("True")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
                
            context.AddOutputPort(OUTPUT_PORT_FALSE)
                .WithDisplayName("False")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}