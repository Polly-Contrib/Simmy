﻿using Polly.Contrib.Simmy.Behavior.Options;
using System;
using System.Threading;
using Polly.Contrib.Simmy.Behavior;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Fluent API for defining Monkey <see cref="Policy"/>. 
    /// </summary>
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds a <see cref="InjectBehaviourPolicy"/> which executes a behaviour if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectBehaviourPolicy InjectBehaviour(Action<InjectBehaviourOptions> configureOptions)
        {
            var options = new InjectBehaviourOptions();
            configureOptions.Invoke(options);

            if (options.Behaviour == null) throw new ArgumentNullException(nameof(options.Behaviour));
            if (options.InjectionRate == null) throw new ArgumentNullException(nameof(options.InjectionRate));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            return new InjectBehaviourPolicy(
                options.Behaviour,
                options.InjectionRate,
                options.Enabled);
        }

        /// <summary>
        /// Builds a <see cref="InjectBehaviourPolicy"/> which executes a behaviour if <paramref name="configureOptions.Enabled"/> returns true and
        /// a random number is within range of <paramref name="configureOptions.InjectionRate"/>.
        /// </summary>
        /// <param name="configureOptions">A callback to configure policy options.</param>
        /// <returns>The policy instance.</returns>
        public static InjectBehaviourPolicy InjectBehaviour(Action<InjectBehaviourBasicOptions> configureOptions)
        {
            var options = new InjectBehaviourBasicOptions();
            configureOptions.Invoke(options);

            if (options.Behaviour == null) throw new ArgumentNullException(nameof(options.Behaviour));
            if (options.Enabled == null) throw new ArgumentNullException(nameof(options.Enabled));

            void BehaviourLambda(Context _, CancellationToken __) => options.Behaviour();
            double InjectionRateLambda(Context _, CancellationToken __) => options.InjectionRate;
            bool EnabledLambda(Context _, CancellationToken __) => options.Enabled();

            return new InjectBehaviourPolicy(
                BehaviourLambda,
                InjectionRateLambda,
                EnabledLambda);
        }
    }
}
