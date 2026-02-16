using _Project.Application.Asset;
using _Project.Application.Audio;
using _Project.Application.GUI;
using _Project.Application.GUI.WindowsSystem;
using _Project.Application.Input;
using _Project.Application.Persistence;
using _Project.Application.Scene;
using _Project.Application.Settings;
using _Project.Gameplay.Dialogs;
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
            RegisterPersistence(builder);
            RegisterCoreSystems(builder);
        }

        private void RegisterMessageBrokers(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(x =>
                GlobalMessagePipe.SetProvider(x.AsServiceProvider()));
        }

        private void RegisterPersistence(IContainerBuilder builder)
        {
            builder.Register<JsonFileStorageProvider>(Lifetime.Singleton)
                .As<IStorageProvider>();
        }

        private void RegisterCoreSystems(IContainerBuilder builder)
        {
            Cursor.visible = false;
            builder.RegisterInstance(_audioSystem.GetComponent<IAudioSystem>());
            
            builder.Register<SettingsService>(Lifetime.Singleton)
                .AsImplementedInterfaces();
            
            builder.Register<GUISystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AddressablesLoader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<SceneSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<WindowSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameInputSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<InputSwitcher>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}