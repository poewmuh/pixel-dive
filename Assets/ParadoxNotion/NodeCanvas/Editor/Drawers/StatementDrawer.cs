#if UNITY_EDITOR

using _Project.Gameplay.Dialogs.Core;
using ParadoxNotion.Design;
using UnityEngine;
using NodeCanvas.DialogueTrees;

namespace NodeCanvas.Editor
{

    ///<summary>A drawer for dialogue tree statements</summary>
    public class StatementDrawer : ObjectDrawer<Statement>
    {
        public override Statement OnGUI(GUIContent content, Statement instance) {
            if ( instance == null ) { instance = new Statement("..."); }
            instance.text = UnityEditor.EditorGUILayout.TextArea(instance.text, Styles.wrapTextArea, GUILayout.Height(100));
            instance.audio = UnityEditor.EditorGUILayout.ObjectField("Audio File", instance.audio, typeof(AudioClip), false) as AudioClip;
            instance.emotionType = (EmotionType)UnityEditor.EditorGUILayout.EnumPopup("Emotion Type", instance.emotionType);
            instance.meta = UnityEditor.EditorGUILayout.TextField("Metadata", instance.meta);
            return instance;
        }
    }
}

#endif