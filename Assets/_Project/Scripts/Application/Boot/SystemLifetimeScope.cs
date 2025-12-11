using _Project.Application.Asset;
using _Project.Application.Audio;
using _Project.Application.Scene;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Application.Boot
{
    public class SystemLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameObject _audioSystem;
        
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessageBrokers(builder);
            
            builder.RegisterInstance(_audioSystem.GetComponent<IAudioSystem>());
            
            builder.Register<AddressablesLoader>(Lifetime.Singleton).As<IAssetLoader>();
            builder.Register<SceneSystem>(Lifetime.Singleton).As<ISceneSystem>();
        }
        
        private void RegisterMessageBrokers(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(x =>
                GlobalMessagePipe.SetProvider(x.AsServiceProvider()));
        }
    }
}
