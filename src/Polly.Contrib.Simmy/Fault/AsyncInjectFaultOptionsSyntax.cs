﻿using System;
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
        /// Builds an <see cref="AsyncInjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectOutcomePolicy InjectFaultAsync(Action<InjectFaultAsyncOptions<Exception>> configureOptions)
        {
            var options = new InjectFaultAsyncOptions<Exception>();
            configureOptions.Invoke(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectOutcomePolicy(options);
        }

        /// <summary>
        /// Builds an <see cref="AsyncInjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectOutcomePolicy<TResult> InjectFaultAsync<TResult>(Action<InjectFaultAsyncOptions<TResult>> configureOptions)
        {
            var options = new InjectFaultAsyncOptions<TResult>();
            configureOptions.Invoke(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectOutcomePolicy<TResult>(options);
        }

        /// <summary>
        /// Builds an <see cref="AsyncInjectOutcomePolicy"/> which injects a fault if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static AsyncInjectOutcomePolicy<TResult> InjectFaultAsync<TResult>(Action<InjectFaultAsyncOptions<Exception>> configureOptions)
        {
            var options = new InjectFaultAsyncOptions<Exception>();
            configureOptions.Invoke(options);

            if (options.Outcome == null) throw new ArgumentNullException(nameof(options.Outcome));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new AsyncInjectOutcomePolicy<TResult>(options);
        }
    }
}
