using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Outcomes
{
    /// <summary>
    /// Options used to configure an <see cref="InjectOutcomePolicy"/>
    /// </summary>
    public class InjectOutcomeAsyncOptions<TResult> : InjectOptionsAsyncBase
    {
        /// <summary>
        /// Outcome Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, Task<TResult>> Outcome { get; set; }
    }
}
