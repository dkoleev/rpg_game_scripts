using VContainer;
using VContainer.Unity;

namespace Darkness.Runtime
{
    public class GameLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Boot>();
        }
    }
}