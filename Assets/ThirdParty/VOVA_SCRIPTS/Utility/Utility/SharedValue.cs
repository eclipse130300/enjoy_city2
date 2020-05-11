using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils
{
    public class SharedValue<T>
    {
        public event Action<T, T> Change;

        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                T old = _value;
                _value = value;
                Change?.Invoke(_value, old);
            }
        }

        public SharedValue()
        {
            Value = default(T);
        }

        public SharedValue(T value)
        {
            Value = value;
        }

        public static explicit operator SharedValue<T>(T value)
        {
            return new SharedValue<T>(value);
        }

        public static implicit operator T(SharedValue<T> counter)
        {
            return counter.Value;
        }
    }
}


