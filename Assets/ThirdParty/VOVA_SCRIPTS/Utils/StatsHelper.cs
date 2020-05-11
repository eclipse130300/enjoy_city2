using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class StatsHelper
    {
        public static StatsHelper Instance = new StatsHelper();

        public class Stats
        {
            public float Min = float.MaxValue;

            public float Max = float.MinValue;

            public float Average;

            public int Count;
        }

        private static int _id;

        private Dictionary<int, Stats> _results = new Dictionary<int, Stats>();

        private Dictionary<int, float> _working = new Dictionary<int, float>();

        public void Start(int id)
        {
            _working[id] = Time.time;
        }

        public void Stop(int id)
        {
            float start;
            if (!_working.TryGetValue(id, out start))
                return;
            _working.Remove(id);
            Stats stat;
            if (!_results.TryGetValue(id, out stat))
            {
                stat = new Stats();
                _results[id] = stat;
            }
            float duration = Time.time - start;
            if (duration < stat.Min)
                stat.Min = duration;
            if (duration > stat.Max)
                stat.Max = duration;
            stat.Count++;
            stat.Average = (stat.Average + duration) / stat.Count;
        }

        public void Break(int id)
        {
            _working.Remove(id);
        }

        public Stats GetResult()
        {
            var results = _results.Values.ToList();
            Stats stats = new Stats();
            foreach (var result in results)
            {
                stats.Min = Mathf.Min(stats.Min, result.Min);
                stats.Max = Mathf.Max(stats.Max, result.Max);
                stats.Count += result.Count;
                stats.Average += result.Average;
            }
            stats.Average /= stats.Count;
            return stats;
        }

        public int MakeId()
        {
            return _id++;
        }

        public void Clear()
        {
            _id = 0;
            _results.Clear();
            _working.Clear();
        }

    }
}
