using System;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Polly.Utilities;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;
using Polly.Timeout;

namespace Polly.Contrib.Simmy.Specs.Latency
{
    [Collection(Constants.AmbientContextDependentTestCollection)]
    [Obsolete]
    public class InjectLatencyTResultSpecs : IDisposable
    {
        private int _totalTimeSlept = 0;

        public InjectLatencyTResultSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
            SystemClock.Sleep = (span, ct) => _totalTimeSlept += span.Milliseconds;
        }

        public void Dispose()
        {
            _totalTimeSlept = 0;
            SystemClock.Reset();
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        #region Context Free

        [Fact]
        public void InjectLatency_Context_Free_Should_Introduce_Delay_If_Enabled()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, 0.6, () => true);
            var executed = false;

            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_Dissabled()
        {
            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(TimeSpan.FromMilliseconds(500), 0.6, () => false);
            var executed = false;

            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_Context_Free_Should_Introduce_Delay_If_InjectionRate_Is_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, 0.6, () => true);
            var executed = false;

            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, 0.3, () => true);
            var executed = false;

            Func<ResultPrimitive> action = () => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        #endregion

        #region With Context

        [Fact]
        public void InjectLatency_With_Context_With_Enabled_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, 0.6, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = false;

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, 0.6, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, 0.3, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["Enabled"] = true;
            context["InjectionRate"] = 0.3;

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(delay, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Latency_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = false;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
            result.Should().Be(ResultPrimitive.Good);
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.3;

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            var policy = MonkeyPolicy.InjectLatency<ResultPrimitive>(latencyProvider, injectionRate, enabled);
            Boolean executed = false;

            Func<Context, ResultPrimitive> action = (ctx) => { executed = true; return ResultPrimitive.Good; };
            var result = policy.Execute(action, context);

            executed.Should().BeTrue();
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

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return (bool)ctx["Enabled"];
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Boolean executed = false;
            Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };
            var policy = MonkeyPolicy.InjectLatency(latencyProvider, injectionRate, enabled);

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                policy.Invoking(x => x.Execute(action, context, cts.Token))
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

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
            {
                if (ctx["InjectionRate"] != null)
                {
                    return (double)ctx["InjectionRate"];
                }

                return 0;
            };

            Boolean executed = false;
            Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    cts.Cancel();
                    return (bool)ctx["Enabled"];
                };

                var policy = MonkeyPolicy.InjectLatency(latencyProvider, injectionRate, enabled);
                policy.Invoking(x => x.Execute(action, context, cts.Token))
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

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Boolean executed = false;
            Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    return (bool)ctx["Enabled"];
                };

                Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
                {
                    cts.Cancel();

                    if (ctx["InjectionRate"] != null)
                    {
                        return (double)ctx["InjectionRate"];
                    }

                    return 0;
                };

                var policy = MonkeyPolicy.InjectLatency(latencyProvider, injectionRate, enabled);

                policy.Invoking(x => x.Execute(action, context, cts.Token))
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
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    return (bool)ctx["Enabled"];
                };

                Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
                {
                    if (ctx["InjectionRate"] != null)
                    {
                        return (double)ctx["InjectionRate"];
                    }

                    return 0;
                };

                Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
                {
                    cts.Cancel();

                    if ((bool)ctx["ShouldInjectLatency"])
                    {
                        return delay;
                    }

                    return TimeSpan.FromMilliseconds(0);
                };

                var policy = MonkeyPolicy.InjectLatency(latencyProvider, injectionRate, enabled);
                Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

                policy.Invoking(x => x.Execute(action, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Theory]
        [InlineData(TimeoutStrategy.Optimistic)]
        [InlineData(TimeoutStrategy.Pessimistic)]
        public void InjectLatency_With_Context_Should_not_inject_the_whole_latency_if_user_cancelationtoken_is_signaled_from_timeout(TimeoutStrategy timeoutStrategy)
        {
            SystemClock.Reset();
            var timeout = TimeSpan.FromSeconds(5);
            var delay = TimeSpan.FromSeconds(10);
            var context = new Context();
            context["ShouldInjectLatency"] = true;
            context["Enabled"] = true;
            context["InjectionRate"] = 0.6;

            Boolean executed = false;
            Stopwatch watch = new Stopwatch();

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    return (bool)ctx["Enabled"];
                };

                Func<Context, CancellationToken, double> injectionRate = (ctx, ct) =>
                {
                    if (ctx["InjectionRate"] != null)
                    {
                        return (double)ctx["InjectionRate"];
                    }

                    return 0;
                };

                Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
                {
                    if ((bool)ctx["ShouldInjectLatency"])
                    {
                        return delay;
                    }

                    return TimeSpan.FromMilliseconds(0);
                };

                Func<Context, CancellationToken, ResultPrimitive> action = (ctx, ct) => { executed = true; return ResultPrimitive.Good; };

                var policy = Policy.Timeout(timeout, timeoutStrategy)
                    .Wrap(MonkeyPolicy.InjectLatency(latencyProvider, injectionRate, enabled));

                watch.Start();
                policy.Invoking(x => { x.Execute(action, context, cts.Token); })
                    .ShouldThrow<TimeoutRejectedException>();
                watch.Stop();
            }

            executed.Should().BeFalse();
            watch.Elapsed.Should().BeCloseTo(timeout, ((int)TimeSpan.FromSeconds(3).TotalMilliseconds));
        }

        #endregion
    }
}
