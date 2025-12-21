using VContainer;
using VContainer.Unity;

namespace _Project.Gameplay.Initializations
{
    public class TrainingLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PauseManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
