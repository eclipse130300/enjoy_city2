using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utils
{
    public class Pool<T>
    {
        private T[] _objects = new T[0];
        private int _size = 0;
        private int _counter = 0;
        private readonly int _capacity = 0;
        private readonly int _growthSize = 0;
        private readonly Func<T> _factory = null;

        public Pool(int growthSize = 1, Func<T> factory = null, int capacity = int.MaxValue)
        {
            _growthSize = growthSize;
            _capacity = capacity;
            _factory = factory;
        }

        public void Clear()
        {
            Array.Clear(_objects, 0, _size);
            _size = 0;
            _counter = 0;
        }

        public T GetObject()
        {
            if (_counter >= _size)
            {
                GrowPool(_growthSize);
            }
            return _counter < _size ? _objects[_counter++] : CreateObject();
        }

        public void PutObject(T instance)
        {
            if (Array.IndexOf(_objects, instance, _counter) >= 0)
            {
                return;
            }
            if (_counter > 0)
            {
                _objects[--_counter] = instance;
            }
            else
            {
                int growthSize = TryGrowPool(_growthSize);
                if (growthSize > 0)
                {
                    _objects[_size] = instance;
                    CreateAndFillObjects(_size + 1, _size + growthSize - 1);
                    _size += growthSize;
                }
            }
        }

        public int GrowPool(int growthSize)
        {
            growthSize = TryGrowPool(growthSize);
            if (growthSize > 0)
            {
                CreateAndFillObjects(_size, _size + growthSize);
                _size += growthSize;
            }
            return growthSize;
        }

        private int TryGrowPool(int growthSize)
        {
            int availableSize = _capacity - _size;
            if (growthSize > availableSize)
            {
                growthSize = availableSize;
            }
            if (growthSize > 0)
            {
                Array.Resize(ref _objects, _size + growthSize);
            }
            return growthSize;
        }

        private void CreateAndFillObjects(int fromIndex, int length)
        {
            for (int i = fromIndex; i < length; i++)
            {
                _objects[i] = CreateObject();
            }
        }

        private T CreateObject()
        {
            return _factory != null ? _factory.Invoke() : Activator.CreateInstance<T>();
        }
    }
}
