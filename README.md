# Simmy
Simmy is a [chaos-engineering](http://principlesofchaos.org/) and fault-injection tool, integrating with the Polly resilience project for .NET.  It is expected to release in early 2019 and will work with Polly v7.0.

# Motivation

There are a lot of questions when it comes to chaos-engineering and make sure that actually my system is ready to face the worst possibles scenarios.

* My system is really resilient enough?
* Am I really handling the right exceptions/scenarios?
* Why should I wait for a handled or even and unhandled exception happens in my production environment?

Using Polly helps me tons to introduce resilience to my project, but I don't want expected or unexpected failures to occur to test it out. My code could be wrongly implemented, testing the scenarios is not straight forward, mocking failure of a dependency for example a cloud SaaS / PaaS service is not always provided straight away.

**How could I simulate chaotics scenarios in my production environment?***

* A way to mock failures of dependencies (any service dependency for example).
* Takes care of when to fail based on some external factors - maybe global configuration or some rule.
* Way to revert easily controlling the blast radius
* Production grade to run this in production system with automation.

# Chaos policies

Simmy offers multiple chaos policies:

|Policy| Premise | Aka| How does the policy works?|
| ------------- | ------------- |:-------------: |------------- |
|**[Fault](#fault)**|Some description here.| "Some description here" |  Some description here. |
|**[Latency](#latency)**|Some description here.| "Some description here" |  Some description here. |
|**[Behavior](#behavior)**|Some description here.| "Some description here" |  Some description here. |

# Usage
All chaos policies (Monkey plicies) are designed to inject the behavior randomly (faults, latency or custom behavior), so a Monkey policy allows you to specify an injection rate between 0 and 1 (0-100%) thus, the higher is the injection rate the higher is the probability to inject them. Also it allows you to specify whether or not the random injection is enabled, that way you can release/hold (turn on/off) the monkeys regardless of injection rate you specify, it means, if you specify an injection rate of 100% but you tell to the policy that the random injection is disabled, it will do nothing.

## Step 1: Set up the Monkey Policy

### Latency

```
// context free setup
var chaosPolicy = MonkeyPolicy.InjectLatency(
	latency: TimeSpan.FromMinutes(1), // or randomized
	injectionRate: 0.5, // inject randomly 50% of the time
	enabled: () => isEnabled() // switch expt on/off by config
	);
	
// using context to set up the policy
var context = new Context();
context["Enabled"] = isEnabled();
context["InjectionRate"] = someInjectionRateProvider();
context["ShouldInjectLatency"] = shouldInjectLatency();
			
Func<Context, bool> enabled = (ctx) =>
{
	// more validations here based on the context (ctx)
	return ((bool)ctx["Enabled"]);
};

Func<Context, TimeSpan> latencyProvider = (ctx) =>
{
	// more validations here based on the context (ctx)
	if ((bool)ctx["ShouldInjectLatency"])
	{
		return someLatencyProvider();
	}

	return TimeSpan.FromMilliseconds(0);
};

Func<Context, double> injectionRate = (ctx) =>
{
	// more validations here based on the context (ctx)
	if (ctx["InjectionRate"] != null)
	{
		return (double)ctx["InjectionRate"];
	}

	return 0;
};
chaosPolicy = MonkeyPolicy.InjectLatency(latencyProvider, injectionRate, enabled);
```

### Fault
```
// context free setup
var fault = new SocketException("Monkey fault exception");
var faultPolicy = MonkeyPolicy.InjectFault<SocketException>(
	fault, 
	injectionRate: 0.5, // inject randomly 50% of the time
	enabled: () => isEnabled() // switch expt on/off by config
	);

// using context to set up the policy
var context = new Context();
var failureMessage = "Failure Message";
context["ShouldFail"] = true;
context["Message"] = failureMessage;
context["InjectionRate"] = 0.5;

Func<Context, Exception> fault = (ctx) =>

	// more validations here based on the context (ctx)
	if (ctx["Message"] != null)
	{
		return new InvalidOperationException(ctx["Message"].ToString());
	}

	return new Exception();
};

Func<Context, double> injectionRate = (ctx) =>
{
	// more validations here based on the context (ctx)
	if (ctx["InjectionRate"] != null)
	{
		return (double)ctx["InjectionRate"];
	}

	return 0;
};

Func<Context, bool> enabled = (ctx) =>
{
	// more validations here based on the context (ctx)
	return ((bool)ctx["ShouldFail"]);
};

faultPolicy = MonkeyPolicy.InjectFault(fault, injectionRate, enabled);
```

### Behavior
```

// todo: add examples here

```

## Step 2: Execute the Monkey Policy

```
// executes the chaos policy directly
chaosPolicy.Execute(() => someMethod());

// executes the chaos policy using Context
chaosPolicy.Execute((ctx) => someMethod(), context);

// wrap the chaos policy using a PolicyWrap
var policyWrap = Policy
  .Wrap(fallbackPolicy, timeoutPolicy, chaosLatencyPolicy);
policyWrap.Execute(() => someMethod())
```


## Further information

For background on the original proposal for a fault-injection dimension to Polly, see [the original issue on the Polly repo](https://github.com/App-vNext/Polly/issues/499).

See [Issues](https://github.com/App-vNext/Simmy/issues) for latest discussions as we bring Simmy towards release!

## Credits

Simmy was [the brainchild of](https://github.com/App-vNext/Polly/issues/499) [@mebjas](https://github.com/mebjas) and [@reisenberger](https://github.com/reisenberger). The major part of the implementation was by [@vany0114](https://github.com/vany0114) and [@mebjas](https://github.com/mebjas), with contributions also from [@reisenberger](https://github.com/reisenberger) of the Polly team.

TODO: 
* examples with other overloads
* async examples
* descriptions