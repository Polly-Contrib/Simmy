﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Polly.Contrib.Simmy.Latency;
using Polly.Contrib.Simmy.Utilities;
using Polly.Utilities;
using Xunit;
using Polly.Timeout;

namespace Polly.Contrib.Simmy.Specs.Latency
{
    [Collection(Helpers.Constants.AmbientContextDependentTestCollection)]
    public class InjectLatencyAsyncWithOptionsSpecs : IDisposable
    {
        private int _totalTimeSlept = 0;

        public InjectLatencyAsyncWithOptionsSpecs()
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
            var executed = false;

            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .Enabled()
            );

            Func<Task> actionAsync = () => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_Dissabled()
        {
            var executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(TimeSpan.FromMilliseconds(500))
                    .InjectionRate(0.6)
                    .Enabled(false)
            );

            Func<Task> actionAsync = () => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Introduce_Delay_If_InjectionRate_Is_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .Enabled()
            );

            Func<Task> actionAsync = () => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_Context_Free_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(0.3)
                    .Enabled()
            );

            Func<Task> actionAsync = () => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        #endregion

        #region BeforeInject
        [Fact]
        public async Task Should_call_before_inject_callback_if_injecting()
        {
            var beforeInjectExecuted = false;
            var executed = false;

            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(TimeSpan.FromMilliseconds(1))
                    .BeforeInject(async (context, cancellation) => { beforeInjectExecuted = true; })
                    .InjectionRate(0.6)
                    .Enabled());

            await policy.ExecuteAsync(async () =>
            {
                beforeInjectExecuted.Should().BeTrue();
                executed = true;
            });
            executed.Should().BeTrue();
            beforeInjectExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Should_not_call_before_inject_callback_if_not_injecting()
        {
            var beforeInjectExecuted = false;
            var executed = false;

            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(TimeSpan.FromMilliseconds(1))
                    .BeforeInject(async (context, cancellation) => { beforeInjectExecuted = true; })
                    .InjectionRate(0.4)
                    .Enabled());

            await policy.ExecuteAsync(async () =>
            {
                beforeInjectExecuted.Should().BeFalse();
                executed = true;
            });
            executed.Should().BeTrue();
            beforeInjectExecuted.Should().BeFalse();
        }
        #endregion

        #region With Context

        [Fact]
        public async Task InjectLatency_With_Context_With_Enabled_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context { ["Enabled"] = true };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context { ["Enabled"] = false };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(0.6)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Enabled_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context { ["Enabled"] = true };

            Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
            {
                return await Task.FromResult((bool)ctx["Enabled"]);
            };

            Boolean executed = false;
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(0.3)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
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
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_InjectionRate_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["Enabled"] = true,
                ["InjectionRate"] = 0.3
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
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(delay)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Latency_Lambda_Should_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(delay.Milliseconds);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = false,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

            executed.Should().BeTrue();
            _totalTimeSlept.Should().Be(0);
        }

        [Fact]
        public async Task InjectLatency_With_Context_With_Latency_Lambda_Should_Not_Introduce_Delay_If_InjectionRate_Is_Not_Covered()
        {
            var delay = TimeSpan.FromMilliseconds(500);
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.3
            };

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
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

            Func<Context, Task> actionAsync = _ => { executed = true; return TaskHelper.EmptyTask; };
            await policy.ExecuteAsync(actionAsync, context);

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
            Func<Context, CancellationToken, Task> actionAsync = (_, ct) => { executed = true; return TaskHelper.EmptyTask; };
            var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                with.Latency(latencyProvider)
                    .InjectionRate(injectionRate)
                    .EnabledWhen(enabled)
            );

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
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

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
            Func<Context, CancellationToken, Task> actionAsync = (_, ct) => { executed = true; return TaskHelper.EmptyTask; };

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                Func<Context, CancellationToken, Task<bool>> enabled = async (ctx, ct) =>
                {
                    cts.Cancel();
                    return await Task.FromResult((bool)ctx["Enabled"]);
                };

                var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                    with.Latency(latencyProvider)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

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
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

            Func<Context, CancellationToken, Task<TimeSpan>> latencyProvider = async (ctx, ct) =>
            {
                if ((bool)ctx["ShouldInjectLatency"])
                {
                    return await Task.FromResult(delay);
                }

                return await Task.FromResult(TimeSpan.FromMilliseconds(0));
            };

            Boolean executed = false;
            Func<Context, CancellationToken, Task> actionAsync = (_, ct) => { executed = true; return TaskHelper.EmptyTask; };

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

                var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                    with.Latency(latencyProvider)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

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
            var context = new Context
            {
                ["ShouldInjectLatency"] = true,
                ["Enabled"] = true,
                ["InjectionRate"] = 0.6
            };

            Boolean executed = false;
            Func<Context, CancellationToken, Task> actionAsync = (_, ct) => { executed = true; return TaskHelper.EmptyTask; };

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

                var policy = MonkeyPolicy.InjectLatencyAsync(with =>
                    with.Latency(latencyProvider)
                        .InjectionRate(injectionRate)
                        .EnabledWhen(enabled)
                );

                policy.Awaiting(async x => await x.ExecuteAsync(actionAsync, context, cts.Token))
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
                    if ((bool)ctx["ShouldInjectLatency"])
                    {
                        return await Task.FromResult(delay);
                    }

                    return await Task.FromResult(TimeSpan.FromMilliseconds(0));
                };

                Func<Context, CancellationToken, Task> actionAsync = (_, ct) =>
                {
                    executed = true;
                    return TaskHelper.EmptyTask;
                };

                var policy = Policy.TimeoutAsync(timeout, timeoutStrategy)
                    .WrapAsync(
                        MonkeyPolicy.InjectLatencyAsync(with =>
                            with.Latency(latencyProvider)
                                .InjectionRate(injectionRate)
                                .EnabledWhen(enabled)
                        ));

                watch.Start();
                policy.Awaiting(async x => { await x.ExecuteAsync(actionAsync, context, cts.Token); })
                    .ShouldThrow<TimeoutRejectedException>();
                watch.Stop();
            }

            executed.Should().BeFalse();
            watch.Elapsed.Should().BeCloseTo(timeout, ((int)TimeSpan.FromSeconds(3).TotalMilliseconds));
        }

        #endregion
    }
}
