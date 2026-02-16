using System;
using System.Collections.Generic;
using _Project.Application.GUI;
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
            CreateOverlays();
        }

        private void CreateOverlays()
        {
            foreach (var overlayType in _usingOverlays)
            {
                _overlaySystem.CreateOverlay(overlayType);
            }
        }
    }
}