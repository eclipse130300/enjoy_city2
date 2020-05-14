using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class StandartConcurrentPool<T,U>
    {
        private readonly ConcurrentBag<T> bag = new ConcurrentBag<T>();
        private readonly Stack<T> stack = new Stack<T>();

        private readonly Func<U,T> createFucntion;
        private readonly Action<T> putFucntion;


        public StandartConcurrentPool(Func<U, T> constructor = null, Action<T> returnToPoolFunction = null)
        {
            if (constructor != null)
            {
                createFucntion = constructor;  
            }    
            else
            {
                constructor = CreateNew;
            }
            if (returnToPoolFunction != null)
            {
                putFucntion = returnToPoolFunction;    
            }
            else
            {
                putFucntion = ReturnToPool;
            }
            bag = new ConcurrentBag<T>();
        }

        public virtual T Take(U value)
        {
            T obj;
            if (!bag.TryTake(out obj))
            {
                obj = createFucntion(value);
                // bag.Add(obj);
            }
           
            return obj;
        }

        public virtual void Put(T instance)
        {
            bag.Add(instance);
            putFucntion.Invoke(instance);
        }

        protected virtual void ReturnToPool(T instance)
        {

        }
        protected virtual T CreateNew(U value)
        {
            return Activator.CreateInstance<T>();
        }
    }
}
