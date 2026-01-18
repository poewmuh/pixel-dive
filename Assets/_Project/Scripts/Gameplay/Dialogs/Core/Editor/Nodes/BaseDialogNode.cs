using Unity.GraphToolkit.Editor;

namespace _Project.Gameplay.Dialogs.Core.Editor.Nodes
{
    public abstract class BaseDialogNode : Node
    {
        public const string INPUT_PORT_DEFAULT_NAME = "InputPort";
        public const string OUTPUT_PORT_DEFAULT_NAME = "OutputPort";
        
        protected void AddInputOutputExecutionPorts(IPortDefinitionContext context)
        {
            context.AddInputPort(INPUT_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(OUTPUT_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}