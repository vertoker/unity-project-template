using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Shared.Async
{
    /// <summary>
    /// Cancel-and-restart wrapper around an async action taking 3 arguments: every call aborts the one still
    /// in flight, then optionally waits <c>delay</c> seconds before invoking. That delay is what turns
    /// it into a debounce — a burst of calls runs the action once, for the last arguments
    /// </summary>
    public class AsyncRunner<T1, T2, T3> : IDisposable
    {
        private readonly Func<T1, T2, T3, CancellationToken, UniTask> _action;
        private readonly float _delay;

        private CancellationTokenSource _cts = new();
        private bool _disposed;

        /// <summary>
        /// <paramref name="delay"/> of zero invokes the action immediately; anything above it debounces
        /// </summary>
        public AsyncRunner(Func<T1, T2, T3, CancellationToken, UniTask> action, float delay = 0f)
        {
            _action = action;
            _delay = delay;
        }

        /// <summary>
        /// Aborts the call in flight, if any, and leaves the runner usable
        /// </summary>
        public void Cancel()
        {
            if (_disposed) return;
            _cts.Cancel();
        }

        /// <summary>
        /// Aborts the call in flight and makes every further call a no-op
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cts.Cancel();
            _cts.Dispose();
        }

        /// <summary>
        /// Fire-and-forget form: failures surface through UniTask's unhandled exception handler
        /// </summary>
        public void CallAsync(T1 arg1, T2 arg2, T3 arg3)
        {
            CallTaskAsync(arg1, arg2, arg3).Forget();
        }

        /// <summary>
        /// Awaitable form: completes when the action finishes, or right away when a newer call cancels this one
        /// </summary>
        public async UniTask CallTaskAsync(T1 arg1, T2 arg2, T3 arg3)
        {
            var token = Restart();
            if (token.IsCancellationRequested) return;

            try
            {
                if (_delay > 0f)
                    await UniTask.WaitForSeconds(_delay, cancellationToken: token);
                await _action(arg1, arg2, arg3, token);
            }
            catch (OperationCanceledException)
            {
                // A newer call or Cancel() took over — the expected way a run ends, not a failure
            }
        }

        /// <summary>
        /// Cancels the previous run and hands out the token of the new one.
        /// The token is captured once: reading the field after an await could already see the next run's source
        /// </summary>
        private CancellationToken Restart()
        {
            if (_disposed) return new CancellationToken(true);

            var previous = _cts;
            _cts = new CancellationTokenSource();

            // Cancel before Dispose, so a token still held by the previous run stays usable
            previous.Cancel();
            previous.Dispose();

            return _cts.Token;
        }
    }
}
