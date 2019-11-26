using System;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Polly.Contrib.Simmy.Utilities;
using Polly.Utilities;
using Xunit;
using Polly.Timeout;

namespace Polly.Contrib.Simmy.Specs.Latency
{
    [Collection(Helpers.Constants.AmbientContextDependentTestCollection)]
    public class InjectLatencyWithOptionsSpecs : IDisposable
    {
        private int _totalTimeSlept = 0;

        public InjectLatencyWithOptionsSpecs()
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
            var executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .Enabled()
            );

            policy.Execute(() => { executed = true; });

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_Dissabled()
        {
            var executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(TimeSpan.FromMilliseconds(500))
                    .InjectionRate(0.6)
                    .Enabled(false)
            );

            policy.Execute(() => { executed = true; });

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_Context_Free_Should_Introduce_Delay_If_InjectionRate_Is_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .Enabled()
            );

            policy.Execute(() => { executed = true; });

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(0.3)
                    .Enabled()
            );

            policy.Execute(() => { executed = true; });

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        #endregion

        #region With Context

        [Fact]
        public void InjectLatency_With_Context_With_Enabled_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context { ["Enabled"] = true };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context { ["Enabled"] = false };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context { ["Enabled"] = true };

            Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
            {
                return ((bool)ctx["Enabled"]);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(0.3)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
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

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["Enabled"] = true,
                ["InjectionRate"] = 0.3
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

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(delay)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Latency_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = false,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.3
            };

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

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            policy.Execute((ctx) => { executed = true; }, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        #endregion

        #region Cancellable scenarios

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_before_to_start_execution()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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
            var policy = MonkeyPolicy.InjectLatency(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                cts.Cancel();

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_enabled_config_delegate()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, bool> enabled = (ctx, ct) =>
                {
                    cts.Cancel();
                    return (bool)ctx["Enabled"];
                };

                var policy = MonkeyPolicy.InjectLatency(with =>
                    with.Latency(latencyProvider)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );
                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_injectionrate_config_delegate()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

            Func<Context, CancellationToken, TimeSpan> latencyProvider = (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return delay;
                }

                return TimeSpan.FromMilliseconds(0);
            };

            Boolean executed = false;
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

                var policy = MonkeyPolicy.InjectLatency(with =>
                    with.Latency(latencyProvider)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
                    .ShouldThrow<OperationCanceledException>();
            }

            executed.Should().BeFalse();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public void InjectLatency_With_Context_Should_not_execute_user_delegate_if_user_cancelationtoken_cancelled_on_latency_config_delegate()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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

                var policy = MonkeyPolicy.InjectLatency(with =>
                    with.Latency(latencyProvider)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

                policy.Invoking(x => x.Execute((ctx, ct) => { executed = true; }, context, cts.Token))
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
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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

                var policy = Policy.Timeout(timeout, timeoutStrategy)
                    .Wrap(
                        MonkeyPolicy.InjectLatency(with =>
                            with.Latency(latencyProvider)
                                .InjectionRate(injectionRate)
                                .EnabledWhen(enabled)
                        ));

                watch.Start();
                policy.Invoking(x => { x.Execute((ctx, ct) => { executed = true; }, context, cts.Token); })
                    .ShouldThrow<TimeoutRejectedException>();
                watch.Stop();
            }

            executed.Should().BeFalse();
            watch.Elapsed.Should().BeCloseTo(timeout, ((int)TimeSpan.FromSeconds(3).TotalMilliseconds));
        }

        #endregion
    }
}
