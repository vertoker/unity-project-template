using System;

// THIS TYPE IS DELIBERATELY NOT IN THE PROJECT NAMESPACE, AND THAT IS THE WHOLE POINT OF IT. il2cpp
// resolves the attribute by its full metadata name and by nothing else — it never references an
// assembly of Unity's own — so a project-owned copy under the exact name works, which is why Unity
// ships the source rather than a reference at <Editor>/Data/il2cpp/Il2CppSetOptionAttribute.cs.
//
// The copy is REQUIRED rather than a convenience: UnityEngine.CoreModule carries the same type and
// declares it `internal`, so user code cannot see it. It is declared `public` here because every
// assembly above consumes it through Shared; if a future Unity version makes its own copy public,
// this one collides (CS0433) and the fix is to delete this file, not to rename anything. An assembly
// that cannot reference Shared — a `noEngineReferences` one — needs its own `internal` copy instead,
// or the two public names become ambiguous in whoever sees both.

// ReSharper disable once CheckNamespace
namespace Unity.IL2CPP.CompilerServices
{
    /// <summary>
    /// The code generation options available for IL to C++ conversion. Enable or disable with caution.
    /// </summary>
    public enum Option
    {
        /// <summary>
        /// Code generation for null checks, on by default. Disabling it prevents
        /// NullReferenceException from being thrown; dereferencing null then usually crashes, and
        /// not always at the point the check would have sat.
        /// </summary>
        NullChecks = 1,

        /// <summary>
        /// Code generation for array bounds checks, on by default. Disabling it prevents
        /// IndexOutOfRangeException from being thrown and allows reads and writes outside a managed
        /// array with no runtime check at all.
        /// </summary>
        ArrayBoundsChecks = 2,

        /// <summary>
        /// Code generation for divide by zero checks, off by default. Enabling it causes
        /// DivideByZeroException to be thrown from generated code.
        /// </summary>
        DivideByZeroChecks = 3,
    }

    /// <summary>
    /// Overrides one of il2cpp's global runtime checks for the assembly, type, method, property or
    /// delegate it is applied to. Only IL2CPP reads it — a Mono player and the Editor ignore it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Struct | AttributeTargets.Class |
                    AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate,
        Inherited = false, AllowMultiple = true)]
    public class Il2CppSetOptionAttribute : Attribute
    {
        /// <summary>
        /// Which check is being overridden
        /// </summary>
        public Option Option { get; }

        /// <summary>
        /// The new state of that check, boxed — il2cpp reads it as a bool
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Binds <paramref name="option"/> to <paramref name="value"/> for everything this attribute covers
        /// </summary>
        public Il2CppSetOptionAttribute(Option option, object value)
        {
            Option = option;
            Value = value;
        }
    }
}
