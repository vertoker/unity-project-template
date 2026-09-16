using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Shared
{
    // The source name comes from the first option that is set: memberNameManual, then the generic
    // parameter T, otherwise the caller's [CallerMemberName]. The shared StringBuilder saves
    // allocations but makes the class non-thread-safe — log from the main thread only

    /// <summary>
    /// The project-wide logging entry point: every message carries a "[Source]: " prefix,
    /// colored per source name in the Editor. Use instead of <see cref="Debug"/>
    /// </summary>
    public static class GameLogger
    {
        private static readonly ILogger DefaultLogger = Debug.unityLogger;
        private static readonly StringBuilder Builder = new(1024);

        /// <summary>
        /// Appends the message as is — the Editor console renders line breaks
        /// </summary>
        private static void AppendMessageEditor(object objMessage)
        {
            Builder.Append(objMessage);
        }

        /// <summary>
        /// Appends the message with every line break collapsed into a space.
        /// Primarily for Android LogCat, which cuts a message at the first line break
        /// </summary>
        private static void AppendMessageRuntime(object objMessage)
        {
            // A notable allocation, yet Debug.Log and Builder.Append(object) do the same thing internally
            var strMessage = objMessage.ToString();
            var lengthMinusOne = strMessage.Length - 1;

            for (var i = 0; i <= lengthMinusOne; i++)
            {
                const char nextChar = '\n';
                const char returnChar = '\r';
                const char spaceChar = ' ';

                var current = strMessage[i];

                if (i < lengthMinusOne) // two characters left, "\r\n" still fits
                {
                    switch (current)
                    {
                        case nextChar:
                            Builder.Append(spaceChar);
                            break;
                        case returnChar when strMessage[i + 1] == nextChar:
                            Builder.Append(spaceChar);
                            i++;
                            break;
                        default:
                            Builder.Append(current);
                            break;
                    }
                }
                else // one character left, only a bare "\n" can match
                {
                    Builder.Append(current == nextChar ? spaceChar : current);
                }
            }
        }

        /// <summary>
        /// Builds the source prefix and the message in the shared buffer and hands the result to <see cref="DefaultLogger"/>
        /// </summary>
        private static void LogInternal(object objMessage, LogType logType,
            Object context, string memberNameManual, string memberName)
        {
            Builder.Clear();

            var typeName = string.IsNullOrEmpty(memberNameManual) ? memberName : memberNameManual;

#if UNITY_EDITOR
            // objMessage = $"[<b><color=#{hexColor}>{typeName}</color></b>]: {message}";

            // RichText is supported only in UnityEditor, so for Android LogCat and the server it makes no sense
            var hexColor = ColorNameCache.GetRandomHexColor(typeName);
            Builder.Append("[<b><color=");
            Builder.Append(hexColor);
            Builder.Append('>');
            Builder.Append(typeName);
            Builder.Append("</color></b>]: ");
#else
            // objMessage = $"[{typeName}]: {message}";

            Builder.Append('[');
            Builder.Append(typeName);
            Builder.Append("]: ");
#endif

            if (objMessage != null)
            {
#if UNITY_EDITOR
                AppendMessageEditor(objMessage);
#else
                AppendMessageRuntime(objMessage);
#endif
            }
            else
            {
                // A message-less call still says something: the member it came from
                Builder.Append(memberName);
            }

            objMessage = Builder.ToString();
            DefaultLogger.Log(logType, objMessage, context);
        }

        /// <summary>
        /// Materializes a foreign buffer into a string, since the message is assembled in the own <see cref="Builder"/>
        /// </summary>
        private static void LogInternal(StringBuilder builderMessage, LogType logType,
            Object context, string memberNameManual, string memberName)
        {
            if (builderMessage == null) return;

            var objMessage = builderMessage.ToString();
            LogInternal(objMessage, logType, context, memberNameManual, memberName);
        }

        // object (string)

        /// <summary>
        /// Logs under the name of <typeparamref name="T"/> — the form for a static or a non-owning helper
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(object message, Object context = null, string memberNameManual = "")
            => LogInternal(message, LogType.Log, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Logs under the calling member's name, or under <paramref name="memberNameManual"/> when given
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(object message, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(message, LogType.Log, context, memberNameManual, memberName);

        /// <summary>
        /// Warns under the name of <typeparamref name="T"/> — the form for a static or a non-owning helper
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning<T>(object message, Object context = null, string memberNameManual = "")
            => LogInternal(message, LogType.Warning, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Warns under the calling member's name, or under <paramref name="memberNameManual"/> when given
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(object message, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(message, LogType.Warning, context, memberNameManual, memberName);

        /// <summary>
        /// Reports an error under the name of <typeparamref name="T"/> — the form for a static or a non-owning helper
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError<T>(object message, Object context = null, string memberNameManual = "")
            => LogInternal(message, LogType.Error, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Reports an error under the calling member's name, or under <paramref name="memberNameManual"/> when given
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(object message, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(message, LogType.Error, context, memberNameManual, memberName);

        /// <summary>
        /// Reports a failed assertion under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogAssertion<T>(object message, Object context = null, string memberNameManual = "")
            => LogInternal(message, LogType.Assert, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Reports a failed assertion under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogAssertion(object message, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(message, LogType.Assert, context, memberNameManual, memberName);

        /// <summary>
        /// Reports an exception under the name of <typeparamref name="T"/>; the stack trace comes from the exception itself
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogException<T>(Exception exception, Object context = null, string memberNameManual = "")
            => LogInternal(exception, LogType.Exception, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Reports an exception under the calling member's name; the stack trace comes from the exception itself
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogException(Exception exception, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(exception, LogType.Exception, context, memberNameManual, memberName);

        // Severity as a value, for a caller that decides it at runtime. The LogType goes second on
        // purpose: first, and Log(LogType.X, someUnityObject) would be ambiguous against Log(object, Object)

        /// <summary>
        /// Logs at <paramref name="logType"/> under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(object message, LogType logType, Object context = null, string memberNameManual = "")
            => LogInternal(message, logType, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Logs at <paramref name="logType"/> under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(object message, LogType logType, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(message, logType, context, memberNameManual, memberName);

        // StringBuilder — for call sites that already build a message, to skip one ToString()

        /// <summary>
        /// Logs a prebuilt buffer under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(StringBuilder builder, Object context = null, string memberNameManual = "")
            => LogInternal(builder, LogType.Log, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Logs a prebuilt buffer under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(StringBuilder builder, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(builder, LogType.Log, context, memberNameManual, memberName);

        /// <summary>
        /// Warns with a prebuilt buffer under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning<T>(StringBuilder builder, Object context = null, string memberNameManual = "")
            => LogInternal(builder, LogType.Warning, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Warns with a prebuilt buffer under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(StringBuilder builder, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(builder, LogType.Warning, context, memberNameManual, memberName);

        /// <summary>
        /// Reports an error with a prebuilt buffer under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError<T>(StringBuilder builder, Object context = null, string memberNameManual = "")
            => LogInternal(builder, LogType.Error, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Reports an error with a prebuilt buffer under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(StringBuilder builder, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(builder, LogType.Error, context, memberNameManual, memberName);

        /// <summary>
        /// Reports a failed assertion with a prebuilt buffer under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogAssertion<T>(StringBuilder builder, Object context = null, string memberNameManual = "")
            => LogInternal(builder, LogType.Assert, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Reports a failed assertion with a prebuilt buffer under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogAssertion(StringBuilder builder, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(builder, LogType.Assert, context, memberNameManual, memberName);

        /// <summary>
        /// Logs a prebuilt buffer at <paramref name="logType"/> under the name of <typeparamref name="T"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log<T>(StringBuilder builder, LogType logType, Object context = null,
            string memberNameManual = "")
            => LogInternal(builder, logType, context, memberNameManual, typeof(T).Name);

        /// <summary>
        /// Logs a prebuilt buffer at <paramref name="logType"/> under the calling member's name
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(StringBuilder builder, LogType logType, Object context = null,
            string memberNameManual = "", [CallerMemberName] string memberName = "")
            => LogInternal(builder, logType, context, memberNameManual, memberName);
    }
}