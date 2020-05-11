using UnityEngine;
using System.Runtime.CompilerServices;

namespace Utils
{
    public static class BitsUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PackColor32(Color color)
        {
            uint r, g, b, a;
            r = (uint)(color.r * 255.0f);
            g = (uint)(color.g * 255.0f);
            b = (uint)(color.b * 255.0f);
            a = (uint)(color.a * 255.0f);

            uint hash = 0;

            hash = (hash | (r << 0));
            hash = (hash | (g << 8));
            hash = (hash | (b << 16));
            hash = (hash | (a << 24));

            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PackPositiveNormalizedFloat3ToU32(float x, float y, float z)
        {
            uint r, g, b;
            r = (uint)(x * 255.0f);
            g = (uint)(y * 255.0f);
            b = (uint)(z * 255.0f);
            
            uint hash = 0;

            hash = (hash | (r << 0));
            hash = (hash | (g << 8));
            hash = (hash | (b << 16));
            
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PackRGBAF32ToRGBU32(Color color)
        {
            return PackPositiveNormalizedFloat3ToU32(color.r, color.g, color.b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color UnpackColor32(uint hash)
        {
            Color color = new Color((hash & 0x000000FF) >> 0,
                                    (hash & 0x0000FF00) >> 8,
                                    (hash & 0x00FF0000) >> 16,
                                    (hash & 0xFF000000) >> 24);

            color /= 255.0f;

            return color;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float UIntToFloat(uint i)
        {
            byte[] bytes = System.BitConverter.GetBytes(i);
            return System.BitConverter.ToSingle(bytes, 0);
        }
    }
}