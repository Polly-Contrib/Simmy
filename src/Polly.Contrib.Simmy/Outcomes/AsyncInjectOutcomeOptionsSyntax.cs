using System;
using Polly.Contrib.Simmy.Outcomes;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Fluent API for defining Monkey <see cref="Policy"/>. 
    /// </summary>
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds an <see cref="AsyncInjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectOutcomePolicy InjectExceptionAsync(Action<InjectOutcomeAsyncOptions<Exception>> configureOptions)
        {
            var options = new InjectOutcomeAsyncOptions<Exception>();
            configureOptions(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectOutcomePolicy(options);
        }

        /// <summary>
        /// Builds an <see cref="AsyncInjectOutcomePolicy"/> which injects a result if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectOutcomePolicy<TResult> InjectResultAsync<TResult>(Action<InjectOutcomeAsyncOptions<TResult>> configureOptions)
        {
            var options = new InjectOutcomeAsyncOptions<TResult>();
            configureOptions(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectOutcomePolicy<TResult>(options);
        }

        /// <summary>
        /// Builds an <see cref="AsyncInjectOutcomePolicy"/> which injects a fault as result if  <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectOutcomePolicy<TResult> InjectResultAsync<TResult>(Action<InjectOutcomeAsyncOptions<Exception>> configureOptions)
        {
            var options = new InjectOutcomeAsyncOptions<Exception>();
            configureOptions(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectOutcomePolicy<TResult>(options);
        }
    }
}
