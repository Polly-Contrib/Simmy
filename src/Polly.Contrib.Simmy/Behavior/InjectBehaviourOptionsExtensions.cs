using System;
using System.Threading;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Allows configuration of behaviour for synchronous monkey behaviour-injection policies.
    /// </summary>
    public static class InjectBehaviourOptionsExtensions
    {
        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="behaviour">A delegate representing the behaviour to inject.</param>
        public static InjectBehaviourOptions Behaviour(this InjectBehaviourOptions options, Action behaviour) =>
            Behaviour(options, (_, __) => behaviour());

        /// <summary>
        /// Configure behaviour to inject with the monkey policy.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="behaviour">A delegate representing the behaviour to inject.</param>
        public static InjectBehaviourOptions Behaviour(this InjectBehaviourOptions options, Action<Context, CancellationToken> behaviour)
        {
            options.Behaviour = behaviour;
            return options;
        }
    }
}
