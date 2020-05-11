
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class CoroutineWorker : BaseGameManager<CoroutineWorker>
    {
        static List<IEnumerator> coroutines = new List<IEnumerator>();
        private static  int MaxCallsPerFrame = 2000;
        private static int _lastIndex = 0;
        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(RoutineUpdate());
        }

        public static IEnumerator StartCoroutineUndirect(IEnumerator routine)
        {
            coroutines.Add(routine);

            return routine;
        }
        public static Coroutine StartCoroutineDirect(IEnumerator routine)
        {
            return (Instance as MonoBehaviour).StartCoroutine(routine);         
        }
        new public static void StopCoroutine(IEnumerator routine)
        {
            coroutines.Remove(routine);
        }

        public IEnumerator RoutineUpdate() {

            var count = coroutines.Count;
            if (coroutines.Count <= MaxCallsPerFrame)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    if (i < 0 || i >= coroutines.Count)
                        break;
                    coroutines[i].MoveNext();
                }
            }
            else
            {
                for (int i = 0; i < MaxCallsPerFrame; i++)
                {
                    if (_lastIndex < 0)
                        _lastIndex = count - 1;
                    coroutines[_lastIndex].MoveNext();
                    --_lastIndex;
                }
                for (int i = count - 1; i >= 0; i--)
                {
                    if (i < 0 || i >= coroutines.Count)
                        break;
                    coroutines[i].MoveNext();
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
