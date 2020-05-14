using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public static float Mod(float a, float b)
        {
            return a - b * Mathf.Floor(a / b);
        }
        public static bool AlmostEqual(float a, float b,int decimalPlace)
        {
            int mod = (int)Mod(10, decimalPlace);
            return ((int)(a * mod)/mod) == ((int)(b * mod) / mod);
        }

        public static float Round(float v, float round)
        {
            float a = Mathf.Floor(v / round) * round;
            float b = Mathf.Ceil(v / round) * round;

            float av = Mathf.Abs(v - a);
            float bv = Mathf.Abs(v - b);

            if (av < bv)
                return (a);
            else
                return (b);

            // return(floorf(v / round) * round);
        }

        public static bool Approx(float x, float y, float threshold)
        {
            return Mathf.Abs(x - y) < threshold;
        }

        public static bool Approx(Vector3 lhs, Vector3 rhs, float threshold)
        {
            Vector3 delta = lhs - rhs;
            float length = delta.magnitude;
            return length < threshold;
        }
    }
}