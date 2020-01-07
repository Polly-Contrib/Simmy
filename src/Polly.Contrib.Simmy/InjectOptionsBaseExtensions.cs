using Polly.Contrib.Simmy.Utilities;
using System;
using System.Threading;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Allows configuration of when and how often chaos behaviour is injected, for synchronous monkey policies.
    /// </summary>
    public static class InjectOptionsBaseExtensions
    {
        /// <summary>
        /// Configure that this monkey policy is enabled.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        public static InjectOptionsBase Enabled(this InjectOptionsBase options) => Enabled(options, true);

        /// <summary>
        /// Configure whether this monkey policy is enabled.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="enabled">A boolean value indicating whether the monkey policy is enabled.</param>
        public static InjectOptionsBase Enabled(this InjectOptionsBase options, bool enabled)
        {
            options.Enabled = (_, __) => enabled;
            return options;
        }

        /// <summary>
        /// Configure when this monkey policy is enabled.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="enabledWhen">A delegate which can be executed to determine whether the monkey policy should be enabled.</param>
        public static InjectOptionsBase EnabledWhen(this InjectOptionsBase options, Func<Context, CancellationToken, bool> enabledWhen)
        {
            options.Enabled = enabledWhen;
            return options;
        }

        /// <summary>
        /// Configure the rate at which this monkey policy should inject chaos.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="injectionRate">The injection rate between [0, 1]</param>
        public static InjectOptionsBase InjectionRate(this InjectOptionsBase options, Double injectionRate)
        {
            injectionRate.EnsureInjectionThreshold();
            options.InjectionRate = (_, __) => injectionRate;
            return options;
        }

        /// <summary>
        /// Configure the rate at which this monkey policy should inject chaos.
        /// </summary>
        /// <param name="options">The configuration object.</param>
        /// <param name="injectionRateProvider">A delegate returning the current rate at which this monkey policy should inject chaos, expressed as double between [0, 1]</param>
        public static InjectOptionsBase InjectionRate(this InjectOptionsBase options, Func<Context, CancellationToken, Double> injectionRateProvider)
        {
            options.InjectionRate = injectionRateProvider;
            return options;
        }
    }
}
