using System;
using Unity.GraphToolkit.Editor;

namespace _Project.Gameplay.Dialogs.Core.Editor.Nodes
{
    [Serializable]
    public class StartNode : BaseDialogNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort(OUTPUT_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}