using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Shared.Utils
{
    /// <summary>
    /// Wrappers around <see cref="ObjectPool{T}"/> for the cases that keep recurring.
    /// Every pool gets <c>collectionCheck</c> from <see cref="DebugUtils.IsDebug"/>, so
    /// double-release is caught in the Editor and costs nothing in release
    /// </summary>
    public static class ObjectPoolFactory
    {
        /// <summary>
        /// Pool of a plain class built with its parameterless constructor
        /// </summary>
        public static ObjectPool<T> CreateDefault<T>(int defaultCapacity = 10, int maxSize = 10000)
            where T : class, new()
        {
            var pool = new ObjectPool<T>(Create, null, null,
                null, DebugUtils.IsDebug(), defaultCapacity, maxSize);
            return pool;

            T Create() => new();
        }

        /// <summary>
        /// Pool of a plain class built by <paramref name="onCreate"/>, with no get/release hooks
        /// </summary>
        public static ObjectPool<T> CreateDefault<T>(Func<T> onCreate, int defaultCapacity = 10,
            int maxSize = 10000) where T : class
        {
            var pool = new ObjectPool<T>(onCreate, null, null,
                null, DebugUtils.IsDebug(), defaultCapacity, maxSize);
            return pool;
        }

        /// <summary>
        /// Pool of a plain class with every hook spelled out
        /// </summary>
        public static ObjectPool<T> CreateDefault<T>(Func<T> onCreate, Action<T> onGet = null,
            Action<T> onRelease = null, Action<T> onDestroy = null, int defaultCapacity = 10,
            int maxSize = 10000) where T : class
        {
            var pool = new ObjectPool<T>(onCreate, onGet, onRelease, onDestroy,
                DebugUtils.IsDebug(), defaultCapacity, maxSize);
            return pool;
        }

        /// <summary>
        /// Pool that instantiates <paramref name="prefab"/> and nothing else — the caller
        /// decides what activity means for these objects
        /// </summary>
        public static ObjectPool<T> CreateComponentOnlyCreate<T>(T prefab, int defaultCapacity = 10,
            int maxSize = 10000) where T : Component
        {
            var pool = new ObjectPool<T>(Create, null, null,
                null, DebugUtils.IsDebug(), defaultCapacity, maxSize);
            return pool;

            T Create() => Object.Instantiate(prefab);
        }

        /// <summary>
        /// Pool that also toggles the GameObject on get and release. A released object stays
        /// in the scene, inactive — it is never destroyed, not even over <paramref name="maxSize"/>
        /// </summary>
        public static ObjectPool<T> CreateComponentToggleGo<T>(T prefab, int defaultCapacity = 10,
            int maxSize = 10000) where T : Component
        {
            var pool = new ObjectPool<T>(Create, EnableGo, DisableGo,
                null, DebugUtils.IsDebug(), defaultCapacity, maxSize);
            return pool;

            T Create() => Object.Instantiate(prefab);
            void EnableGo(T component) => component.gameObject.SetActive(true);
            void DisableGo(T component) => component.gameObject.SetActive(false);
        }

        /// <summary>
        /// Same as <see cref="CreateComponentToggleGo{T}"/>, plus it destroys the GameObject
        /// when the pool clears or overflows <paramref name="maxSize"/>
        /// </summary>
        public static ObjectPool<T> CreateComponentToggleGoAndDestroy<T>(T prefab,
            int defaultCapacity = 10, int maxSize = 10000) where T : Component
        {
            var pool = new ObjectPool<T>(Create, EnableGo, DisableGo,
                DestroyGo, DebugUtils.IsDebug(), defaultCapacity, maxSize);
            return pool;

            T Create() => Object.Instantiate(prefab);
            void EnableGo(T component) => component.gameObject.SetActive(true);
            void DisableGo(T component) => component.gameObject.SetActive(false);
            void DestroyGo(T component)
            {
                // The scene may already be tearing down, taking the object with it
                if (!component || !component.gameObject) return;
                Object.Destroy(component.gameObject);
            }
        }
    }
}
