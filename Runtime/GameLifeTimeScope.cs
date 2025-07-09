using Darkness.Runtime.Messages;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Darkness.Runtime
{
    public class GameLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Register MessagePipe
            var options = builder.RegisterMessagePipe();
            // Register message handlers
            builder.RegisterMessageBroker<GameLoadedMessage>(options);
            
            builder.RegisterEntryPoint<Boot>();
        }
    }
}