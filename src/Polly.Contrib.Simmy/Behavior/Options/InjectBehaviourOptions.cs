using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Behavior.Options
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectBehaviourPolicy"/>
    /// </summary>
    public class InjectBehaviourOptions
    {
        /// <summary>
        /// Behaviour Delegate to be executed
        /// </summary>
        public Action<Context, CancellationToken> Behaviour { get; set; }

        /// <summary>
        /// Lambda to get injection rate between [0, 1]
        /// </summary>
        public Func<Context, CancellationToken, Double> InjectionRate{ get; set; }

        /// <summary>
        /// Lambda to check if this policy is enabled in current context
        /// </summary>
        public Func<Context, CancellationToken, bool> Enabled{ get; set; }
    }

    /// <inheritdoc/>
    public class InjectBehaviourBasicOptions : DefaultOptions
    {
        /// <summary>
        /// Behaviour Delegate to be executed without context
        /// </summary>
        public Action Behaviour { get; set; }
    }
}
