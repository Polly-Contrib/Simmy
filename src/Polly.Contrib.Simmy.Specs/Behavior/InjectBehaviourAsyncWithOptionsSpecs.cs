using FluentAssertions;
using Polly.Contrib.Simmy.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Polly.Contrib.Simmy.Specs.Behavior
{
    [Collection(Helpers.Constants.AmbientContextDependentTestCollection)]
    public class InjectBehaviourAsyncWithOptionsSpecs
    {
        public InjectBehaviourAsyncWithOptionsSpecs()
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

            var policy = MonkeyPolicy.InjectBehaviourAsync(options =>
            {
                options.InjectionRate = 0.6;
                options.Enabled = () => false;
                options.Behaviour = () =>
                {
                    injectedBehaviourExecuted = true;
                    return Task.CompletedTask;
                };
            });

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.CompletedTask; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeFalse();
        }

        [Fact]
        public void Given_enabled_and_randomly_within_threshold_should_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync(options =>
            {
                options.InjectionRate = 0.6;
                options.Enabled = () => true;
                options.Behaviour = () =>
                {
                    injectedBehaviourExecuted = true;
                    return Task.CompletedTask;
                };
            });

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.CompletedTask; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeTrue();
        }

        [Fact]
        public void Given_enabled_and_randomly_not_within_threshold_should_not_inject_behaviour()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync(options =>
            {
                options.InjectionRate = 0.4;
                options.Enabled = () => true;
                options.Behaviour = () =>
                {
                    injectedBehaviourExecuted = true;
                    return Task.CompletedTask;
                };
            });

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.CompletedTask; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeFalse();
        }

        [Fact]
        public void Should_inject_behaviour_before_executing_user_delegate()
        {
            Boolean userDelegateExecuted = false;
            Boolean injectedBehaviourExecuted = false;

            var policy = MonkeyPolicy.InjectBehaviourAsync(options =>
            {
                options.InjectionRate = 0.6;
                options.Enabled = () => true;
                options.Behaviour = () =>
                {
                    userDelegateExecuted.Should().BeFalse(); // Not yet executed at the time the injected behaviour runs.
                    injectedBehaviourExecuted = true;
                    return Task.CompletedTask;
                };
            });

            policy.ExecuteAsync(() => { userDelegateExecuted = true; return Task.CompletedTask; });

            userDelegateExecuted.Should().BeTrue();
            injectedBehaviourExecuted.Should().BeTrue();
        }
    }
}
