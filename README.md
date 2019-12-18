# Simmy



Simmy is a [chaos-engineering](http://principlesofchaos.org/) and fault-injection tool, integrating with the [Polly resilience project for .NET](https://github.com/App-vNext/Polly).  It is releasing April 2019 and works with [Polly v7.0.0](https://www.nuget.org/packages/Polly/7.1.0) onwards.

Simmy allows you to introduce a chaos-injection policy or policies at any location where you execute code through Polly.

[![NuGet version](https://badge.fury.io/nu/Polly.Contrib.Simmy.svg)](https://badge.fury.io/nu/Polly.Contrib.Simmy) [![Build status](https://ci.appveyor.com/api/projects/status/5v3bpgjkw4snv3no?svg=true)](https://ci.appveyor.com/project/Polly-Contrib/simmy) [![Slack Status](http://www.pollytalk.org/badge.svg)](http://www.pollytalk.org)

<img src="./Simmy_lg.png" alt="Simmy"  width="300"/>

# Motivation

There are a lot of questions when it comes to chaos-engineering and making sure that a system is actually ready to face the worst possible scenarios:

* Is my system resilient enough?
* Am I handling the right exceptions/scenarios?
* How will my system behave if X happens?
* How can I test without waiting for a handled (or even unhandled) exception to happen in my production environment?

Using Polly helps me introduce resilience to my project, but I don't want to have to wait for expected or unexpected failures to test it out. My resilience could be wrongly implemented; testing the scenarios is not straight forward; and mocking failure of some dependencies (for example a cloud SaaS or PaaS service) is not always straightforward.

**What do I need, to simulate chaotic scenarios in my production environment?**

* A way to mock failures of dependencies (any service dependency for example).
* Define when to fail based on some external factors - maybe global configuration or some rule.
* A way to revert easily, to control the blast radius.
* Production grade, to run this in a production or near-production system with automation.

# Chaos policies

Simmy offers the following chaos-injection policies:

|Policy| What does the policy do?|
| ------------- |------------- |
|**[Exception](#Inject-exception)**|Injects exceptions in your system.|
|**[Result](#Inject-result)**|Substitute results to fake faults in your system.|
|**[Latency](#Inject-latency)**|Injects latency into executions before the calls are made.|
|**[Behavior](#Inject-behavior)**|Allows you to inject _any_ extra behaviour, before a call is placed. |

# Usage

## Inject exception
```csharp
var chaosPolicy = MonkeyPolicy.InjectException(Action<InjectOutcomeOptions<Exception>>);
```

## Inject result
```csharp
var chaosPolicy = MonkeyPolicy.InjectResult(Action<InjectOutcomeOptions<TResult>>);
```

## Inject latency
```csharp
var chaosPolicy = MonkeyPolicy.InjectLatency(Action<InjectLatencyOptions>);
```

## Inject behavior
```csharp
var chaosPolicy = MonkeyPolicy.InjectBehaviour(Action<InjectBehaviourOptions>);
```

## Parameters
All the parameters are expressed in a Fluent-builder syntax way.

### Enabled
Determines whether the policy is enabled or not.

* Configure that the monkey policy is enabled. 
```csharp
PolicyOptions.Enabled();
```

* Receives a boolean value indicating whether the monkey policy is enabled.
```csharp
PolicyOptions.Enabled(bool);
```

* Receives a delegate which can be executed to determine whether the monkey policy should be enabled.
```csharp
PolicyOptions.EnabledWhen(Func<Context, CancellationToken, bool>);
```

### InjectionRate
A decimal between 0 and 1 inclusive. The policy will inject the fault, randomly, that proportion of the time, eg: if 0.2, twenty percent of calls will be randomly affected; if 0.01, one percent of calls; if 1, all calls. 

* Receives a double value between [0, 1] indicating the rate at which this monkey policy should inject chaos.
```csharp
PolicyOptions.InjectionRate(Double);
```

* Receives a delegate which can be executed to determine the rate at which this monkey policy should inject chaos.
```csharp
PolicyOptions.InjectionRate(Func<Context, CancellationToken, Double>);
```

### Fault
The fault to inject. The `Fault` api has overloads to build the policy in a generic way: `PolicyOptions.Fault<TResult>(...)`

* Receives an exception to configure the fault to inject with the monkey policy.
```csharp
PolicyOptions.Fault(Exception);
```

* Receives a delegate representing the fault to inject with the monkey policy.
```csharp
PolicyOptions.Fault(Func<Context, CancellationToken, Exception>);
```

### Result
The result to inject.

* Receives a generic TResult value to configure the result to inject with the monkey policy.
```csharp
PolicyOptions.Result<TResult>(TResult);
```

* Receives a delegate representing the result to inject with the monkey policy.
```csharp
PolicyOptions.Result<TResult>(Func<Context, CancellationToken, TResult>);
```

### Latency
The latency to inject.

* Receives a TimeSpan value to configure the latency to inject with the monkey policy.
```csharp
PolicyOptions.Latency(TimeSpan);
```

* Receives a delegate representing the latency to inject with the monkey policy.
```csharp
PolicyOptions.Latency(Func<Context, CancellationToken, TimeSpan>);
```

### Behaviour
The behaviour to inject. 

* Receives an Action to configure the behaviour to inject with the monkey policy.
```csharp
PolicyOptions.Behaviour(Action);
```

* Receives a delegate representing the Action to inject with the monkey policy.
```csharp
PolicyOptions.Behaviour(Action<Context, CancellationToken>);
```

### Context-driven behaviour

All parameters are available in a `Func<Context, ...>` form.  This allows you to control the chaos injected:

+ in a **dynamic** manner: by eg driving the chaos from external configuration files
+ in a **targeted** manner: by tagging your policy executions with a [`Context.OperationKey`](https://github.com/App-vNext/Polly/wiki/Keys-And-Context-Data#pre-defined-keys-on-context) and introducing chaos targeting particular tagged operations

The [example app](https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi) demonstrates both these approaches in practice.

# Basic examples

## Step 1: Set up the Monkey Policy

### Fault

```csharp
// Following example causes the policy to throw SocketException with a probability of 5% if enabled
var fault = new SocketException(errorCode: 10013);
var chaosPolicy = MonkeyPolicy.InjectException(with =>
	with.Fault(fault)
		.InjectionRate(0.05)
		.Enabled()
	);
```

### Result

```csharp
// Following example causes the policy to return a bad request HttpResponseMessage with a probability of 5% if enabled
var result = new HttpResponseMessage(HttpStatusCode.BadRequest);
var chaosPolicy = MonkeyPolicy.InjectResult<HttpResponseMessage>(with =>
	with.Result(result)
		.InjectionRate(0.05)
		.Enabled()
);
```

### Latency
```csharp

// Following example causes policy to introduce an added latency of 5 seconds to a randomly-selected 10% of the calls.
var isEnabled = true;
var chaosPolicy = MonkeyPolicy.InjectLatency(with =>
	with.Latency(TimeSpan.FromSeconds(5))
		.InjectionRate(0.1)
		.Enabled(isEnabled)
	);
```

### Behavior

```csharp
// Following example causes policy to execute a method to restart a virtual machine; the probability that method will be executed is 1% if enabled
var chaosPolicy = MonkeyPolicy.InjectBehaviour(with =>
	with.Behaviour(() => restartRedisVM())
		.InjectionRate(0.01)
		.EnabledWhen((ctx, ct) => isEnabled(ctx, ct))
	);
```

## Step 2: Execute code through the Monkey Policy

```csharp
// Executes through the chaos policy directly
chaosPolicy.Execute(() => someMethod());

// Executes through the chaos policy using Context
chaosPolicy.Execute((ctx) => someMethod(), context);

// Wrap the chaos policy inside other Polly resilience policies, using PolicyWrap
var policyWrap = Policy
  .Wrap(fallbackPolicy, timeoutPolicy, chaosLatencyPolicy);
policyWrap.Execute(() => someMethod())

// All policies are also available in async forms.
var chaosLatencyPolicy = MonkeyPolicy.InjectLatencyAsync(with =>
	with.Latency(TimeSpan.FromSeconds(5))
		.InjectionRate(0.1)
		.Enabled()
	);
var policyWrap = Policy
  .WrapAsync(fallbackPolicy, timeoutPolicy, chaosLatencyPolicy);
var result = await policyWrap.ExecuteAsync(token => service.GetFoo(parametersBar, token), myCancellationToken);

// For general information on Polly policy syntax see: https://github.com/App-vNext/Polly
```

It is usual to place the Simmy policy innermost in a PolicyWrap. By placing the chaos policies innermost, they subvert the usual outbound call at the last minute, substituting their fault or adding extra latency. The existing Polly policies - further out in the PolicyWrap - still apply, so you can test how the Polly resilience you have configured handles the chaos/faults injected by Simmy.

**Note:** The above examples demonstrate how to execute through a Simmy policy directly, and how to include a Simmy policy in an individual PolicyWrap. If your policies are configured by .NET Core DI at StartUp, for example via HttpClientFactory, there are also patterns which can configure Simmy into your app as a whole, at StartUp. See the Simmy Sample App discussed below.

## Example app: Controlling chaos via configuration and Polly.Context

This [Simmy sample app](https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi) shows different approaches/patterns for how you can configure Simmy to introduce chaos policies in a project.  Patterns demonstrated are:

* Configuring `StartUp` so that Simmy chaos policies are only introduced in builds for certain environments (for instance, Dev but not Prod).
* Configuring Simmy chaos policies to be injected into the app without changing any existing Polly configuration code.
* Injecting faults or chaos by modifying external configuration. 

The patterns shown in the sample app are intended as starting points but are not mandatory.  Simmy is very flexible, and we would love to hear how you use it!

## Wrapping up

All chaos policies (Monkey policies) are designed to inject behavior randomly (faults, latency or custom behavior), so a Monkey policy allows you to specify an injection rate between 0 and 1 (0-100%) thus, the higher is the injection rate the higher is the probability to inject them. Also it allows you to specify whether or not the random injection is enabled, that way you can release/hold (turn on/off) the monkeys regardless of injection rate you specify, it means, if you specify an injection rate of 100% but you tell to the policy that the random injection is disabled, it will do nothing.

## Further information

See [Issues](https://github.com/App-vNext/Simmy/issues) for latest discussions on taking Simmy forward!

## Credits

Simmy was [the brainchild of](https://github.com/App-vNext/Polly/issues/499) [@mebjas](https://github.com/mebjas) and [@reisenberger](https://github.com/reisenberger). The major part of the implementation was by [@vany0114](https://github.com/vany0114) and [@mebjas](https://github.com/mebjas), with contributions also from [@reisenberger](https://github.com/reisenberger) of the Polly team.

## Blogs and architecture samples around Simmy

### Blog posts
* [Simmy, the monkey for making chaos](http://elvanydev.com/chaos-injection-with-simmy/) -by [Geovanny Alzate Sandoval](https://twitter.com/geovany0114)
* [Simmy and Azure App Configuration](http://elvanydev.com/simmy-with-azure-app-configuration/) -by [Geovanny Alzate Sandoval](https://twitter.com/geovany0114)
* [Simmy series on No Dogma blog](https://nodogmablog.bryanhogan.net/tag/simmy/) -by [Bryan Hogan](https://twitter.com/bryanjhogan)

### Samples
* [Dylan Reisenberger](http://www.thepollyproject.org/author/dylan/) presents an [intentionally simple example](https://github.com/Polly-Contrib/Polly.Contrib.SimmyDemo_WebApi) .NET Core WebAPI app demonstrating how we can set up Simmy chaos policies for certain environments and without changing any existing configuration code injecting faults or chaos by modifying external configuration.

* [Geovanny Alzate Sandoval](https://github.com/vany0114) made a [microservices based sample application](https://github.com/vany0114/chaos-injection-using-simmy) to demonstrate how chaos engineering works with Simmy using chaos policies in a distributed system and how we can inject even a custom behavior given our needs or infrastructure, this time injecting custom behavior to generate chaos in our Service Fabric Cluster.
 
