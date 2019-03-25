# Simmy
Simmy is a [chaos-engineering](http://principlesofchaos.org/) and fault-injection tool, integrating with the Polly resilience project for .NET.  It is expected to release in early 2019 and will work with Polly v7.0.

# Motivation

There are a lot of questions when it comes to chaos-engineering and making sure that actually, my system is ready to face the worst possible scenarios.

* My system is really resilient enough?
* Am I really handling the right exceptions/scenarios?
* Why should I wait for a handled or even an unhandled exception happens in my production environment?

Using Polly helps me tons to introduce resilience to my project, but I don't want expected or unexpected failures to occur to test it out. My code could be wrongly implemented, testing the scenarios is not straight forward, mocking failure of a dependency for example a cloud SaaS / PaaS service is not always provided straight away.

**How could I simulate chaotics scenarios in my production environment?**

* A way to mock failures of dependencies (any service dependency for example).
* Takes care of when to fail based on some external factors - maybe global configuration or some rule.
* Way to revert easily controlling the blast radius
* Production grade to run this in production system with automation.

# Chaos policies

Simmy offers multiple chaos policies:

|Policy| What does the policy do?|
| ------------- |------------- |
|**[Fault](##Inject-fault)**|Injects exceptions or substitute results, to fake faults in your system.|
|**[Latency](##Inject-latency)**|Injects latency into calls before the calls are made.|
|**[Behavior](##Inject-behavior)**|Allows you to inject _any_ extra behaviour, before a call is placed. |

# Usage

## Inject fault
```csharp
var chaosPolicy = MonkeyPolicy.InjectFault(
    Exception | Func<Context, Exception> fault,
    double | Func<Context, double> injectionRate, 
    Func<bool> | Func<Context, bool> enabled 
);
```

## Inject latency
```csharp
var chaosPolicy = MonkeyPolicy.InjectLatency(
    TimeSpan | Func<Context, Timespan> latency,
    double | Func<Context, double> injectionRate, 
    Func<bool> | Func<Context, bool> enabled 
);
```

## Inject behavior
```csharp
var chaosPolicy = MonkeyPolicy.InjectLatency(
    Action | Action<Context> behaviour,
    double | Func<Context, double> injectionRate, 
    Func<bool> | Func<Context, bool> enabled 
);
```
* **injectionRate:** A decimal between 0 and 1 inclusive. The policy will inject the fault, randomly, that proportion of the time, eg: if 0.2, twenty percent of calls will be randomly affected; if 0.01, one percent of calls; if 1, all calls. (You can pass a delegate to get the injection rate based on the Context.)
* **enabled:** Faults are only injected when returns true. You can pass a delegate to get the value based on the Context.
* **fault:** The fault to inject. You can pass a delegate to get the fault exception object based on the Context.
* **latency:** The latency to inject. You can pass a delegate to get the latency based on the Context.
* **behaviour:** The behaviour to inject. You can pass a delegate to get the behaviour based on the Context.

## Wrapping up
All chaos policies (Monkey policies) are designed to inject the behavior randomly (faults, latency or custom behavior), so a Monkey policy allows you to specify an injection rate between 0 and 1 (0-100%) thus, the higher is the injection rate the higher is the probability to inject them. Also it allows you to specify whether or not the random injection is enabled, that way you can release/hold (turn on/off) the monkeys regardless of injection rate you specify, it means, if you specify an injection rate of 100% but you tell to the policy that the random injection is disabled, it will do nothing.

# Basic examples

## Step 1: Set up the Monkey Policy

### Fault
```csharp
// Following example causes the policy to throw SocketException with a probability of 50% if enabled
var fault = new SocketException("Monkey fault exception");
var faultPolicy = MonkeyPolicy.InjectFault<SocketException>(
	fault, 
	injectionRate: 0.5, 
	enabled: () => isEnabled()
	);
```

### Latency
```csharp
// Following example causes policy to introduce an added latency of 1 minute to 50% of the method calls.
var chaosPolicy = MonkeyPolicy.InjectLatency(
	latency: TimeSpan.FromMinutes(1),
	injectionRate: 0.5,
	enabled: () => isEnabled()
	);
```

### Behavior
```csharp
// Following example causes policy to execute a method that's supposed to restart a virtual machine, the probability that method will be executed is 50% if enabled
var chaosPolicy = MonkeyPolicy.InjectBehaviour(
	behaviour: () => restartRedisVM(), 
	injectionRate: 0.5,
	enabled: () => isEnabled()
	);
```

## Step 2: Execute the Monkey Policy

```csharp
// executes the chaos policy directly
chaosPolicy.Execute(() => someMethod());

// executes the chaos policy using Context
chaosPolicy.Execute((ctx) => someMethod(), context);

// wrap the chaos policy using a PolicyWrap
var policyWrap = Policy
  .Wrap(fallbackPolicy, timeoutPolicy, chaosLatencyPolicy);
policyWrap.Execute(() => someMethod())
```
It is usual to place the Simmy policy innermost in a PolicyWrap. By placing the chaos policies innermost, they subvert the usual outbound call at the last minute, substituting their fault or adding extra latency. The existing Polly policies - further out in the PolicyWrap - still apply, so you can test how the Polly resilience you have configured handles the chaos/faults injected by Simmy.

**Note:** The above examples demonstrate how to execute through a Simmy policy directly, and how to include a Simmy policy in an individual PolicyWrap. If your policies are configured by .NET Core DI at StartUp, for example via HttpClientFactory, there are also patterns which can configure Simmy into your app as a whole, at StartUp. For examples, see the [Simmy Sample App](https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi).

## Example app: Controlling chaos via configuration and Polly.Context

This [handly example](https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi) shows different approaches/patterns about how you can configure Simmy to introduce chaos policies in your environments, such as:

* Configuring `StartUp` so that Simmy chaos policies are only introduced in builds for certain environments (for instance, Dev but not Prod).
* Configuring Simmy chaos policies to be injected into the app without changing any existing configuration code.
* Injecting faults or chaos by modifying external configuration. 

**Note:** The patterns shown in this sample app are not mandatory. They are intended to demonstrate approaches you could take when introducing Simmy to an app, but Simmy is very flexible, and comments in this article describe how you could also take Simmy further.

## Further information

For background on the original proposal for a fault-injection dimension to Polly, see [the original issue on the Polly repo](https://github.com/App-vNext/Polly/issues/499).
See [Issues](https://github.com/App-vNext/Simmy/issues) for latest discussions as we bring Simmy towards release!

## Credits

Simmy was [the brainchild of](https://github.com/App-vNext/Polly/issues/499) [@mebjas](https://github.com/mebjas) and [@reisenberger](https://github.com/reisenberger). The major part of the implementation was by [@vany0114](https://github.com/vany0114) and [@mebjas](https://github.com/mebjas), with contributions also from [@reisenberger](https://github.com/reisenberger) of the Polly team.
