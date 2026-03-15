using System;
using System.Collections;
using System.Linq;
using _Project.Gameplay.FadeGUI;
using _Project.Gameplay.Platforming.Stage;
using _Project.Gameplay.TruePuzzle.Interactable;
using _Project.Gameplay.TruePuzzle.Player;
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
        [SerializeField] private SerializedDictionary<CurrentLevelType, GameObject[]> _levelObjects;
        [SerializeField] private DepthScalerComponent _depthScalerComponent;
        
        public IEnumerator AsyncFade(FadeType fadeType)
        {
            var completion = new UniTaskCompletionSource();
            _fadeMessagePub.Publish(new FadeMessageEvent(fadeType, Color.black, completion));
            while (completion.Task.Status is UniTaskStatus.Pending)
            {
                yield return completion.Task;
            }
        }

        public void RecalculateGraph()
        {
            AstarPath.active.Scan();
        }

        public void LoadLevel(CurrentLevelType levelType)
        {
            foreach (var levelObject in _levelObjects.Values.SelectMany(levelObjects => levelObjects))
            {
                levelObject.SetActive(false);
            }

            foreach (var levelObject in _levelObjects[levelType])
            {
                levelObject.SetActive(true);
            }

            _depthScalerComponent.SetLevel(levelType);
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
    }

    public enum CurrentLevelType
    {
        StartLevel,
        FirstKeyLevel
    }
}
