using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool.RegisterWaitForSingleObject.Example.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            Timer timer = new Timer();
            await timer.StartAsync(() => { }, 5000);

            
        }
    }

    public class Timer
    {
        private readonly AutoResetEvent timerEvent = new AutoResetEvent(false);

        public Task StartAsync(Action action, int millisecondsTimeOutInterval, CancellationToken cancellationToken = default(CancellationToken))
        {
            return OnStartAsync(action, millisecondsTimeOutInterval, cancellationToken);
        }

        protected Task OnStartAsync(Action action, int millisecondsTimeOutInterval, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var waitOrTimeoutCallback = new WaitOrTimerCallback((state, timedOut) =>
            {
                action();

                OnStartAsync(action, millisecondsTimeOutInterval, cancellationToken);
            });

            var registeredWaitHandle = System.Threading.ThreadPool.RegisterWaitForSingleObject(
                timerEvent,
                callBack: waitOrTimeoutCallback,
                state: this,
                millisecondsTimeOutInterval: millisecondsTimeOutInterval,
                executeOnlyOnce: true);

            //registeredWaitHandle?.Unregister(waitObject: timerEvent);

            return Task.CompletedTask;
        }
    }
}
