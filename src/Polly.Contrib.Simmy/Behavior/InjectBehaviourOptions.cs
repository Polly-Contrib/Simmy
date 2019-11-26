using System;
using System.Threading;
using Polly.Contrib.Simmy.Behavior;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectBehaviourPolicy"/>
    /// </summary>
    public class InjectBehaviourOptions : InjectOptionsBase
    {
        /// <summary>
        /// Behaviour Delegate to be executed
        /// </summary>
        internal Action<Context, CancellationToken> Behaviour { get; set; }
    }
}
