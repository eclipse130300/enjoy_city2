using CMS.Config;
using CMS.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CMS.Config
{
    [Serializable]
    public class ListWithNumericOperation<T> : IGUIDrawable, IEnumerable<T>, IList<T> where T:new()
    {
        [SerializeField]
        protected List<T> List = new List<T>();


        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)List).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)List).GetEnumerator();
        }
        public int Add(object value)
        {
            return ((IList)List).Add(value);
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)List).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)List).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)List).RemoveAt(index);
        }

        public void Add(T item)
        {
            ((IList<T>)List).Add(item);
        }

        public void Clear()
        {
            ((IList<T>)List).Clear();
        }

        public bool Contains(T item)
        {
            return ((IList<T>)List).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>)List).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return ((IList<T>)List).Remove(item);
        }
        public int Count => ((IList<T>)List).Count;

        public bool IsReadOnly => ((IList<T>)List).IsReadOnly;

        public T this[int index] { get { return List[index]; } set { List[index] = value; } }

        public static List<T> operator +(ListWithNumericOperation<T> b, List<T> c)
        {
            List<T> list = new List<T>();

            list.AddRange(b);
            list.AddRange(c);

            return list;
        }
        public static List<T> operator -(ListWithNumericOperation<T> b, List<T> c)
        {
            List<T> list = new List<T>();
            list.AddRange(b);
            list.RemoveAll(x => c.Contains(x));
            return list;
        }
        public static List<T> operator *(ListWithNumericOperation<T> b, List<T> c)
        {
            List<T> list = new List<T>();
            list.AddRange(b);
            list.RemoveAll(x => !c.Contains(x));
            return list;
        }
        public static List<T> operator /(ListWithNumericOperation<T> b, List<T> c)
        {
            List<T> list = new List<T>();
            list.AddRange(b);
            list.RemoveAll(x => !c.Contains(x)&&!b.Contains(x));
            return list;
        }
        #region Editor
#if UNITY_EDITOR


        public virtual void Draw()
        {
           
        }
#endif
        #endregion
    }
}
