using System;

namespace _Project.Gameplay.Dialogs.Core.Editor.Nodes
{
    [Serializable]
    public class RandomNode : BaseDialogNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);
        }
    }
}