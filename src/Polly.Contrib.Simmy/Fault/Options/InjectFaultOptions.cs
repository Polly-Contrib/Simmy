using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Fault.Options
{
    /// <summary>
    /// Options used to configure an <see cref="InjectOutcomePolicy"/>
    /// </summary>
    public class InjectFaultOptions<TResult> : InjectOptionsBase
    {
        /// <summary>
        /// Outcome Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, TResult> Outcome { get; set; }
    }
}
