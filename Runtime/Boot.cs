using System;
using Darkness.Runtime.Core;
using VContainer.Unity;

namespace Darkness.Runtime
{
    public class Boot : IStartable, IDisposable
    {
        void IStartable.Start()
        {
            StartGame(RunMode.Full);
        }

        private void StartGame(RunMode runMode)
        {
            //TODO: logic for game loading
        }

        public void Dispose()
        {
            //Cleanup
        }
    }
}