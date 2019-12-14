using System;
using FluentAssertions;
using Polly.Contrib.Simmy.Behavior;
using Polly.Contrib.Simmy.Specs.Helpers;
using Polly.Contrib.Simmy.Utilities;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Behavior
{
    [Collection(Helpers.Constants.AmbientContextDependentTestCollection)]
    public class InjectBehaviourTResultWithOptionsSpecs : IDisposable
    {
        public InjectBehaviourTResultWithOptionsSpecs()
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

            var policy = MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { injectedBehaviourExecuted = true; })
                    .InjectionRate(0.6)
                    .Enabled(false));

            policy.Execute(() => { userDelegateExecuted = true; return ResultPrimitive.Good; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeFalse();
        }

        [Fact]
        public void Given_enabled_and_randomly_within_threshold_should_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { injectedBehaviourExecuted = true; })
                    .InjectionRate(0.6)
                    .Enabled());

            policy.Execute(() => { userDelegateExecuted = true; return ResultPrimitive.Good; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeTrue();
        }

        [Fact]
        public void Given_enabled_and_randomly_not_within_threshold_should_not_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { injectedBehaviourExecuted = true; })
                    .InjectionRate(0.4)
                    .Enabled());

            policy.Execute(() => { userDelegateExecuted = true; return ResultPrimitive.Good; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeFalse();
        }

        [Fact]
        public void Should_inject_behaviour_before_executing_user_delegate()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() =>
                    {
                        userDelegateExecuted.Should().BeFalse(); // Not yet executed at the time the injected behaviour runs.
                        injectedBehaviourExecuted = true;
                    })
                    .InjectionRate(0.6)
                    .Enabled());

            policy.Execute(() => { userDelegateExecuted = true; return ResultPrimitive.Good; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeTrue();
        }

        #region invalid threshold on configuration and execution time

        [Fact]
        public void Should_throw_error_on_configuration_time_when_threshold_is_negative()
        {
            Action act = () => MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { })
                    .Enabled()
                    .InjectionRate(-1)
            );

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_error_on_configuration_time_when_threshold_is_greater_than_one()
        {
            Action act = () => MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { })
                    .Enabled()
                    .InjectionRate(1.1)
            );

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_error_on_execution_time_when_threshold_is_is_negative()
        {
            var policy = MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { })
                    .Enabled()
                    .InjectionRate((_, __) => -1d)
            );

            policy.Invoking(x => x.Execute(() => ResultPrimitive.Good))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Should_throw_error_on_execution_time_when_threshold_is_greater_than_one()
        {
            var policy = MonkeyPolicy.InjectBehaviour<ResultPrimitive>(with =>
                with.Behaviour(() => { })
                    .Enabled()
                    .InjectionRate((_, __) => 1.1)
            );

            policy.Invoking(x => x.Execute(() => ResultPrimitive.Good))
                .ShouldThrow<ArgumentOutOfRangeException>();
        }

        #endregion
    }
}
