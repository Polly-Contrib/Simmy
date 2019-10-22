using System;
using System.Threading;
using Polly.Contrib.Simmy.Fault;

namespace Polly.Contrib.Simmy
{
    public partial class MonkeyPolicy
    {
        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="fault">The fault exception object to throw</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free fashion</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy InjectFault(
            Exception fault,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            Exception FaultLambda(Context _, CancellationToken __) => fault;
            double InjectionRateLambda(Context _, CancellationToken __) => injectionRate;
            bool EnabledLambda(Context _, CancellationToken __) => enabled();

            return new InjectOutcomePolicy(FaultLambda, InjectionRateLambda, EnabledLambda);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="fault">The fault exception object to throw</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy InjectFault(
            Exception fault,
            Double injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            Exception FaultLambda(Context _, CancellationToken __) => fault;
            double InjectionRateLambda(Context _, CancellationToken __) => injectionRate;

            return new InjectOutcomePolicy(FaultLambda, InjectionRateLambda, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="faultProvider">lambda to get the fault exception object</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy InjectFault(
            Func<Context, CancellationToken, Exception> faultProvider,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (faultProvider == null) throw new ArgumentNullException(nameof(faultProvider));
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectOutcomePolicy(faultProvider, injectionRate, enabled);
        }
    }

    public partial class MonkeyPolicy
    {
        #region Exception Based Faults
        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="fault">The fault exception object to throw</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free fashion</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(
            Exception fault,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            Exception FaultLambda(Context _, CancellationToken __) => fault;
            double InjectionRateLambda(Context _, CancellationToken __) => injectionRate;
            bool EnabledLambda(Context _, CancellationToken __) => enabled();

            return new InjectOutcomePolicy<TResult>((Func<Context, CancellationToken, Exception>)FaultLambda, InjectionRateLambda, EnabledLambda);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="fault">The fault exception object to throw</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(
            Exception fault,
            Double injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            Exception FaultLambda(Context _, CancellationToken __) => fault;
            double InjectionRateLambda(Context _, CancellationToken __) => injectionRate;

            return new InjectOutcomePolicy<TResult>((Func<Context, CancellationToken, Exception>)FaultLambda, InjectionRateLambda, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which executes a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="faultProvider">lambda to get the fault exception object</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(
            Func<Context, CancellationToken, Exception> faultProvider,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (faultProvider == null) throw new ArgumentNullException(nameof(faultProvider));
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectOutcomePolicy<TResult>(faultProvider, injectionRate, enabled);
        }

        #endregion

        #region TResult Based Faults

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="fault">The fault result object to inject</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in context free fashion</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(
            TResult fault,
            Double injectionRate,
            Func<bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            TResult FaultLambda(Context _, CancellationToken __) => fault;
            double InjectionRateLambda(Context _, CancellationToken __) => injectionRate;
            bool EnabledLambda(Context _, CancellationToken __) => enabled();

            return new InjectOutcomePolicy<TResult>((Func<Context, CancellationToken, TResult>)FaultLambda, InjectionRateLambda, EnabledLambda);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which injects a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="fault">The fault result object to inject</param>
        /// <param name="injectionRate">injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(
            TResult fault,
            Double injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            TResult FaultLambda(Context _, CancellationToken __) => fault;
            double InjectionRateLambda(Context _, CancellationToken __) => injectionRate;

            return new InjectOutcomePolicy<TResult>((Func<Context, CancellationToken, TResult>)FaultLambda, InjectionRateLambda, enabled);
        }

        /// <summary>
        /// Builds an <see cref="InjectOutcomePolicy"/> which executes a fault if <paramref name="enabled"/> returns true and
        /// a random number is within range of <paramref name="injectionRate"/>.
        /// </summary>
        /// <param name="faultProvider">The fault result object to inject</param>
        /// <param name="injectionRate">lambda to get injection rate between [0, 1]</param>
        /// <param name="enabled">Lambda to check if this policy is enabled in current context</param>
        /// <returns>The policy instance.</returns>
        [Obsolete("This overload is going to be deprecated, use the overload which takes Action<InjectFaultOptions> instead.")]
        public static InjectOutcomePolicy<TResult> InjectFault<TResult>(
            Func<Context, CancellationToken, TResult> faultProvider,
            Func<Context, CancellationToken, Double> injectionRate,
            Func<Context, CancellationToken, bool> enabled)
        {
            if (faultProvider == null) throw new ArgumentNullException(nameof(faultProvider));
            if (injectionRate == null) throw new ArgumentNullException(nameof(injectionRate));
            if (enabled == null) throw new ArgumentNullException(nameof(enabled));

            return new InjectOutcomePolicy<TResult>(faultProvider, injectionRate, enabled);
        }

        #endregion
    }
}
