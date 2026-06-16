using System;
using System.Collections;
using _Project.Gameplay.FadeGUI;
using _Project.Gameplay.Platforming.Stage;
using _Project.Gameplay.TruePuzzle.Interactable;
using _Project.Gameplay.TruePuzzle.Inventory;
using _Project.Gameplay.TruePuzzle.Player;
using _Project.GUI;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using TriInspector;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.TruePuzzle.Level
{
    public class StartLevelDirector : LevelDirector
    {
        [Inject] private IPublisher<FadeMessageEvent> _fadeMessagePub;
        [Inject] private ISubscriber<DragFailedMessage> _dragFailedSub;
        [Inject] private IPublisher<OilOverlayEventMessage> _oilOverlayMessagePub;
        [Inject] private IPublisher<FinalKeyEvent> _onFinalKeyPub;
        
        [SerializeField] private SerializedDictionary<CurrentLevelType, GameObject[]> _levelObjects;
        [SerializeField] private DepthScalerComponent _depthScalerComponent;
        [SerializeField] private SerializedDictionary<CurrentLevelType, GameObject[]> _redScenesObjects;

        private void Start()
        {
            _dragFailedSub.Subscribe(OnDragFailed).AddTo(this);
        }

        private void OnDragFailed(DragFailedMessage msg)
        {
            if (msg.Reason is DragFailedReason.InvalidTarget)
            {
                fsmOwner.SendEvent("OnFailedDrag");
            }
        }

        public IEnumerator AsyncFade(FadeType fadeType)
        {
            var completion = new UniTaskCompletionSource();
            _fadeMessagePub.Publish(new FadeMessageEvent(fadeType, Color.black, completion));
            while (completion.Task.Status is UniTaskStatus.Pending)
            {
                yield return completion.Task;
            }
        }

        public void ActiveOilEffect(bool isActive)
        {
            _oilOverlayMessagePub.Publish(new OilOverlayEventMessage(isActive));
        }

        public void StartFinalKey()
        {
            _onFinalKeyPub.Publish(new FinalKeyEvent());
        }

        public void RecalculateGraph()
        {
            AstarPath.active.Scan();
        }

        public void LoadLevel(CurrentLevelType levelType)
        {
            DisableAllObjects(_levelObjects);
            EnableObjectsForLevel(_levelObjects, levelType);

            _depthScalerComponent.SetLevel(levelType);
        }
        
        public void LoadRedSceneLevel(CurrentLevelType levelType)
        {
            Debug.Log($"[LEVEL DIRECTOR] Loading red scene level: {levelType}");
            DisableAllObjects(_redScenesObjects);
            EnableObjectsForLevel(_redScenesObjects, levelType);
        }
        
        private void DisableAllObjects(SerializedDictionary<CurrentLevelType, GameObject[]> levelObjects)
        {
            foreach (var objects in levelObjects.Values)
            {
                SetObjectsActive(objects, false);
            }
        }

        private void EnableObjectsForLevel(
            SerializedDictionary<CurrentLevelType, GameObject[]> levelObjects,
            CurrentLevelType levelType)
        {
            if (levelObjects.TryGetValue(levelType, out var objects))
            {
                SetObjectsActive(objects, true);
                return;
            }
            
            Debug.LogWarning($"{nameof(StartLevelDirector)} has no objects configured for level {levelType}.", this);
        }

        private static void SetObjectsActive(GameObject[] objects, bool isActive)
        {
            if (objects == null) return;
            
            foreach (var levelObject in objects)
            {
                if (levelObject != null)
                {
                    levelObject.SetActive(isActive);
                }
            }
        }

        [Button]
        public void LoadStartLevel()
        {
            LoadLevel(CurrentLevelType.StartLevel);
        }
        
        [Button]
        public void LoadFirstKeyLevel()
        {
            LoadLevel(CurrentLevelType.FirstKeyLevel);
        }
        
        [Button]
        public void LoadGreenLevel()
        {
            LoadLevel(CurrentLevelType.GreenLevel);
        }
        
        [Button]
        public void LoadRedLevel()
        {
            LoadLevel(CurrentLevelType.RedLevel);
        }
    }

    public enum CurrentLevelType
    {
        StartLevel,
        FirstKeyLevel,
        GreenLevel,
        RedLevel,
        Scene1,
        Scene2,
        Scene3,
        Scene4,
        Scene5,
        Scene6,
    }
}
