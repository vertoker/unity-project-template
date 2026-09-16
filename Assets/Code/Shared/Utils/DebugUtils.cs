namespace Shared.Utils
{
    /// <summary>
    /// The one place that answers "are diagnostics compiled into this build"
    /// </summary>
    public static class DebugUtils
    {
        /// <summary>
        /// True in the Editor and in any build that kept its checks — the switch for
        /// collection checks, extra asserts and other work a release build should not pay for
        /// </summary>
        public static bool IsDebug()
        {
#if DEBUG && !UNITY_ASSERTIONS
            // If DEBUG exists, but UNITY_ASSERTIONS is not
            return true;
#elif UNITY_ASSERTIONS
            // If Unity Assertions is active
            return true;
#elif UNITY_INCLUDE_INSTRUMENTATION
            // Profiling and diagnostic logging are compiled in. This branch read
            // DEVELOPMENT_BUILD until 6.6 deprecated it and split it in two; the other half,
            // UNITY_ENABLE_CHECKS, is what the UNITY_ASSERTIONS branch above already covers.
            return true;
#elif UNITY_EDITOR
            // In editor always active
            return true;
#else
            return false;
#endif
        }
    }
}
