﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy.Fault.Options
{
    /// <summary>
    /// Options used to configure an <see cref="InjectOutcomePolicy"/>
    /// </summary>
    public class InjectFaultAsyncOptions<TResult> : InjectOptionsAsyncBase
    {
        internal Func<Context, CancellationToken, Task<TResult>> Outcome { get; set; }
    }
}
