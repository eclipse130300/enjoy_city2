
using UnityEngine;
namespace Utils
{
    [System.Serializable]
    public class StringSimpleActionEvent : ValuePair<string, SimpleActionEvent<Object>> { }
    [System.Serializable]

    public class StringPair : ValuePair<string, string> { }
    [System.Serializable]
    public class StringIntPair : ValuePair<string, int> { }
    [System.Serializable]
    public class IntStringString : ValuePair<int, string,string> { }
    [System.Serializable]
    public class ValuePair<T, U>
    {
        [SerializeField]
        public T Value1;
        [SerializeField]
        public U Value2;
      
    }
    [System.Serializable]
    public class ValuePair<T, U, N>
    {
        [SerializeField]
        public T Value1;
        [SerializeField]
        public U Value2;
        [SerializeField]
        public N Value3;
    }
}
