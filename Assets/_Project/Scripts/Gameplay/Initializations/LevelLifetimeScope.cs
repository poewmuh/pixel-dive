using _Project.Gameplay.Platforming.CheckPoints;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay.Initializations
{
    public class LevelLifetimeScope : LifetimeScope
    {
        [SerializeField] private CheckPointManager _checkPointManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PauseManager>(Lifetime.Singleton).AsImplementedInterfaces();

            if (_checkPointManager != null)
            {
                builder.RegisterInstance(_checkPointManager).As<CheckPointManager>();
            }
        }
    }
}
