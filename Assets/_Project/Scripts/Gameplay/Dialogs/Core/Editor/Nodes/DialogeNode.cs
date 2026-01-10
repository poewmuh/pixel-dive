using System;
using UnityEngine.Localization;

namespace _Project.Gameplay.Dialogs.Core.Editor.Nodes
{
    [Serializable]
    public class DialogeNode : BaseDialogNode
    {
        public const string IN_PORT_ACTOR_NAME_NAME = "ActorName";
        public const string IN_PORT_ACTOR_SPRITE_NAME = "ActorSprite";
        public const string IN_PORT_DIALOGUE_NAME = "Dialogue";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            AddInputOutputExecutionPorts(context);

            context.AddInputPort<LocalizedString>(IN_PORT_ACTOR_NAME_NAME)
                .WithDisplayName("Actor Name")
                .Build();
            context.AddInputPort<EmotionType>(IN_PORT_ACTOR_SPRITE_NAME)
                .WithDisplayName("Actor Sprite")
                .Build();
            context.AddInputPort<LocalizedString>(IN_PORT_DIALOGUE_NAME)
                .Build();
        }
    }
}
