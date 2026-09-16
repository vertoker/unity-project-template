using System;
using UnityEngine;

// The m_ prefix is what the custom drawer looks the backing field up by — see Shared.Editor.SerializableGuidDrawer
// ReSharper disable InconsistentNaming

namespace Shared
{
    /// <summary>
    /// A <see cref="System.Guid"/> Unity can serialize: the value lives as a string in the asset
    /// and is parsed back on load. Converts to and from <see cref="System.Guid"/> implicitly,
    /// and compares by value — but the value is mutable, so never mutate one that is a dictionary key
    /// </summary>
    [Serializable]
    public class SerializableGuid : ISerializationCallbackReceiver, IEquatable<SerializableGuid>
    {
        private Guid m_Guid;
        [SerializeField, HideInInspector] private string m_SerializedGuid;

        /// <summary>
        /// Unwraps the value; a null wrapper reads as <see cref="System.Guid.Empty"/> rather than throwing
        /// </summary>
        public static implicit operator Guid(SerializableGuid serializableGuid)
            => serializableGuid?.m_Guid ?? Guid.Empty;

        /// <summary>
        /// Wraps the value in a fresh instance
        /// </summary>
        public static implicit operator SerializableGuid(Guid guid) => new(guid);

        /// <summary>
        /// The wrapped value; assigning it is picked up by the next serialization pass
        /// </summary>
        public Guid Guid
        {
            get => m_Guid;
            set => m_Guid = value;
        }

        /// <summary>
        /// Leaves the value at <see cref="System.Guid.Empty"/> — the shape Unity needs to deserialize into
        /// </summary>
        public SerializableGuid()
        {
        }

        /// <summary>
        /// Wraps an existing <see cref="System.Guid"/>
        /// </summary>
        public SerializableGuid(Guid guid)
        {
            m_Guid = guid;
            m_SerializedGuid = m_Guid.ToString();
        }

        /// <summary>
        /// A wrapper around <see cref="System.Guid.Empty"/>
        /// </summary>
        public static SerializableGuid GetEmpty() => new(Guid.Empty);

        /// <summary>
        /// A wrapper around a freshly generated <see cref="System.Guid"/>
        /// </summary>
        public static SerializableGuid GetNewGuid() => new(Guid.NewGuid());

        /// <inheritdoc/>
        public void OnBeforeSerialize()
        {
            m_SerializedGuid = m_Guid.ToString();
        }

        /// <inheritdoc/>
        public void OnAfterDeserialize()
        {
            m_Guid = Guid.Empty;
            if (!string.IsNullOrEmpty(m_SerializedGuid) && !Guid.TryParse(m_SerializedGuid, out m_Guid))
                GameLogger.LogWarning<SerializableGuid>(
                    $"Attempted to parse (deserialize) invalid Guid string '{m_SerializedGuid}'. Guid will set to Guid.Empty");
        }

        /// <summary>
        /// Replaces the value with a freshly generated one, in place
        /// </summary>
        public void NewGuid()
        {
            m_Guid = Guid.NewGuid();
        }

        /// <summary>
        /// The wrapped value in the standard "D" form, the same text that goes into the asset
        /// </summary>
        public override string ToString() => m_Guid.ToString();

        // A null wrapper is never equal to anything, not even to a wrapper of Guid.Empty. The implicit
        // conversion above does read null as Guid.Empty, but equality cannot follow it: `guid == null`
        // is the null check everyone writes, and it has to keep meaning "is the reference null"

        /// <inheritdoc/>
        public bool Equals(SerializableGuid other) => other is not null && m_Guid == other.m_Guid;

        /// <summary>
        /// Value equality against another wrapper; any other type, including a bare
        /// <see cref="System.Guid"/>, is not equal — use the operators for those
        /// </summary>
        public override bool Equals(object obj) => obj is SerializableGuid other && Equals(other);

        /// <summary>
        /// The wrapped value's hash. Mutating the wrapper afterwards makes it unfindable in any hash table
        /// </summary>
        public override int GetHashCode() => m_Guid.GetHashCode();

        /// <summary>
        /// Value equality; two nulls are equal, a null and a wrapper never are
        /// </summary>
        public static bool operator ==(SerializableGuid left, SerializableGuid right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.m_Guid == right.m_Guid;
        }

        /// <inheritdoc cref="op_Equality(SerializableGuid,SerializableGuid)"/>
        public static bool operator !=(SerializableGuid left, SerializableGuid right) => !(left == right);

        // Without these, comparing a wrapper to a bare Guid is CS0034: the compiler can convert
        // either side and has no reason to prefer one

        /// <summary>
        /// Value equality against a bare <see cref="System.Guid"/>; a null wrapper matches nothing
        /// </summary>
        public static bool operator ==(SerializableGuid left, Guid right) => left is not null && left.m_Guid == right;

        /// <inheritdoc cref="op_Equality(SerializableGuid,System.Guid)"/>
        public static bool operator !=(SerializableGuid left, Guid right) => !(left == right);

        /// <inheritdoc cref="op_Equality(SerializableGuid,System.Guid)"/>
        public static bool operator ==(Guid left, SerializableGuid right) => right is not null && right.m_Guid == left;

        /// <inheritdoc cref="op_Equality(SerializableGuid,System.Guid)"/>
        public static bool operator !=(Guid left, SerializableGuid right) => !(left == right);
    }
}
