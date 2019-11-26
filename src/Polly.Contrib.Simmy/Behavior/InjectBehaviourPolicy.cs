﻿using System;
using System.Threading;

namespace Polly.Contrib.Simmy.Behavior
{
    /// <summary>
    /// A policy that injects any custom behaviour before the execution of delegates.
    /// </summary>
    public class InjectBehaviourPolicy : Simmy.MonkeyPolicy
    {
        private readonly Action<Context, CancellationToken> _behaviour;

        [Obsolete]
        internal InjectBehaviourPolicy(Action<Context, CancellationToken> behaviour, Func<Context, CancellationToken, double> injectionRate, Func<Context, CancellationToken, bool> enabled) 
            : base(injectionRate, enabled)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
        }

        internal InjectBehaviourPolicy(InjectBehaviourOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _behaviour = options.Behaviour ?? throw new ArgumentNullException(nameof(options.Behaviour));
        }

        /// <inheritdoc/>
        protected override TResult Implementation<TResult>(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
        {
            return MonkeyEngine.InjectBehaviourImplementation(
                action,
                context,
                cancellationToken,
                (ctx, ct) => _behaviour(ctx, ct),
                InjectionRate,
                Enabled);
        }
    }
    /// <summary>
    /// A policy that injects any custom behaviour before the execution of delegates returning <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of return values this policy will handle.</typeparam>
    public class InjectBehaviourPolicy<TResult> : MonkeyPolicy<TResult>
    {
        private readonly Action<Context, CancellationToken> _behaviour;

        [Obsolete]
        internal InjectBehaviourPolicy(Action<Context, CancellationToken> behaviour, Func<Context, CancellationToken, double> injectionRate, Func<Context, CancellationToken, bool> enabled)
            : base(injectionRate, enabled)
        {
            _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
        }

        internal InjectBehaviourPolicy(InjectBehaviourOptions options)
            : base(options.InjectionRate, options.Enabled)
        {
            _behaviour = options.Behaviour ?? throw new ArgumentNullException(nameof(options.Behaviour));
        }

        /// <inheritdoc/>
        protected override TResult Implementation(Func<Context, CancellationToken, TResult> action, Context context, CancellationToken cancellationToken)
        {
            return MonkeyEngine.InjectBehaviourImplementation(
                action,
                context,
                cancellationToken,
                (ctx, ct) => _behaviour(ctx, ct),
                InjectionRate,
                Enabled);
        }
    }
}
