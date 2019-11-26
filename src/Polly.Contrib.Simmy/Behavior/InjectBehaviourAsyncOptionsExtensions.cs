using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Allows configuration of behaviour for asynchronous monkey behaviour-injection policies.
    /// </summary>
    public static class InjectBehaviourAsyncOptionsExtensions
    {
        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="behaviour">A delegate representing the behaviour to inject.</param>
        public static InjectBehaviourAsyncOptions Behaviour(this InjectBehaviourAsyncOptions options, Func<Task> behaviour) =>
            Behaviour(options, (_, __) => behaviour());

        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="behaviour">A delegate representing the behaviour to inject.</param>
        public static InjectBehaviourAsyncOptions Behaviour(this InjectBehaviourAsyncOptions options, Func<Context, CancellationToken, Task> behaviour)
        {
            options.Behaviour = behaviour;
            return options;
        }
    }
}
