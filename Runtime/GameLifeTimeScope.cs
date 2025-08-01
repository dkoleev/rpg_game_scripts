using System;
using Darkness.Runtime.DebTools;
using Darkness.Runtime.Experimental;
using Darkness.Runtime.Gameplay;
using Darkness.Runtime.Gameplay.Levels;
using Darkness.Runtime.Gameplay.Player;
using Darkness.Runtime.Log;
using Darkness.Runtime.Messages;
using Darkness.Runtime.ScriptableObjects;
using Darkness.Runtime.Utils.Resource;
using MessagePipe;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Darkness.Runtime {
    public class GameLifeTimeScope : LifetimeScope {
        [SerializeField] private LevelsList levelsList;
        [SerializeField] private CharactersList charactersList;
        
        protected override void Configure(IContainerBuilder builder) {
            // RegisterMessagePipe(builder);

            // builder.RegisterInstance(gameSettings.PlayerSettings);
            // builder.RegisterInstance(gameSettings.CharactersViewRef);
            builder.RegisterInstance(levelsList);
            builder.RegisterInstance(charactersList);
            
            builder.Register<GameLogger>(Lifetime.Singleton);
            builder.Register<AddressableLoader>(Lifetime.Singleton);
            builder.Register<LevelsManager>(Lifetime.Singleton);
            builder.Register<SpawnManager>(Lifetime.Singleton);
            builder.Register<SaveSystem>(Lifetime.Singleton);
            builder.RegisterEntryPoint<ProgressAutoSaver>();

            builder.RegisterEntryPoint<InputHandler>();
            builder.RegisterEntryPoint<Boot>();
            
            //----DEBUG-----
            builder.Register<DebugHotKeys>(Lifetime.Singleton).AsImplementedInterfaces();
            //-------------

            // builder.Register<EntityViewManager>(Lifetime.Singleton);
            // builder.RegisterSystemFromDefaultWorld<EntityViewSyncSystem>();
        }

        private void RegisterMessagePipe(IContainerBuilder builder) {
            // RegisterMessagePipe returns options.
            var options = builder.RegisterMessagePipe(pipeOptions => {
                pipeOptions.EnableCaptureStackTrace = true;
            });
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

         
            // builder.RegisterMessageBroker<PerformAttackInputMessage>(options);
        }
    }
}