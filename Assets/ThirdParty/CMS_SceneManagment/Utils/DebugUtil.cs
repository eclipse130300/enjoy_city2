using UnityEngine;

namespace Utils
{
    public static class DebugUtil
    {
        public static void DrawCircle(Vector3 at, float radius, Color color, int details = 24)
        {
            float angle = 0.0f;
            float step = (1.0f / (float) details) * 3.14f * 2.0f;

            for (int i = 0; i < details; ++i)
            {
                Vector3 a = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
                Vector3 b = new Vector3(Mathf.Sin(angle + step), 0.0f, Mathf.Cos(angle + step));

                a *= radius;
                b *= radius;

                a += at;
                b += at;

                angle += step;

                Debug.DrawLine(a, b, color);
            }
        }
    }
}