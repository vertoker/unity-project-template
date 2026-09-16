// v 1.2, made by vertoker (2026)

using System;
using UnityEditor;
using UnityEngine;

namespace Shared.Editor
{
    /// <summary>
    /// Draws <see cref="SerializableGuid"/> as an editable text field with a right-click menu
    /// for copy, paste, regenerate and clear
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        // The name of SerializableGuid's backing field — the two files have to stay in lockstep
        private const string SerializedGuidField = "m_SerializedGuid";

        private static readonly GUIContent CopyContent = EditorGUIUtility.TrTextContent("Copy");
        private static readonly GUIContent PasteContent = EditorGUIUtility.TrTextContent("Paste");
        private static readonly GUIContent NewGuidContent = EditorGUIUtility.TrTextContent("New Guid");
        private static readonly GUIContent EmptyGuidContent = EditorGUIUtility.TrTextContent("Empty Guid");

        /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var serializedProperty = property.FindPropertyRelative(SerializedGuidField);

            // The menu fires after OnGUI returns, by which point property may already be invalid,
            // so the callbacks below rebuild their own SerializedObject from these two instead
            var propertyPath = property.propertyPath;
            var targets = property.serializedObject.targetObjects;
            var currentValue = serializedProperty.stringValue;

            label = EditorGUI.BeginProperty(position, label, serializedProperty);

            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 1 && position.Contains(e.mousePosition))
            {
                var context = new GenericMenu();

                context.AddItem(CopyContent, false, Copy);
                context.AddItem(PasteContent, false, Paste);
                context.AddItem(NewGuidContent, false, NewGuid);
                context.AddItem(EmptyGuidContent, false, EmptyGuid);

                context.ShowAsContext();
                e.Use();
            }

            if (serializedProperty.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
                EditorGUI.BeginChangeCheck();
                var typed = EditorGUI.TextField(position, label, string.Empty);
                if (EditorGUI.EndChangeCheck() && Guid.TryParse(typed, out var typedGuid))
                {
                    // Explicit user edit while mixed - this one IS meant to broadcast the same
                    // value to every selected target, same as Paste/Empty Guid below.
                    serializedProperty.stringValue = typedGuid.ToString();
                }
                EditorGUI.showMixedValue = false;
            }
            else
            {
                var guid = Guid.Empty;
                if (!string.IsNullOrEmpty(currentValue) && !Guid.TryParse(currentValue, out guid))
                    GameLogger.LogWarning<SerializableGuidDrawer>(
                        $"Attempted to parse (load) invalid Guid string '{currentValue}'. Guid will set to Guid.Empty");

                EditorGUI.BeginChangeCheck();
                var newSerializedGuid = EditorGUI.TextField(position, label, guid.ToString());
                if (EditorGUI.EndChangeCheck())
                {
                    if (!Guid.TryParse(newSerializedGuid, out guid))
                        GameLogger.LogWarning<SerializableGuidDrawer>(
                            $"Attempted to parse (save) invalid Guid string '{newSerializedGuid}'. Guid will set to Guid.Empty");

                    serializedProperty.stringValue = guid.ToString();
                }
            }

            EditorGUI.EndProperty();
            return;

            void Copy()
            {
                EditorGUIUtility.systemCopyBuffer = currentValue;
            }

            void Paste()
            {
                var pasted = EditorGUIUtility.systemCopyBuffer;
                if (!Guid.TryParse(pasted, out var pastedGuid))
                {
                    GameLogger.LogWarning<SerializableGuidDrawer>(
                        $"Attempted to parse (paste) invalid Guid string '{pasted}'. Nothing was pasted");
                    return;
                }

                var value = pastedGuid.ToString();
                SetOnEveryTarget(() => value);
            }

            void NewGuid()
            {
                // The factory is called per target, so a multi-selection gets distinct guids
                SetOnEveryTarget(() => Guid.NewGuid().ToString());
            }

            void EmptyGuid()
            {
                var value = Guid.Empty.ToString();
                SetOnEveryTarget(() => value);
            }

            void SetOnEveryTarget(Func<string> valueFactory)
            {
                foreach (var target in targets)
                {
                    var individualObject = new SerializedObject(target);
                    var individualProperty = individualObject
                        .FindProperty(propertyPath)
                        .FindPropertyRelative(SerializedGuidField);

                    individualProperty.stringValue = valueFactory();
                    individualObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
