using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class CashedRandomGenerator
    {

        protected static Queue<float> cashedValues = new Queue<float>();
        static float cashedValue;
        public static void Init(int count)
        {
            cashedValues = new Queue<float>(count);
            for (int i = 0; i < count; i++)
            {
                cashedValues.Enqueue(Random.Range(0f, 1f));
            }
        }
        public static float Range(float min, float max)
        {
            cashedValue = cashedValues.Dequeue();
            cashedValues.Enqueue(cashedValue);
            return cashedValue * (max - min) + min;
        }

    }

    public class CashedRandomInCircleGenerator
    {

        protected static Queue<Vector2> cashedValues = new Queue<Vector2>();
        static Vector2 cashedValue;
        public static void Init(int count)
        {
            cashedValues = new Queue<Vector2>(count);
            for (int i = 0; i < count; i++)
            {
                cashedValues.Enqueue(Random.insideUnitCircle);
            }
        }
        public static Vector2 Range(float radius)
        {
            cashedValue = cashedValues.Dequeue();
            cashedValues.Enqueue(cashedValue);
            return cashedValue * radius;
        }

    }
}

