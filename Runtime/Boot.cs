using System;
using Darkness.Runtime.Core;
using DG.Tweening;
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
            DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(100, 30);
            //TODO: logic for game loading
        }

        public void Dispose()
        {
            //Cleanup
        }
    }
}