using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Behavior.Options
{
    /// <summary>
    /// Options used to configure an <see cref="AsyncInjectBehaviourPolicy"/>
    /// </summary>
    public class InjectBehaviourAsyncOptions
    {
        /// <summary>
        /// Behaviour Delegate to be executed
        /// </summary>
        public Func<Context, CancellationToken, Task> Behaviour { get; set; }

        /// <summary>
        /// Lambda to get injection rate between [0, 1]
        /// </summary>
        public Func<Context, CancellationToken, Task<Double>> InjectionRate { get; set; }

        /// <summary>
        /// Lambda to check if this policy is enabled in current context
        /// </summary>
        public Func<Context, CancellationToken, Task<bool>> Enabled { get; set; }
    }

    /// <inheritdoc/>
    public class InjectBehaviourAsyncBasicOptions : DefaultOptions
    {
        /// <summary>
        /// Behaviour Delegate to be executed without context
        /// </summary>
        public Func<Task> Behaviour { get; set; }
    }
}
