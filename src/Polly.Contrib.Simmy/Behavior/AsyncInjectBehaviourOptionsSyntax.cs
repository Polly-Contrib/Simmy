using Polly.Contrib.Simmy.Behavior.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Simmy.Behavior;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Fluent API for defining Monkey <see cref="Policy"/>. 
    /// </summary>
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds a <see cref="AsyncInjectBehaviourPolicy"/> which executes a behaviour if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectBehaviourPolicy InjectBehaviourAsync(Action<InjectBehaviourAsyncOptions> configureOptions)
        {
            var options = new InjectBehaviourAsyncOptions();
            configureOptions.Invoke(options);

            if (options.Behaviour == null) throw new ArgumentNullException(nameof(options.Behaviour));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectBehaviourPolicy(
                options.Behaviour,
                options.InjectionRate,
                options.Enabled);
        }
    }
}
