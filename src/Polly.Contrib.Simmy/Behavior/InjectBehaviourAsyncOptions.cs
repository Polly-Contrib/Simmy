using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Behavior
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectBehaviourPolicy"/>
    /// </summary>
    public class InjectBehaviourAsyncOptions : InjectOptionsAsyncBase
    {
        /// <summary>
        /// Behaviour Delegate to be executed
        /// </summary>
        internal Func<Context, CancellationToken, Task> BehaviourInternal { get; set; }
    }
}
