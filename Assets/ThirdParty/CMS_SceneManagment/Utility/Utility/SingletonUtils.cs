using UnityEngine;

namespace Utils
{
    public static class SingletonUtils
    {
        public static void SetSingleton<T>(T component, ref T instance) where T : Component
        {
            if (instance == null)
            {
                instance = component;
                if (component.transform.root == null)
                    GameObject.DontDestroyOnLoad(component.gameObject);
            }
            else
            {
                GameObject.Destroy(component.gameObject);
            }
        }
    }
}