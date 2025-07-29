using Darkness.Runtime.Messages;
using MessagePipe;
using UnityEngine;

namespace Darkness.Runtime {
    public static class MessagePipeBoot {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize() {
            var builder = new BuiltinContainerBuilder();
            builder.AddMessagePipe(pipeOptions => {
                pipeOptions.EnableCaptureStackTrace = true;
            });
            
            builder.AddMessageBroker<PerformInputMessage>();
            
            // create provider and set to Global(to enable diagnostics window and global fucntion)
            var provider = builder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(provider);
        }
    }
}