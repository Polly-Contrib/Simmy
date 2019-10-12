using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Contrib.Simmy
{
    /// <summary>
    /// Options used to configure a <see cref="MonkeyPolicy"/>
    /// </summary>
    public abstract class InjectOptionsAsyncBase
    {
        /// <summary>
        /// Lambda to get injection rate between [0, 1]
        /// </summary>
        internal Func<Context, CancellationToken, Task<Double>> InjectionRate { get; set; }

        /// <summary>
        /// Lambda to check if this policy is enabled in current context
        /// </summary>
        internal Func<Context, CancellationToken, Task<bool>> Enabled { get; set; }
    }
}