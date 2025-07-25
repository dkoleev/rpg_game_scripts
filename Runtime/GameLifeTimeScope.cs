using Darkness.Runtime.Experimental;
using Darkness.Runtime.Gameplay;
using Darkness.Runtime.Log;
using Darkness.Runtime.Utils.Resource;
using MessagePipe;
using VContainer;
using VContainer.Unity;

namespace Darkness.Runtime {
    public class GameLifeTimeScope : LifetimeScope {
        protected override void Configure(IContainerBuilder builder) {
            // RegisterMessagePipe returns options.
            var options = builder.RegisterMessagePipe( /* configure option */);
            // Setup GlobalMessagePipe to enable a diagnostics window and global function
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // RegisterMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
            /*
             * If you are using Unity 2022.1 or later and VContainer 1.14.0 or later, you do not need RegsiterMessageBroker<>.
             * A set of types including ISubscriber<>, IPublisher<> or its asynchronous version will be resolved automatically.
             * Note that IRequesthandler<> and IRequestAllHanlder<> still require manual registration.
             */
            //builder.RegisterMessageBroker<GameLoadedMessage>(options);
            // also exists RegisterMessageBroker<TKey, TMessage>, RegisterRequestHandler, RegisterAsyncRequestHandler
            // RegisterMessageHandlerFilter: Register for filter, also exists RegisterAsyncMessageHandlerFilter, Register(Async)RequestHandlerFilter
            //builder.RegisterMessageHandlerFilter<MyFilter<int>>();

            builder.Register<GameLogger>(Lifetime.Singleton);
            builder.Register<AddressableLoader>(Lifetime.Singleton);
            builder.Register<PlayerPlatformerAttack>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            // builder.RegisterEntryPoint<Boot>();
            // builder.RegisterEntryPoint<InputHandler>();
            
            // builder.Register<EntityViewManager>(Lifetime.Singleton);
            // builder.RegisterSystemFromDefaultWorld<EntityViewSyncSystem>();
        }
    }
}