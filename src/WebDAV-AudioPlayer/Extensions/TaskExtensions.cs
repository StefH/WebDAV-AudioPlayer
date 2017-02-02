using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebDav.AudioPlayer.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);

            return await task;
        }

        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();
            var totalCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationTokenSource.Token, cancellationToken);

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, totalCancellationTokenSource.Token));
            if (completedTask == task)
            {
                totalCancellationTokenSource.Cancel();
                return await task;  // Very important in order to propagate exceptions
            }

            throw new TimeoutException("The operation has timed out.");
        }
    }
}