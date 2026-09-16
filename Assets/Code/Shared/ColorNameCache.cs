using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Shared
{
    /// <summary>
    /// Cache of light colors derived from a name (usually a type name) for log highlighting
    /// </summary>
    public static class ColorNameCache
    {
        private const int InitialCacheCapacity = 64;
        private const byte MinLightColor = 128;

        // Key is the name hash, not the name itself: the string is only needed while computing the color
        private static readonly Dictionary<int, Color32> ColorCache = new(InitialCacheCapacity);

        /// <summary>
        /// Drops the cache, colors are recomputed on the next request
        /// </summary>
        public static void ClearCache()
        {
            ColorCache.Clear();
        }

        // The color is deterministic within a single run only: string.GetHashCode is randomized
        // at process start, so the same name gets a different color between sessions

        /// <summary>
        /// Opaque color by name, every channel no darker than <see cref="MinLightColor"/> (legible on a dark background)
        /// </summary>
        public static Color32 GetRandomColor(string typeName)
        {
            var hash = typeName.GetHashCode();
            if (ColorCache.TryGetValue(hash, out var color))
                return color;

            var rgbaBytes = BitConverter.GetBytes(hash);
            rgbaBytes[0] = (byte)(rgbaBytes[0] % MinLightColor + MinLightColor);
            rgbaBytes[1] = (byte)(rgbaBytes[1] % MinLightColor + MinLightColor);
            rgbaBytes[2] = (byte)(rgbaBytes[2] % MinLightColor + MinLightColor);
            color = new Color32(rgbaBytes[0], rgbaBytes[1], rgbaBytes[2], byte.MaxValue);

            ColorCache[hash] = color;
            return color;
        }

        /// <summary>
        /// The <see cref="GetRandomColor"/> color as "#RRGGBB" for RichText markup
        /// </summary>
        public static string GetRandomHexColor(string typeName)
        {
            var color = GetRandomColor(typeName);

            // ColorUtility.ToHtmlStringRGB allocates a second string for the '#' prefix
            var hexColor = string.Format(CultureInfo.InvariantCulture.NumberFormat,
                "#{0:X2}{1:X2}{2:X2}", color.r, color.g, color.b);
            return hexColor;
        }
    }
}
