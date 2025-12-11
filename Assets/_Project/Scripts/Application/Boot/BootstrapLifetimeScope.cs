using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Project.Application.Boot
{
    public class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private LogoView _logoView;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_logoView);
        }
    }
}
