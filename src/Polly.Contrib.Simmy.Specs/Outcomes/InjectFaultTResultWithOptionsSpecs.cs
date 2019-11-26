using System;
using System.Threading;
using FluentAssertions;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Outcomes
{
    [Collection(Constants.AmbientContextDependentTestCollection)]
    public class InjectFaultTResultWithOptionsSpecs : IDisposable
    {
        public InjectFaultTResultWithOptionsSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
        }

        public void Dispose()
        {
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        #region Basic Overload, Result as Fault, Context Free
        [Fact]
        public void InjectFaultContext_Free_Enabled_Should_not_execute_user_delegate()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault<ResultPrimitive>(fault)
                    .InjectionRate(0.6)
                    .Enabled()
            );

            policy.Invoking(x => x.Execute(action))
                .ShouldThrowExactly<Exception>().WithMessage(exceptionMessage);

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultContext_Free_Enabled_Should_execute_user_delegate()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault<ResultPrimitive>(fault)
                    .InjectionRate(0.3)
                    .Enabled()
            );

            policy.Invoking(x => x.Execute(action))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }
        #endregion

        #region Basic Overload, Result as Fault, With Context
        [Fact]
        public void InjectFaultWith_Context_Should_not_execute_user_delegate()
        {
            Boolean executed = false;
            Context context = new Context { ["ShouldFail"] = true };
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault(new Exception())
                    .InjectionRate(0.6)
                    .EnabledWhen((ctx, ct) => ((bool)ctx["ShouldFail"]))
            );

            policy.Invoking(x => x.Execute(action, context))
                .ShouldThrowExactly<Exception>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_Should_execute_user_delegate()
        {
            Boolean executed = false;
            Context context = new Context { ["ShouldFail"] = true };
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault(new Exception())
                    .InjectionRate(0.4)
                    .EnabledWhen((ctx, ct) => ((bool)ctx["ShouldFail"]))
            );

            policy.Invoking(x => x.Execute(action, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFaultWith_Context_Should_execute_user_delegate_with_enabled_lambda_disabled()
        {
            Boolean executed = false;
            Context context = new Context { ["ShouldFail"] = false };
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault(new Exception())
                    .InjectionRate(0.6)
                    .EnabledWhen((ctx, ct) => ((bool)ctx["ShouldFail"]))
            );

            policy.Invoking(x => x.Execute(action, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }
        #endregion

        #region Overload, All based on context
        [Fact]
        public void InjectFaultWith_Context_Should_not_execute_user_delegate_default_context()
        {
            Boolean executed = false;
            Context context = new Context();
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

            Func<Context, CancellationToken, Exception> fault = (ctx, ct) => new Exception();
            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) => 0.6;
            Func<Context, CancellationToken, bool> enabled = (ctx, ct) => true;

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault<ResultPrimitive>(fault)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Invoking(x => x.Execute(action, context))
                .ShouldThrow<Exception>();

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_Should_not_execute_user_delegate_full_context()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context
            {
                ["ShouldFail"] = true,
                ["Message"] = failureMessage,
                ["InjectionRate"] = 0.6
            };
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };

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

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Fault(fault)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Invoking(x => x.Execute(action, context))
                .ShouldThrowExactly<InvalidOperationException>();

            executed.Should().BeFalse();
        }
        #endregion

        #region TResult Based Monkey Policies
        [Fact]
        public void InjectFaultContext_Free_Should_Return_Fault()
        {
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Result(fault)
                    .InjectionRate(0.6)
                    .Enabled()
            );

            ResultPrimitive response = policy.Execute(action);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultContext_Free_Should_Not_Return_Fault()
        {
            Boolean executed = false;
            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Result(fault)
                    .InjectionRate(0.4)
                    .Enabled()
            );

            ResultPrimitive response = policy.Execute(action);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFaultWith_Context_Enabled_Should_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context { ["ShouldFail"] = true };
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;
            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Result(fault)
                    .InjectionRate(0.6)
                    .EnabledWhen(enabled)
            );

            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_Enabled_Should_Not_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context { ["ShouldFail"] = false };
            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            ResultPrimitive fault = ResultPrimitive.Fault;
            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Result(fault)
                    .InjectionRate(0.4)
                    .EnabledWhen(enabled)
            );

            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFaultWith_Context_InjectionRate_Should_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context { ["InjectionRate"] = 0.6 };

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            Func<Context, CancellationToken, ResultPrimitive> fault = (ctx, ct) =>
            {
                return ResultPrimitive.Fault;
            };

            Func<Context, CancellationToken, Double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return true;
            };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Result(fault)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFaultWith_Context_InjectionRate_Should_Not_Return_Fault()
        {
            Boolean executed = false;
            Context context = new Context();
            context["InjectionRate"] = 0.4;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            Func<Context, CancellationToken, ResultPrimitive> fault = (ctx, ct) =>
            {
                return ResultPrimitive.Fault;
            };

            Func<Context, CancellationToken, Double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return true;
            };

            var policy = MonkeyPolicy.InjectResult<ResultPrimitive>(with =>
                with.Result(fault)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            ResultPrimitive response = policy.Execute(action, context);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }
        #endregion

        #region Cancellable scenarios

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_before_to_start_execution()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context
            {
                ["ShouldFail"] = true, ["Message"] = failureMessage, ["InjectionRate"] = 0.6
            };

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

            Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

            var policy = MonkeyPolicy.InjectException(with =>
                with.Fault(fault)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                policy.Invoking(x => x.Execute((ctx, ct) => action, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_enabled_config_delegate()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context
            {
                ["ShouldFail"] = true, ["Message"] = failureMessage, ["InjectionRate"] = 0.6
            };

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

            Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    cts.Cancel();
                    return (bool)ctx["ShouldFail"];
                };

                var policy = MonkeyPolicy.InjectException(with =>
                    with.Fault(fault)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

                policy.Invoking(x => x.Execute(action, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_injectionrate_config_delegate()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context
            {
                ["ShouldFail"] = true, ["Message"] = failureMessage, ["InjectionRate"] = 0.6
            };

            Func<Context, CancellationToken, Exception> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return ex;
                }

                return new Exception();
            };

            Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

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

                var policy = MonkeyPolicy.InjectException(with =>
                    with.Fault(fault)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

                policy.Invoking(x => x.Execute(action, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_fault_config_delegate()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context
            {
                ["ShouldFail"] = true, ["Message"] = failureMessage, ["InjectionRate"] = 0.6
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

                Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

                var policy = MonkeyPolicy.InjectException(with =>
                    with.Fault(fault)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

                policy.Invoking(x => x.Execute(action, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        #endregion
    }
}
