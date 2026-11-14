using FinalShipCore.Runtime.Contracts.Loading;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FinalShipCore.Runtime.Core
{
    public class LoadingPipeline
    {
        public bool IsRunning { get; private set; }

        public ILoadingTask CurrentTask { get; private set; }

        public async Task ExecuteAsync(IReadOnlyList<ILoadingTask> tasks, CancellationToken cancellationToken)
        {
            if (tasks == null)
                throw new ArgumentException(nameof(tasks));

            IsRunning = true;

            try
            {
                foreach (var task in tasks)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    CurrentTask = task;

                    await task.ExecuteAsync(cancellationToken);
                }
            }
            finally
            {
                IsRunning = false;
                CurrentTask = null;
            }
        }
    }
}
