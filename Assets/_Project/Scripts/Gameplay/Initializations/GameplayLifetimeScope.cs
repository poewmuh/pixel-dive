using System.Collections.Generic;
using _Project.Application.GUI;
using _Project.Gameplay.Dialogs;
using _Project.Gameplay.TruePuzzle.Cursor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay.Initializations
{
    public class GameplayLifetimeScope : LifetimeScope
    {
        [SerializeField] private LevelInitilizer levelInitilizer;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PauseManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<OverlaySystem>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<CursorSystem>(Lifetime.Scoped).AsImplementedInterfaces();

            if (levelInitilizer)
            {
                levelInitilizer.Initilize(builder);
            }
        }
    }
}
