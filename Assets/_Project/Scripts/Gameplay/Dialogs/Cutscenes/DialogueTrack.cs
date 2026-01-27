using UnityEngine.Timeline;

namespace _Project.Gameplay.Dialogs.Cutscenes
{
    [TrackColor(0.2f, 0.6f, 0.9f)]
    [TrackClipType(typeof(DialogClip))]
    [TrackBindingType(typeof(CutsceneDialogBridge))]
    public class DialogueTrack : TrackAsset
    {
    }
}