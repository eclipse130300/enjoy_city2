using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utils
{


    public interface IUpdateManager
    {
        void Register(IUpdatable obj);

        void UnRegister(IUpdatable obj);

        void UnRegisterAll();
    }

    public interface IUpdatable
    {
        /// <summary>
        /// Индекс обьекта сохраняется и задается автоматом
        /// </summary>
        int Index { get; set; } 
        /// <summary>
        /// Id класса если 0 то получаем автоматом от типа
        /// </summary>
        int TypeHash { get; }
        /// <summary>
        /// Максимальное количество вызовов за кадр для этого Id если 0 выполняется каждый FixedUpdate
        /// </summary>
        uint MaxRefFrame { get; }
        /// <summary>
        /// Выполняется не чаще чем. Eсли 0 выполняется каждый FixedUpdate
        /// </summary>
        uint SkipFrames { get; }

        void OnUpdate(float delta);
    }
    public class UpdateManager : BaseGameManager<UpdateManager>, IUpdateManager
    {
        private class Updatable
        {
            public IUpdatable Item;

            public uint MaxPerFrame = 0;
            public uint SkipFrames = 0;
            public uint FramesSkiped = 0;
            public float LastUpdate = 0;
            public SharedValue<uint> UpdatesCount;
        }

        private Updatable[] _objects = new Updatable[100];

        private Dictionary<int, SharedValue<uint>> _updateLimits = new Dictionary<int, SharedValue<uint>>();

        private int _index = 0;

        private const int MaxCallsPerFrame = 20000;

        private int _lastIndex = 0;

        private int _count;


        public void Register(IUpdatable obj)
        {
            int index = obj.Index;
            if (index <= 0)
            {
                index = _index++;
                obj.Index = index;
            }

            if (index >= _objects.Length)
            {
                Array.Resize(ref _objects, 2 * _objects.Length);
               
            }

            Updatable updatable = _objects[index];
            if (updatable == null)
            {
                updatable = new Updatable();
                updatable.MaxPerFrame = obj.MaxRefFrame;
                updatable.SkipFrames = obj.SkipFrames;

                _objects[index] = updatable;
               
                int hash = obj.TypeHash;
                if(hash == 0)
                {
                    hash = obj.GetType().GetHashCode();
                }

                if (!_updateLimits.ContainsKey(hash))
                {
                    _updateLimits[hash] = new SharedValue<uint>();
                }
                updatable.UpdatesCount = _updateLimits[hash];
            }

            updatable.Item = obj;
            _count++;
        }

        public void UnRegister(IUpdatable obj)
        {
            int index = obj.Index;
            if (_objects[index] != null)
            {
                _objects[index].Item = null;
                _objects[index] = null;
                _count--;
            }
        }

        public void UnRegisterAll()
        {
            //_objects.Clear();
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;
            int count = _objects.Length;
            int num = 0;
            int totalNum = 0;
         
            foreach (var item in _updateLimits.Values)
            {
                item.Value = 0;
            }

            while (count>0 && num < _count && num < MaxCallsPerFrame && totalNum <= _count)
            {
                if (_lastIndex >= count)
                    _lastIndex = 0;
                var obj = _objects[_lastIndex];
                if (obj?.Item != null)
                {
                   
                   
                    if (obj.MaxPerFrame == 0)
                    {
                        ++num;
                        if(obj.FramesSkiped >= obj.SkipFrames)
                        {
                            
                            obj.Item.OnUpdate(Time.realtimeSinceStartup - obj.LastUpdate);
                            obj.FramesSkiped = 0;
                            obj.LastUpdate = Time.realtimeSinceStartup;
                        }
                        else
                        {
                            obj.FramesSkiped++;
                        }
                            
                    }
                    else
                    {
                        ++num;
                        if (obj.FramesSkiped >= obj.SkipFrames)
                        {
                            obj.Item.OnUpdate(Time.realtimeSinceStartup - obj.LastUpdate);
                            ++obj.UpdatesCount.Value;
                            obj.FramesSkiped = 0;
                            obj.LastUpdate = Time.realtimeSinceStartup;
                        }
                        else
                        {
                            obj.FramesSkiped ++;
                        }
                    }
                    ++totalNum;
                } 
                ++_lastIndex;
            }

            
        }
    }
}
