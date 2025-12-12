using _Project.Application.Asset;
using _Project.Application.Audio;
using _Project.Application.GUI;
using _Project.Application.Scene;
using _Project.Application.GUI.WindowsSystem;
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
            
            builder.Register<GUISystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AddressablesLoader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SceneSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<WindowSystem>(Lifetime.Singleton).AsImplementedInterfaces();
        }
        
        private void RegisterMessageBrokers(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(x =>
                GlobalMessagePipe.SetProvider(x.AsServiceProvider()));
        }
    }
}
