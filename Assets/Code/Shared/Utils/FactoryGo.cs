using System.Runtime.CompilerServices;
using UnityEngine;

namespace Shared.Utils
{
    /// <summary>
    /// GameObject creation in one call. Four families, each a cascade of overloads collapsing into
    /// one implementation: Container/Component decides whether a component is added, and the Local
    /// prefix decides whether position and rotation are read as local or as world space
    /// </summary>
    public static class FactoryGo
    {
        // Container, world space

        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer()
            => CreateContainer(null, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(string name)
            => CreateContainer(name, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(Transform parent)
            => CreateContainer(null, Vector3.zero, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(string name, Transform parent)
            => CreateContainer(name, Vector3.zero, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(Vector3 position, Transform parent)
            => CreateContainer(null, position, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(Vector3 position, Quaternion rotation, Transform parent)
            => CreateContainer(null, position, rotation, parent);
        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(string name, Vector3 position, Transform parent)
            => CreateContainer(name, position, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(string name, Vector3 position, Quaternion rotation)
            => CreateContainer(name, position, rotation, null);

        /// <summary>
        /// Empty GameObject placed in world space; a null <paramref name="name"/> keeps Unity's default
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateContainer(string name, Vector3 position, Quaternion rotation, Transform parent)
        {
            var container = CreateGameObject(name);
            var transform = container.transform;
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;
            return transform;
        }

        // Container, local space

        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer()
            => CreateLocalContainer(null, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(string name)
            => CreateLocalContainer(name, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(Transform parent)
            => CreateLocalContainer(null, Vector3.zero, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(string name, Transform parent)
            => CreateLocalContainer(name, Vector3.zero, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(Vector3 position, Transform parent)
            => CreateLocalContainer(null, position, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(Vector3 position, Quaternion rotation, Transform parent)
            => CreateLocalContainer(null, position, rotation, parent);
        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(string name, Vector3 position, Transform parent)
            => CreateLocalContainer(name, position, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateLocalContainer(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(string name, Vector3 position, Quaternion rotation)
            => CreateLocalContainer(name, position, rotation, null);

        /// <summary>
        /// Empty GameObject placed relative to <paramref name="parent"/>; a null <paramref name="name"/> keeps Unity's default
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform CreateLocalContainer(string name, Vector3 position, Quaternion rotation, Transform parent)
        {
            var container = CreateGameObject(name);
            var transform = container.transform;
            transform.SetParent(parent);
            transform.localPosition = position;
            transform.localRotation = rotation;
            return transform;
        }

        // Component, world space

        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>() where TComponent : Component
            => CreateComponent<TComponent>(null, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(string name) where TComponent : Component
            => CreateComponent<TComponent>(name, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(Transform parent) where TComponent : Component
            => CreateComponent<TComponent>(null, Vector3.zero, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(string name, Transform parent) where TComponent : Component
            => CreateComponent<TComponent>(name, Vector3.zero, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(Vector3 position, Transform parent) where TComponent : Component
            => CreateComponent<TComponent>(null, position, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component
            => CreateComponent<TComponent>(null, position, rotation, parent);
        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(string name, Vector3 position, Transform parent) where TComponent : Component
            => CreateComponent<TComponent>(name, position, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(string name, Vector3 position, Quaternion rotation) where TComponent : Component
            => CreateComponent<TComponent>(name, position, rotation, null);

        /// <summary>
        /// New GameObject placed in world space, carrying a fresh <typeparamref name="TComponent"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateComponent<TComponent>(string name, Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component
        {
            var container = CreateGameObject(name);
            var transform = container.transform;
            transform.SetParent(parent);
            transform.position = position;
            transform.rotation = rotation;
            return container.AddComponent<TComponent>();
        }

        // Component, local space

        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>() where TComponent : Component
            => CreateLocalComponent<TComponent>(null, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(string name) where TComponent : Component
            => CreateLocalComponent<TComponent>(name, Vector3.zero, Quaternion.identity, null);
        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(Transform parent) where TComponent : Component
            => CreateLocalComponent<TComponent>(null, Vector3.zero, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(string name, Transform parent) where TComponent : Component
            => CreateLocalComponent<TComponent>(name, Vector3.zero, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(Vector3 position, Transform parent) where TComponent : Component
            => CreateLocalComponent<TComponent>(null, position, Quaternion.identity, parent);

        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component
            => CreateLocalComponent<TComponent>(null, position, rotation, parent);
        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(string name, Vector3 position, Transform parent) where TComponent : Component
            => CreateLocalComponent<TComponent>(name, position, Quaternion.identity, parent);
        /// <inheritdoc cref="CreateLocalComponent{TComponent}(string,Vector3,Quaternion,Transform)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(string name, Vector3 position, Quaternion rotation) where TComponent : Component
            => CreateLocalComponent<TComponent>(name, position, rotation, null);

        /// <summary>
        /// New GameObject placed relative to <paramref name="parent"/>, carrying a fresh <typeparamref name="TComponent"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent CreateLocalComponent<TComponent>(string name, Vector3 position, Quaternion rotation, Transform parent) where TComponent : Component
        {
            var container = CreateGameObject(name);
            var transform = container.transform;
            transform.SetParent(parent);
            transform.localPosition = position;
            transform.localRotation = rotation;
            return container.AddComponent<TComponent>();
        }

        /// <summary>
        /// The one construction point: the name-less overloads pass null, and
        /// <c>new GameObject(null)</c> would name the object an empty string rather than "GameObject"
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GameObject CreateGameObject(string name)
        {
            return name == null ? new GameObject() : new GameObject(name);
        }
    }
}
