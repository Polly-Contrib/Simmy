using System;
using System.Threading;
using System.Threading.Tasks;
using Polly.Contrib.Simmy.Utilities;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Allows configuration of when and how often chaos behaviour is injected, for asynchronous monkey policies.
    /// </summary>
    public static class InjectOptionsAsyncBaseExtensions
    {
        /// <summary>
        /// Configure that this monkey policy is enabled.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        public static InjectOptionsAsyncBase Enabled(this InjectOptionsAsyncBase options) => Enabled(options, true);

        /// <summary>
        /// Configure whether this monkey policy is enabled.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="enabled">A boolean value indicating whether the monkey policy is enabled.</param>
        public static InjectOptionsAsyncBase Enabled(this InjectOptionsAsyncBase options, bool enabled)
        {
            options.Enabled = (_, __) => Task.FromResult(enabled);
            return options;
        }

        /// <summary>
        /// Configure when this monkey policy is enabled.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="enabledWhen">A delegate which can be executed to determine whether the monkey policy should be enabled.</param>
        public static InjectOptionsAsyncBase EnabledWhen(this InjectOptionsAsyncBase options, Func<Context, CancellationToken, Task<bool>> enabledWhen)
        {
            options.Enabled = enabledWhen;
            return options;
        }

        /// <summary>
        /// Configure the rate at which this monkey policy should inject chaos.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="injectionRate">The injection rate between [0, 1]</param>
        public static InjectOptionsAsyncBase InjectionRate(this InjectOptionsAsyncBase options, Double injectionRate)
        {
            injectionRate.EnsureInjectionThreshold();
            options.InjectionRate = (_, __) => Task.FromResult(injectionRate);
            return options;
        }

        /// <summary>
        /// Configure the rate at which this monkey policy should inject chaos.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="injectionRateProvider">A delegate returning the current rate at which this monkey policy should inject chaos, expressed as double between [0, 1]</param>
        public static InjectOptionsAsyncBase InjectionRate(this InjectOptionsAsyncBase options, Func<Context, CancellationToken, Task<Double>> injectionRateProvider)
        {
            options.InjectionRate = injectionRateProvider;
            return options;
        }
    }
}
