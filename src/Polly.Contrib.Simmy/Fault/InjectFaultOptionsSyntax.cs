using System;
using Polly.Contrib.Simmy.Fault;
using Polly.Contrib.Simmy.Fault.Options;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Fluent API for defining Monkey <see cref="Policy"/>. 
    /// </summary>
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectOutcomePolicy InjectFault(Action<InjectFaultOptions<Exception>> configureOptions)
        {
            var options = new InjectFaultOptions<Exception>();
            configureOptions.Invoke(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new InjectOutcomePolicy(options);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(Action<InjectFaultOptions<TResult>> configureOptions)
        {
            var options = new InjectFaultOptions<TResult>();
            configureOptions.Invoke(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new InjectOutcomePolicy<TResult>(options);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(Action<InjectFaultOptions<Exception>> configureOptions)
        {
            var options = new InjectFaultOptions<Exception>();
            configureOptions.Invoke(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new InjectOutcomePolicy<TResult>(options);
        }
    }
}
