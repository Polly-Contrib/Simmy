using System;
using System.Threading;
using FluentAssertions;
using Polly.Contrib.Simmy.Utilities;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Fault
{
    [Collection(Helpers.Constants.AmbientContextDependentTestCollection)]
    public class InjectFaultSpecs : IDisposable
    {
        public InjectFaultSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
        }

        public void Dispose()
        {
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        #region Basic Overload, Exception, Context Free
        [Fact]
        public void InjectFault_Context_Free_Enabled_Should_not_execute_user_delegate()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            var policy = MonkeyPolicy.InjectFault(fault, 0.6, () => true);

            Boolean executed = false;
            policy.Invoking(x => x.Execute(() => { executed = true; }))
                .ShouldThrowExactly<Exception>().WithMessage(exceptionMessage);

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_Context_Free_Enabled_Should_execute_user_delegate()
        {
            Exception fault = new Exception("test");
            var policy = MonkeyPolicy.InjectFault(fault, 0.3, () => true);

            Boolean executed = false;
            policy.Invoking(x => x.Execute(() => { executed = true; }))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFault_Context_Free_Enabled_Should_execute_user_delegate_not_throw_if_injected_fault_is_permitted_null()
        {
            Exception fault = null;
            var policy = MonkeyPolicy.InjectFault(fault, 0.6, () => true);

            Boolean executed = false;
            policy.Invoking(x => x.Execute(() => { executed = true; }))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }

        #endregion

        #region Basic Overload, Exception, With Context
        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            var policy = MonkeyPolicy.InjectFault(
                new Exception("test"),
                0.6,
                (ctx, ct) =>
                {
                    return ((bool)ctx["ShouldFail"]);
                });

            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldThrowExactly<Exception>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_execute_user_delegate()
        {
            Exception fault = new Exception("test");
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            var policy = MonkeyPolicy.InjectFault(
                new Exception("test"),
                0.4,
                (ctx, ct) =>
                {
                    return ((bool)ctx["ShouldFail"]);
                });

            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFault_With_Context_Should_execute_user_delegate_with_enabled_lambda_return_false()
        {
            Exception fault = new Exception("test");
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = false;
            var policy = MonkeyPolicy.InjectFault(
                new Exception("test"),
                0.6,
                (ctx, ct) =>
                {
                    return ((bool)ctx["ShouldFail"]);
                });

            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }
        #endregion

        #region Overload, All based on context

        [Fact]
        public void InjectFault_should_throw_if_injection_rate_is_out_of_range_too_low()
        {
            Boolean executed = false;
            Context context = new Context();

            Func<Context, CancellationToken, Exception> fault = (ctx, ct) => new Exception();
            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) => -0.1;
            Func<Context, CancellationToken, bool> enabled = (ctx, ct) => true;
            var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);
            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldThrowExactly<ArgumentOutOfRangeException>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_should_throw_if_injection_rate_is_out_of_range_too_high()
        {
            Boolean executed = false;
            Context context = new Context();

            Func<Context, CancellationToken, Exception> fault = (ctx, ct) => new Exception();
            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) => 1.01;
            Func<Context, CancellationToken, bool> enabled = (ctx, ct) => true;
            var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);
            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldThrowExactly<ArgumentOutOfRangeException>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_with_default_context()
        {
            Boolean executed = false;
            Context context = new Context();

            Func<Context, CancellationToken, Exception> fault = (ctx, ct) => new Exception();
            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) => 0.6;
            Func<Context, CancellationToken, bool> enabled = (ctx, ct) => true;
            var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);
            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldThrowExactly<Exception>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_with_all_context_set()
        {
            Boolean executed = false;
            Context context = new Context();
            string failureMessage = "Failure Message";
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Exception> fault = (ctx, ct) =>
            {
                if (ctx["Message"] != null)
                {
                    return new InvalidOperationException(ctx["Message"].ToString());
                }

                return new Exception();
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);
            policy.Invoking(x => x.Execute((ctx) => { executed = true; }, context))
                .ShouldThrowExactly<InvalidOperationException>().WithMessage(failureMessage);

            executed.Should().BeFalse();
        }
        #endregion

        #region Cancellable scenarios

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_before_to_start_execution()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Exception> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return ex;
                }

                return new Exception();
            };

            Func<Context, CancellationToken, Double> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return rate;
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return (bool)ctx["ShouldFail"];
            };

            var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_enabled_config_delegate()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Exception> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return ex;
                }

                return new Exception();
            };

            Func<Context, CancellationToken, Double> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return rate;
            };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    cts.Cancel();
                    return (bool)ctx["ShouldFail"];
                };

                var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_injectionrate_config_delegate()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Exception> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return ex;
                }

                return new Exception();
            };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    return (bool)ctx["ShouldFail"];
                };

                Func<Context, CancellationToken, Double> injectionRate = (ctx, ct) =>
                {
                    double rate = 0;
                    if (ctx["InjectionRate"] != null)
                    {
                        rate = (double)ctx["InjectionRate"];
                    }

                    cts.Cancel();
                    return rate;
                };

                var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_fault_config_delegate()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    return (bool)ctx["ShouldFail"];
                };

                Func<Context, CancellationToken, Double> injectionRate = (ctx, ct) =>
                {
                    double rate = 0;
                    if (ctx["InjectionRate"] != null)
                    {
                        rate = (double)ctx["InjectionRate"];
                    }

                    return rate;
                };

                Func<Context, CancellationToken, Exception> fault = (ctx, ct) =>
                {
                    cts.Cancel();
                    if (ctx["Message"] != null)
                    {
                        Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                        return ex;
                    }

                    return new Exception();
                };

                var policy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        #endregion
    }
}