using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Polly.Utilities;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Latency
{
    [Collection(Constants.AmbientContextDependentTestCollection)]
    public class InjectLatencyTResultAsyncSpecs : IDisposable
    {
        private int _totalTimeSlept = 0;

        public InjectLatencyTResultAsyncSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
            SystemClock.SleepAsync = async (span, ct) => _totalTimeSlept += await Task.FromResult(span.Milliseconds);
        }

        public void Dispose()
        {
            _totalTimeSlept = 0;
            SystemClock.Reset();
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        #region Context Free

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Introduce_Delay_If_Enabled()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, 0.6, () => true);
            var executed = false;

            Func<Task<ResultPrimitive>> actionAsync = () => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_Dissabled()
        {
            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(TimeSpan.FromMilliseconds(500), 0.6, () => false);
            var executed = false;

            Func<Task<ResultPrimitive>> actionAsync = () => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Introduce_Delay_If_InjectionRate_Is_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, 0.6, () => true);
            var executed = false;

            Func<Task<ResultPrimitive>> actionAsync = () => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, 0.3, () => true);
            var executed = false;

            Func<Task<ResultPrimitive>> actionAsync = () => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        #endregion

        #region With Context

        [Fact]
        public async Task InjectLatency_With_Context_With_Enabled_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, 0.6, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = false;

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, 0.6, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, 0.3, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double) ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;
            context["InjectionRate"] = 0.3;

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double) ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(delay, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Latency_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double) ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = false;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double) ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.3;

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double) ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, Task<ResultPrimitive>> actionAsync = _ => { executed = true; return Task.FromResult(ResultPrimitive.Good); };
            var result = await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        #endregion

        #region Cancellable scenarios

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_before_to_start_execution()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double)ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (_, ct) => { executed = true; return Task.FromResult(ResultPrimitive.Good); };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_enabled_config_delegate()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return await Task.FromResult((double)ctx["InjectionRate"]);
                }

                return await Task.FromResult(0);
            };

            Boolean executed = false;
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (_, ct) => { executed = true; return Task.FromResult(ResultPrimitive.Good); };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
                {
                    cts.Cancel();
                    return await Task.FromResult((bool)ctx["Enabled"]);
                };

                var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_injectionrate_config_delegate()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Boolean executed = false;
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (_, ct) => { executed = true; return Task.FromResult(ResultPrimitive.Good); };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
                {
                    return await Task.FromResult((bool)ctx["Enabled"]);
                };

                Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
                {
                    cts.Cancel();

                    if (ctx["InjectionRate"] != null)
                    {
                        return await Task.FromResult((double)ctx["InjectionRate"]);
                    }

                    return await Task.FromResult(0);
                };

                var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_latency_config_delegate()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Boolean executed = false;
            Func<Context, CancellationToken, Task<ResultPrimitive>> actionAsync = (_, ct) => { executed = true; return Task.FromResult(ResultPrimitive.Good); };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
                {
                    return await Task.FromResult((bool)ctx["Enabled"]);
                };

                Func<Context, CancellationToken, Task<double>> injectionRate = async (ctx, ct) =>
                {
                    if (ctx["InjectionRate"] != null)
                    {
                        return await Task.FromResult((double)ctx["InjectionRate"]);
                    }

                    return await Task.FromResult(0);
                };

                Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
                {
                    cts.Cancel();

                    if ((bool)ctx["ShouldInjectLatency"])
                    {
                        return await Task.FromResult(delay);
                    }

                    return await Task.FromResult(TimeSpan.FromMilliseconds(0));
                };

                var policy = MonkeyPolicy.InjectLatencyAsync<ResultPrimitive>(latencyProvider, injectionRate, enabled);

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        #endregion
    }
}
