namespace Polly.Contrib.Simmy.Specs.Helpers
{
    /// <summary>
    /// Constants supporting tests.
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// Used to identify an xUnit test collection dependent on manipulating some ambient context.
        /// <remarks>Tests in such collections are not parallelized, which prevents one test polluting another when ambient context is manipulated.</remarks>
        /// </summary>
        public const string AmbientContextDependentTestCollection = "AmbientContextDependentTestCollection";
    }
}