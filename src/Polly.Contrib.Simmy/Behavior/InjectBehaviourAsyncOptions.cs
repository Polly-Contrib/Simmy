using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Simmy.Behavior;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectBehaviourPolicy"/>
    /// </summary>
    public class InjectBehaviourAsyncOptions : InjectOptionsAsyncBase
    {
        /// <summary>
        /// Behaviour Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, Task> Behaviour { get; set; }
    }
}
