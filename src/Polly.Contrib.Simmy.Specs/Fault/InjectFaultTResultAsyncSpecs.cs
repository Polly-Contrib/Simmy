using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Polly;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Fault
{
    [Collection(Constants.AmbientContextDependentTestCollection)]
    public class InjectFaultTResultAsyncSpecs : IDisposable
    {
        public InjectFaultTResultAsyncSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
        }

        public void Dispose()
        {
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        #region Basic Overload, Exception, Context Free
        [Fact]
        public void InjectFault_Context_Free_Enabled_Should_not_execute_user_delegate_async()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            Boolean executed = false;
            Func<Task<ResultPrimitive>> actionAsync = () => { executed = true; return Task.FromResult(ResultPrimitive.Good); };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, 0.6, () => true);
            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync))
                .ShouldThrowExactly<Exception>()
                .WithMessage(exceptionMessage);
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_Context_Free_Enabled_Should_execute_user_delegate_async()
        {
            string exceptionMessage = "exceptionMessage";
            Exception fault = new Exception(exceptionMessage);
            Boolean executed = false;
            Func<Task<ResultPrimitive>> actionAsync = () => { executed = true; return Task.FromResult(ResultPrimitive.Good); };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, 0.3, () => true);
            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync))
                .ShouldNotThrow<Exception>();
            executed.Should().BeTrue();
        }
        #endregion

        #region Basic Overload, Exception, With Context
        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_async()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true; return Task.FromResult(ResultPrimitive.Good);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(
                new Exception(),
                0.6,
                async (ctx, ct) =>
                {
                    return await Task.FromResult((bool)ctx["ShouldFail"]);
                });

            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context)).ShouldThrowExactly<Exception>();
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_execute_user_delegate_async()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true; return Task.FromResult(ResultPrimitive.Good);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(
                new Exception(),
                0.4,
                async (ctx, ct) =>
                {
                    return await Task.FromResult((bool)ctx["ShouldFail"]);
                });

            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }

        [Fact]
        public void InjectFault_With_Context_Should_execute_user_delegate_async_with_enabled_lambda_return_false()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = false;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true; return Task.FromResult(ResultPrimitive.Good);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(
                new Exception(),
                0.4,
                async (ctx, ct) =>
                {
                    return await Task.FromResult((bool)ctx["ShouldFail"]);
                });

            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context))
                .ShouldNotThrow<Exception>();

            executed.Should().BeTrue();
        }
        #endregion

        #region Overload, All based on context
        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_async_with_default_values()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<Exception>> fault = (ctx, cts) => Task.FromResult(new Exception());
            Func<Context, CancellationToken, Task<double>> injectionRate = (ctx, ct) => Task.FromResult(0.6);
            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) => Task.FromResult(true);
            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);
            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context))
                .ShouldThrowExactly<Exception>();
            executed.Should().BeFalse();
        }

        [Fact]
        public void InjectFault_With_Context_Should_not_execute_user_delegate_async_with_all_values_set()
        {
            string failureMessage = "Failure Message";
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<Exception>> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return Task.FromResult(ex);
                }

                return Task.FromResult(new Exception());
            };

            Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return Task.FromResult(rate);
            };

            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
            {
                return Task.FromResult((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);
            policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context))
                .ShouldThrowExactly<InvalidOperationException>();
            executed.Should().BeFalse();
        }

        #endregion

        #region TResult Based Monkey Policies
        [Fact]
        public async Task InjectFault_Context_Free_Should_Return_Fault_async()
        {
            Boolean executed = false;
            Func<Task<ResultPrimitive>> actionAsync = () =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            ResultPrimitive fault = ResultPrimitive.Fault;

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, 0.6, () => true);
            ResultPrimitive response = await policy.ExecuteAsync(actionAsync);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public async Task InjectFault_Context_Free_Should_Not_Return_Fault_async()
        {
            Boolean executed = false;
            Func<Task<ResultPrimitive>> actionAsync = () =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            ResultPrimitive fault = ResultPrimitive.Fault;

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, 0.4, () => true);
            ResultPrimitive response = await policy.ExecuteAsync(actionAsync);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }

        [Fact]
        public async Task InjectFault_With_Context_Enabled_Should_Return_Fault_async()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = true;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            ResultPrimitive fault = ResultPrimitive.Fault;
            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
            {
                return Task.FromResult((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, 0.6, enabled);
            ResultPrimitive response = await policy.ExecuteAsync(actionAsync, context);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public async Task InjectFault_With_Context_Enabled_Should_Not_Return_Fault_async()
        {
            Boolean executed = false;
            Context context = new Context();
            context["ShouldFail"] = false;
            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            ResultPrimitive fault = ResultPrimitive.Fault;
            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
            {
                return Task.FromResult((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, 0.6, enabled);
            ResultPrimitive response = await policy.ExecuteAsync(actionAsync, context);
            response.Should().Be(ResultPrimitive.Good);
            executed.Should().BeTrue();
        }

        [Fact]
        public async Task InjectFault_With_Context_InjectionRate_Should_Return_Fault_async()
        {
            Boolean executed = false;
            Context context = new Context();
            context["InjectionRate"] = 0.6;

            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<ResultPrimitive>> fault = (ctx, cts) =>
            {
                return Task.FromResult(ResultPrimitive.Fault);
            };

            Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return Task.FromResult(rate);
            };

            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
            {
                return Task.FromResult(true);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);
            ResultPrimitive response = await policy.ExecuteAsync(actionAsync, context);
            response.Should().Be(ResultPrimitive.Fault);
            executed.Should().BeFalse();
        }

        [Fact]
        public async Task InjectFault_With_Context_InjectionRate_Should_Not_Return_Fault_async()
        {
            Boolean executed = false;
            Context context = new Context();
            context["InjectionRate"] = 0.4;

            Func<Context, Task<ResultPrimitive>> actionAsync = (ctx) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<ResultPrimitive>> fault = (ctx, cts) =>
            {
                return Task.FromResult(ResultPrimitive.Fault);
            };

            Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return Task.FromResult(rate);
            };

            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
            {
                return Task.FromResult(true);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);
            ResultPrimitive response = await policy.ExecuteAsync(actionAsync, context);
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
            Context context = new Context();
            context["ShouldFail"] = true;
            context["Message"] = failureMessage;
            context["InjectionRate"] = 0.6;
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (ctx, ct) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<Exception>> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return Task.FromResult(ex);
                }

                return Task.FromResult(new Exception());
            };

            Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return Task.FromResult(rate);
            };

            Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
            {
                return Task.FromResult((bool)ctx["ShouldFail"]);
            };

            var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
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
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (ctx, ct) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<Exception>> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return Task.FromResult(ex);
                }

                return Task.FromResult(new Exception());
            };

            Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
            {
                double rate = 0;
                if (ctx["InjectionRate"] != null)
                {
                    rate = (double)ctx["InjectionRate"];
                }

                return Task.FromResult(rate);
            };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
                {
                    cts.Cancel();
                    return Task.FromResult((bool)ctx["ShouldFail"]);
                };

                var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
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
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (ctx, ct) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            Func<Context, CancellationToken, Task<Exception>> fault = (ctx, cts) =>
            {
                if (ctx["Message"] != null)
                {
                    Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                    return Task.FromResult(ex);
                }

                return Task.FromResult(new Exception());
            };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
                {
                    return Task.FromResult((bool)ctx["ShouldFail"]);
                };

                Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
                {
                    double rate = 0;
                    if (ctx["InjectionRate"] != null)
                    {
                        rate = (double)ctx["InjectionRate"];
                    }

                    cts.Cancel();
                    return Task.FromResult(rate);
                };

                var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
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
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (ctx, ct) =>
            {
                executed = true;
                return Task.FromResult(ResultPrimitive.Good);
            };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = (ctx, ct) =>
                {
                    return Task.FromResult((bool)ctx["ShouldFail"]);
                };

                Func<Context, CancellationToken, Task<Double>> injectionRate = (ctx, ct) =>
                {
                    double rate = 0;
                    if (ctx["InjectionRate"] != null)
                    {
                        rate = (double)ctx["InjectionRate"];
                    }

                    return Task.FromResult(rate);
                };

                Func<Context, CancellationToken, Task<Exception>> fault = (ctx, ct) =>
                {
                    cts.Cancel();
                    if (ctx["Message"] != null)
                    {
                        Exception ex = new InvalidOperationException(ctx["Message"].ToString());
                        return Task.FromResult(ex);
                    }

                    return Task.FromResult(new Exception());
                };

                var policy = MonkeyPolicy.InjectFaultAsync<ResultPrimitive>(fault, injectionRate, enabled);

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
        }

        #endregion
    }
}