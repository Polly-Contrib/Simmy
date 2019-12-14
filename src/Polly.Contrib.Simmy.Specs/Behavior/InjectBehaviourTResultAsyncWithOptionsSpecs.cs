using System;
using System.Threading.Tasks;
using Polly.Contrib.Simmy.Behavior;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;
using FluentAssertions;

namespace Polly.Contrib.Simmy.Specs.Behavior
{
    [Collection(Helpers.Constants.AmbientContextDependentTestCollection)]
    public class InjectBehaviourTResultAsyncWithOptionsSpecs : IDisposable
    {
        public InjectBehaviourTResultAsyncWithOptionsSpecs()
        {
            ThreadSafeRandom_LockOncePerThread.NextDouble = () => 0.5;
        }

        public void Dispose()
        {
            ThreadSafeRandom_LockOncePerThread.Reset();
        }

        [Fact]
        public void Given_not_enabled_should_not_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() =>
                    {
                        injectedBehaviourExecuted = true;
                        return Task.CompletedTask;
                    })
                    .InjectionRate(0.6)
                    .Enabled(false)
                );

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.FromResult(ResultPrimitive.Good); });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeFalse();
        }

        [Fact]
        public void Given_enabled_and_randomly_within_threshold_should_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() =>
                    {
                        injectedBehaviourExecuted = true;
                        return Task.CompletedTask;
                    })
                    .InjectionRate(0.6)
                    .Enabled()
            );

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.FromResult(ResultPrimitive.Good); });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeTrue();
        }

        [Fact]
        public void Given_enabled_and_randomly_not_within_threshold_should_not_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() =>
                    {
                        injectedBehaviourExecuted = true;
                        return Task.CompletedTask;
                    })
                    .InjectionRate(0.4)
                    .Enabled(false)
            );

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.FromResult(ResultPrimitive.Good); });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeFalse();
        }

        [Fact]
        public void Should_inject_behaviour_before_executing_user_delegate()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() =>
                    {
                        userDelegateExecuted.Should().BeFalse(); // Not yet executed at the time the injected behaviour runs.
                        injectedBehaviourExecuted = true;
                        return Task.CompletedTask;
                    })
                    .InjectionRate(0.6)
                    .Enabled()
            );

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.FromResult(ResultPrimitive.Good); });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeTrue();
        }

        #region invalid threshold on configuration and execution time

        [Fact]
        public void Should_throw_error_on_configuration_time_when_threshold_is_negative()
        {
            Action act = () => MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() => Task.CompletedTask)
                    .Enabled()
                    .InjectionRate(-1)
            );

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_error_on_configuration_time_when_threshold_is_greater_than_one()
        {
            Action act = () => MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() => Task.CompletedTask)
                    .Enabled()
                    .InjectionRate(1.1)
            );

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_error_on_execution_time_when_threshold_is_is_negative()
        {
            var policy = MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() => Task.CompletedTask)
                    .Enabled()
                    .InjectionRate((_, __) => Task.FromResult(-1d))
            );

            policy.Awaiting(async x => await x.ExecuteAsync(() => Task.FromResult(ResultPrimitive.Good)))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_error_on_execution_time_when_threshold_is_greater_than_one()
        {
            var policy = MonkeyPolicy.InjectBehaviourAsync<ResultPrimitive>(with =>
                with.Behaviour(() => Task.CompletedTask)
                    .Enabled()
                    .InjectionRate((_, __) => Task.FromResult(1.1))
            );

            policy.Awaiting(async x => await x.ExecuteAsync(() => Task.FromResult(ResultPrimitive.Good)))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        #endregion
    }
}
