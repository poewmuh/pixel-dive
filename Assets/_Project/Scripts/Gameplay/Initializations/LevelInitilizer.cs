using System;
using System.Collections.Generic;
using _Project.Application.GUI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace _Project.Gameplay.Initializations
{
    public abstract class LevelInitilizer : MonoBehaviour
    {
        [SerializeField] protected List<OverlayType> _usingOverlays;
        
        [Inject] private IOverlaySystem _overlaySystem;
        
        public abstract void Initilize(IContainerBuilder builder);

        protected virtual void Start()
        {
            CreateOverlays().Forget();
        }

        private async UniTaskVoid CreateOverlays()
        {
            foreach (var overlayType in _usingOverlays)
            {
                await _overlaySystem.CreateOverlay(overlayType);
            }
        }
    }
}
