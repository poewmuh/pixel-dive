using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Project.Gameplay.Dialogs.Cutscenes
{
    public class DialogClip : PlayableAsset, ITimelineClipAsset
    {
        public DialogueClipBehaviour template = new ();
        
        public ClipCaps clipCaps => ClipCaps.None;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DialogueClipBehaviour>.Create(graph, template);
            var behaviour = playable.GetBehaviour();
            behaviour.director = owner.GetComponent<PlayableDirector>();
            return playable;
        }
    }
}
