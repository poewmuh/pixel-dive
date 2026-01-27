using _Project.Application.GUI.DialogeSystem;
using _Project.Gameplay.Dialogs.Core;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Playables;

namespace _Project.Gameplay.Dialogs.Cutscenes
{
    [System.Serializable]
    public class DialogueClipBehaviour : PlayableBehaviour
    {
        public LocalizedString actorName;
        public LocalizedString message;
        public EmotionType emotion;
    
        [HideInInspector] public PlayableDirector director;
    
        private IDialogeSystem _dialogSystem;
        private bool _hasStarted;
        private bool _dialogSkipped;
        private bool _timelinePaused;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _hasStarted = false;
            _dialogSkipped = false;
            _timelinePaused = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var bridge = playerData as CutsceneDialogBridge;
            if (bridge == null)
            {
                Debug.LogError("DialogueTrack должен быть привязан к CutsceneDialogBridge!");
                return;
            }

            _dialogSystem = bridge.DialogSystem;
            if (_dialogSystem == null)
                return;

            if (!_hasStarted)
            {
                StartDialogue(playable);
                _hasStarted = true;
            }

            bool typingFinished = !_dialogSystem.IsTyping();
            double currentTime = playable.GetTime();
            double duration = playable.GetDuration();
            bool clipAlmostEnded = currentTime >= duration * 0.99;

            if ((typingFinished || clipAlmostEnded) && !_timelinePaused && !_dialogSkipped)
            {
                PauseTimeline();
                _timelinePaused = true;
            }

            if (_dialogSystem.IsSkipped())
            {
                _dialogSkipped = true;

                if (_timelinePaused)
                {
                    ResumeTimeline();
                    _timelinePaused = false;
                }
            }
        }
        
        private void StartDialogue(Playable playable)
        {
            double clipDuration = playable.GetDuration();
            var localizedMessage = message.GetLocalizedString();
            int charCount = localizedMessage.Length;
            
            Debug.Log($"Clip duration: {clipDuration} seconds");
            Debug.Log($"Message: '{localizedMessage}', chars: {charCount}");

            if (charCount == 0)
            {
                _dialogSystem.CutsceneDialogStart(
                    actorName.GetLocalizedString(),
                    localizedMessage,
                    emotion
                );
                return;
            }

            const string punctuationMarks = ".,!?;";
            int punctuationCount = 0;

            foreach (char c in localizedMessage)
            {
                if (punctuationMarks.Contains(c))
                    punctuationCount++;
            }

            int regularChars = charCount - punctuationCount;
            float letterDelay = (float)(clipDuration / (regularChars + punctuationCount * 10));
            Debug.Log($"regularChars: {regularChars}, punctuationCount: {punctuationCount}, letterDelay: {letterDelay}");

            _dialogSystem.CutsceneDialogStart(
                actorName.GetLocalizedString(),
                localizedMessage,
                emotion,
                letterDelay
            );
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (_dialogSystem != null && _hasStarted)
            {
                //_dialogSystem.CloseCutsceneDialog();
            
                if (_timelinePaused)
                {
                    ResumeTimeline();
                }
            }
        
            _hasStarted = false;
        }

        private void PauseTimeline()
        {
            if (director != null)
            {
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            }
        }

        private void ResumeTimeline()
        {
            if (director != null)
            {
                director.playableGraph.GetRootPlayable(0).SetSpeed(1);
            }
        }
    }
}